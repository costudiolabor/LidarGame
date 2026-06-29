using UnityEngine;
using System;
using extOSC;

[Serializable]
public class OSCHandler : IDisposable
{
    [SerializeField] private OSCReceiver oscReceiver;
    [SerializeField] private ConfigOSC configOsc;
    
    private string _startGame;
    private string _stopGame;
    
    public event Action StartEvent, StopEvent;

    public void Initialize()
    {
        configOsc.InitEvent += OnInit;
        configOsc.UpdateEvent += OnUpdateSetting;
    }

    private void OnInit(SettingOSC settingOsc)
    {
        UpdateSetting(settingOsc);
        Bindings();
        oscReceiver.Connect();
    }

    private void OnUpdateSetting(SettingOSC settingOsc)
    {
        UpdateSetting(settingOsc);
    }

    private void UpdateSetting(SettingOSC settingOsc)
    {
        oscReceiver.LocalHost = settingOsc.remoteIP;
        oscReceiver.LocalPort = settingOsc.remotePort;
        _startGame = settingOsc.addressStartGame;
        _stopGame = settingOsc.addressStopGame;
        Bindings();
    }
    
    private void Bindings()
    {
        oscReceiver.ClearBinds();
        oscReceiver.Bind(_startGame, OnStart);
        oscReceiver.Bind(_stopGame, OnStop);
    }

    private void OnStart(OSCMessage message) => StartEvent?.Invoke();
    private void OnStop(OSCMessage message) => StopEvent?.Invoke();

    public void Dispose()
    {
        configOsc.InitEvent -= OnInit;
        configOsc.UpdateEvent -= OnUpdateSetting;
    }
    
}
