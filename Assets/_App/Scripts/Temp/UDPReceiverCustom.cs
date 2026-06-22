using Google.Protobuf;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tracking.Protos;
using UnityEngine;

public class UDPReceiverCustom : MonoBehaviour
{
	[Header("UDP Settings")]
	[SerializeField] private int port = 5005;
	[SerializeField] private bool autoStart = true;
	[SerializeField] private int receiveBufferSize = 65536;
	[SerializeField] private bool reuseAddress = true;

	[Header("Protobuf Settings")]
	[SerializeField] private bool validateMessages = true;

	[Header("Debug")]
	[SerializeField] private bool logMessages = false;
	[SerializeField] private bool logErrors = true;

	private UdpClient udpClient;
	private Thread receiveThread;
	private bool isReceiving = false;
	private readonly object threadLock = new object();

	private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

	public event Action<TrackedObjectsBatchProto> OnDataReceived;
	public event Action<Exception> OnDeserializationError;

	public TrackedObjectsBatchProto LastReceivedBatch { get; private set; }

	public int TotalPacketsReceived { get; private set; }
	public int TotalObjectsReceived { get; private set; }
	public int TotalDeserializationErrors { get; private set; }
	public int TotalBytesReceived { get; private set; }
	
	public int Port
	{
		get => port;
		set => port = value;
	}
	
	void Update()
	{
		while (mainThreadActions.TryDequeue(out Action action))
		{
			try
			{
				action?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogError($"Error executing main thread action: {e.Message}");
			}
		}
	}

	public void StartReceiving()
	{
		if (isReceiving)
		{
			QueueMainThreadAction(() => Debug.LogWarning("UDP receiver is already running"));
			return;
		}

		try
		{
			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
			udpClient = new UdpClient();
			udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
			udpClient.Client.Bind(localEndPoint);

			udpClient.Client.ReceiveBufferSize = receiveBufferSize;
			udpClient.Client.SendBufferSize = receiveBufferSize;

			isReceiving = true;
			receiveThread = new Thread(ReceiveData)
			{
				IsBackground = true,
				Priority = System.Threading.ThreadPriority.BelowNormal
			};
			receiveThread.Start();

			QueueMainThreadAction(() => Debug.Log($"UDP receiver started on port {port}"));
		}
		catch (Exception e)
		{
			if (logErrors)
			{
				QueueMainThreadAction(() => Debug.LogError($"Failed to start UDP receiver: {e.Message}"));
			}
			StopReceiving();
		}
	}

	public void StopReceiving()
	{
		isReceiving = false;

		try
		{
			udpClient?.Close();
		}
		catch (Exception e)
		{
			if (logErrors)
			{
				QueueMainThreadAction(() => Debug.LogError($"Error closing UDP client: {e.Message}"));
			}
		}

		if (receiveThread != null && receiveThread.IsAlive)
		{
			if (!receiveThread.Join(2000))
			{
				receiveThread.Interrupt();
			}
		}

		QueueMainThreadAction(() => Debug.Log("UDP receiver stopped"));
	}

	private void ReceiveData()
	{
		IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

		while (isReceiving && udpClient != null)
		{
			try
			{
				byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

				if (receivedBytes == null || receivedBytes.Length == 0)
				{
					continue;
				}

				TotalPacketsReceived++;
				TotalBytesReceived += receivedBytes.Length;

				TryDeserializeProtobuf(receivedBytes, remoteEndPoint);
			}
			catch (ThreadInterruptedException)
			{
				break;
			}
			catch (SocketException e)
			{
				if (isReceiving && logErrors)
				{
					QueueMainThreadAction(() =>
					{
						Debug.LogError($"Socket exception: {e.SocketErrorCode} - {e.Message}");
					});

					Thread.Sleep(100);
				}
			}
			catch (Exception e)
			{
				if (isReceiving && logErrors)
				{
					QueueMainThreadAction(() =>
					{
						Debug.LogError($"Error in UDP receiver: {e.GetType().Name} - {e.Message}");
					});
				}
			}
		}
	}

	private void TryDeserializeProtobuf(byte[] data, IPEndPoint source)
	{
		try
		{
			byte[] protobufData = data;

			if (data.Length > 14 && data[0] == 0x01)
			{
				int dataLength = BitConverter.ToInt32(data, 10);
				protobufData = new byte[dataLength];
				Array.Copy(data, 14, protobufData, 0, dataLength);
			}

			TrackedObjectsBatchProto batch;

			if (validateMessages)
			{
				var parser = TrackedObjectsBatchProto.Parser;
				batch = parser.ParseFrom(protobufData);
				ValidateBatch(batch);
			}
			else
			{
				batch = TrackedObjectsBatchProto.Parser.ParseFrom(protobufData);
			}

			TotalObjectsReceived += batch.Objects.Count;

			lock (threadLock)
			{
				LastReceivedBatch = batch;
			}

			QueueMainThreadAction(() =>
			{
				HandleReceivedData(batch, source);
			});
		}
		catch (InvalidProtocolBufferException e)
		{
			TotalDeserializationErrors++;

			if (logErrors)
			{
				QueueMainThreadAction(() =>
				{
					Debug.LogError($"Protobuf deserialization error from {source}: {e.Message}");

					string hex = BitConverter.ToString(data, 0, Math.Min(data.Length, 32)).Replace("-", " ");
					Debug.Log($"First 32 bytes (hex): {hex}");

					if (data.Length == 0)
					{
						Debug.LogError("Received empty packet");
					}
					else if (data[0] == 0)
					{
						Debug.LogError("First byte is zero - likely empty or corrupted data");
					}
					else if (IsAsciiPrintable(data))
					{
						string text = System.Text.Encoding.ASCII.GetString(data, 0, Math.Min(data.Length, 100));
						Debug.LogError($"Data appears to be text, not protobuf: {text}");
					}
				});
			}

			QueueMainThreadAction(() =>
			{
				OnDeserializationError?.Invoke(e);
			});
		}
		catch (Exception e)
		{
			TotalDeserializationErrors++;

			if (logErrors)
			{
				QueueMainThreadAction(() =>
				{
					Debug.LogError($"Error processing data from {source}: {e.Message}");
				});
			}

			QueueMainThreadAction(() =>
			{
				OnDeserializationError?.Invoke(e);
			});
		}
	}

