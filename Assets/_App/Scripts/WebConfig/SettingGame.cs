using System;
using VIZHU.Webconfig;

[Serializable]
[ConfigSettings("SettingGame", "Configs/SettingGame.json", "Основные настройки игры")]
public struct SettingGame
{
    [VisualsSlider("Задержка спавна противников", min: 0.5f, max: 3.0f, step: 0.1f, value: 2.0f)]
    public float delaySeconds;
    
    [VisualsSlider("Задержка выстрела башни", min: 0.1f, max: 3.0f, step: 0.1f, value: 0.5f)]
    public float delayFire;
    
}
