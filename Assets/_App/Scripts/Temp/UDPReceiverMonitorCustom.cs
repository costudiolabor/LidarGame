using UnityEngine;
using UnityEngine.UI;

public class UDPReceiverMonitorCustom : MonoBehaviour
{
    [SerializeField] private UDPReceiverCustom udpReceiver;
    [SerializeField] private Text statusText;
    [SerializeField] private Text statsText;
    [SerializeField] private Text coordinateText;
    [SerializeField] private float updateInterval = 1f;

    private float nextUpdateTime;

    void Start()
    {
        if (udpReceiver == null)
            udpReceiver = GetComponent<UDPReceiverCustom>();

        if (statusText == null || statsText == null)
        {
            Debug.LogWarning("UI Text components not assigned to UDPReceiverMonitor");
            enabled = false;
        }
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateUI();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    void UpdateUI()
    {
        if (udpReceiver == null) return;

        statusText.text = $"Port: {6000}\n" +
                          $"Packets: {udpReceiver.TotalPacketsReceived}\n" +
                          $"Errors: {udpReceiver.TotalDeserializationErrors}\n" +
                          $"Objects: {udpReceiver.TotalObjectsReceived}";

        if (udpReceiver.LastReceivedBatch != null)
        {
            var batch = udpReceiver.LastReceivedBatch;
            statsText.text = $"Last batch:\n" +
                             $"ID: {batch.BatchId}\n" +
                             $"Source: {batch.Source}\n" +
                             $"Objects: {batch.Objects.Count}\n" +
                             $"Time: {batch.Timestamp}\n" +
                             $"Object 0: {(batch.Objects.Count > 0 ? batch.Objects[0].X : "null")}\n" +
                             $"Object 0: {(batch.Objects.Count > 0 ? batch.Objects[0].Y : "null")}";
            
            //coordinateText.text = $"coordinate: \n "+
                                 // $"X: {batch.Objects[0].State.}"
        }
    }

    public void OnStartClicked()
    {
        udpReceiver?.StartReceiving();
    }

    public void OnStopClicked()
    {
        udpReceiver?.StopReceiving();
    }

    public void OnTestClicked()
    {
        udpReceiver?.SendTestMessage();
    }

    public void OnResetStatsClicked()
    {
        udpReceiver?.ResetStatistics();
    }
}