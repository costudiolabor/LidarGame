using System;
using VIZHU.Webconfig;

[Serializable]
[ConfigSettings("SettingOSC", "Configs/SettingOSC.json", "Настройки OSC")]
public struct SettingOSC
{
    [VisualsString("Удаленный IP", "192.168.2.176", " Удаленный IP для сообщения")]
    public string remoteIP;

    [VisualsInt("Удаленный Port","Удаленный Port для сообщения", 8000)] 
    public int remotePort;

    [VisualsString("Адрес OSC запуск игры", "/game/start", "Адрес OSC запуск игры")]
    public string addressStartGame;

    [VisualsString("Адрес OSC остановка игры", "/game/stop", "Адрес OSC остановка игры")]
    public string addressStopGame;
}