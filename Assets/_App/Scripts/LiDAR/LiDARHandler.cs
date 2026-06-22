using System;
using Tracking.Protos;
using UnityEngine;

[Serializable]
public class LiDARHandler
{
    [SerializeField] private SettingConnect settingConnect;
    [SerializeField] private UDPReceiverCustom receiver;
    [SerializeField] private float updateInterval = 1f;

    private float _nextUpdateTime; 
    public event Action<TrackedObjectsBatchProto> DataDoneEvent;
    
    public void Initialize()
    {
        receiver.Port = settingConnect.Port;
        receiver.StartReceiving();
    }

    public void Update()
    {
        if (Time.time >= _nextUpdateTime)
        {
            _nextUpdateTime = Time.time + updateInterval;
            TrackedObjectsBatchProto batch = receiver.LastReceivedBatch;
            DataDoneEvent?.Invoke(batch);
        }
    }
}