	private bool IsAsciiPrintable(byte[] data)
	{
		int printableCount = 0;
		for (int i = 0; i < Math.Min(data.Length, 100); i++)
		{
			if (data[i] >= 32 && data[i] <= 126)
				printableCount++;
		}
		return printableCount > data.Length * 0.8;
	}

	private void ValidateBatch(TrackedObjectsBatchProto batch)
	{
		if (batch == null)
			Debug.Log("Batch is null");
			//throw new InvalidProtocolBufferException("Batch is null");

		if (batch.Version <= 0)
		{
			QueueMainThreadAction(() =>
			{
				Debug.LogWarning($"Received batch with invalid version: {batch.Version}");
			});
		}

		if (batch.Timestamp == 0)
		{
			QueueMainThreadAction(() =>
			{
				Debug.LogWarning("Received batch with zero timestamp");
			});
		}
	}

	private void HandleReceivedData(TrackedObjectsBatchProto batch, IPEndPoint source)
	{
		if (logMessages)
		{
			string timeStr = batch.Timestamp > 0
				? DateTimeOffset.FromUnixTimeMilliseconds(batch.Timestamp).ToString("HH:mm:ss.fff")
				: "invalid";

			Debug.Log($"Batch #{batch.BatchId} from {batch.Source} ({source.Address}) " +
					 $"at {timeStr} with {batch.Objects.Count} objects");

			if (batch.Objects.Count > 0)
			{
				var firstObj = batch.Objects[0];
				Debug.Log($"  First object: {firstObj.ObjectId} " +
						 $"at ({firstObj.X:F2}, {firstObj.Y:F2}) " +
						 $"type: {firstObj.Type}");
			}
		}

		OnDataReceived?.Invoke(batch);
	}

	private void QueueMainThreadAction(Action action)
	{
		mainThreadActions.Enqueue(action);
	}

	public void SendTestMessage(string address = "127.0.0.1", int testPort = -1)
	{
		if (testPort == -1) testPort = port;

		try
		{
			var testBatch = CreateTestBatch();
			byte[] data = testBatch.ToByteArray();

			using (var client = new UdpClient())
			{
				int bytesSent = client.Send(data, data.Length, address, testPort);
				Debug.Log($"Sent test message ({bytesSent} bytes) to {address}:{testPort}");
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to send test message: {e.Message}");
		}
	}

	public TrackedObjectsBatchProto CreateTestBatch()
	{
		var testBatch = new TrackedObjectsBatchProto
		{
			BatchId = UnityEngine.Random.Range(1, 1000),
			Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			Source = "UnityTest",
			Version = 1
		};

		int numObjects = UnityEngine.Random.Range(1, 5);
		for (int i = 0; i < numObjects; i++)
		{
			var testObject = new TrackedObjectProto
			{
				ObjectId = $"test_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{i}",
				X = UnityEngine.Random.Range(0f, 100f),
				Y = UnityEngine.Random.Range(0f, 100f),
				Speed = UnityEngine.Random.Range(0f, 10f),
				Direction = UnityEngine.Random.Range(0f, 360f),
				State = ObjectStateProto.StateActive,
				Type = i % 2 == 0 ? "person" : "vehicle",
				Confidence = UnityEngine.Random.Range(0.5f, 0.99f),
				Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
			};

			testObject.CurrentCluster = new ClusterProto
			{
				CenterX = testObject.X,
				CenterY = testObject.Y,
				Width = UnityEngine.Random.Range(10f, 30f),
				Height = UnityEngine.Random.Range(10f, 30f),
				Area = UnityEngine.Random.Range(100f, 900f),
				PointsCount = UnityEngine.Random.Range(10, 100),
				ClusterId = $"cluster_{i}"
			};

			testObject.Metadata["test_key"] = "test_value";
			testObject.Metadata["unity_time"] = Time.time.ToString();

			testBatch.Objects.Add(testObject);
		}

		return testBatch;
	}

	public bool TryDeserializeBytes(byte[] data, out TrackedObjectsBatchProto batch)
	{
		batch = null;

		try
		{
			batch = TrackedObjectsBatchProto.Parser.ParseFrom(data);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public void ResetStatistics()
	{
		TotalPacketsReceived = 0;
		TotalObjectsReceived = 0;
		TotalDeserializationErrors = 0;
		TotalBytesReceived = 0;
	}

	void OnDestroy()
	{
		StopReceiving();
	}

	void OnApplicationQuit()
	{
		StopReceiving();
	}

	void OnDisable()
	{
		StopReceiving();
	}
}