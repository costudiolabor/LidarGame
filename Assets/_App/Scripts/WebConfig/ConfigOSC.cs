using System;
using UnityEngine;
using VIZHU.Webconfig;

public class ConfigOSC : ConfigurableBehaivour<SettingOSC>
{
    public event Action<SettingOSC> InitEvent, UpdateEvent;

    public override void OnInit(SettingOSC config)
    {
        Debug.Log("Init: ");
        this.config = config;
        InitEvent?.Invoke(config);
        //UpdateEvent?.Invoke(config);
    }

    public override void OnUpdate(SettingOSC oldConfig, SettingOSC newConfig)
    {
        Debug.Log("Updated");
        config = newConfig;
        UpdateEvent?.Invoke(config);
    }

    public override void OnButtonClicked(string button) { }

    protected override void OnDestroy()
    {
        InitEvent = null;
        UpdateEvent = null;
        base.OnDestroy();
    }
}