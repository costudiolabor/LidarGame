using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class UDPReader : MonoBehaviour
{
    [Header("Настройки UDP")]
    public int port = 7001;

    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private bool isRunning = true;

    // Очередь для безопасной передачи байтов в основной поток
    private readonly Queue<byte[]> dataQueue = new Queue<byte[]>();
    private readonly object queueLock = new object();

    void Start()
    {
        InitializeUDP();
        _ = ReceiveDataAsync();
    }

    private void InitializeUDP()
    {
        try
        {
            // IPAddress.Any слушает ВСЕ сетевые карты (Ethernet, Wi-Fi), а не только 127.0.0.1
            endPoint = new IPEndPoint(IPAddress.Any, port);
            udpClient = new UdpClient(endPoint);
            
            // Отключаем исключение при получении ICMP-сообщений "Port Unreachable"
            //udpClient.Client.IOControl(IOControlCode.UdpConnectionReset, new byte[] { 0, 0, 0, 0 }, null);
            
            Debug.Log($"[UDP] Слушаю порт {port} на всех сетевых интерфейсах.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDP] Ошибка инициализации: {e.Message}");
        }
    }

    private async Task ReceiveDataAsync()
    {
        while (isRunning && udpClient != null)
        {
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                
                // Кладем сырые байты в очередь
                lock (queueLock)
                {
                    dataQueue.Enqueue(result.Buffer);
                }
            }
            catch (ObjectDisposedException)
            {
                break; // Нормальное завершение
            }
            catch (Exception e)
            {
                Debug.LogError($"[UDP] Ошибка приема: {e.Message}");
            }
        }
    }

    void Update()
    {
        lock (queueLock)
        {
            while (dataQueue.Count > 0)
            {
                byte[] data = dataQueue.Dequeue();
                ProcessLidarData(data);
            }
        }
    }

    private void ProcessLidarData(byte[] data)
    {
        // 1. Просто логируем факт получения и размер (для лидара это нормально)
        Debug.Log($"[UDP] Получен пакет от лидара! Размер: {data.Length} байт.");

        // 2. (Опционально) Вывод первых 16 байт в HEX-формате, чтобы понять, что это за данные
        int lengthToPrint = Mathf.Min(16, data.Length);
        string hex = BitConverter.ToString(data, 0, lengthToPrint);
        Debug.Log($"[UDP] Первые байты (HEX): {hex}");

        // ЗДЕСЬ будет ваша логика парсинга протокола лидара (например, парсинг XYZ координат)
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }
    }
}