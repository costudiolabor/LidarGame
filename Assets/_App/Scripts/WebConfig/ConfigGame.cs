using System;
using UnityEngine;
using VIZHU.Webconfig;

public class ConfigGame : ConfigurableBehaivour<SettingGame>
{
    public event Action<SettingGame> UpdateSettingEvent;

    public override void OnInit(SettingGame config)
    {
        Debug.Log("Init: ");
        this.config = config;
        UpdateSettingEvent?.Invoke(config);
    }

    public override void OnUpdate(SettingGame oldConfig, SettingGame newConfig)
    {
        Debug.Log("Updated");
        config = newConfig;
        UpdateSettingEvent?.Invoke(config);
    }

    public override void OnButtonClicked(string button)
    {
        
    }

    protected override void OnDestroy()
    {
        UpdateSettingEvent = null;
    }
}