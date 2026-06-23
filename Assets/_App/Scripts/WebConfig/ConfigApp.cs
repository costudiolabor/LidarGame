using System;
using System.Collections.Generic;
using UnityEngine;
using VIZHU.Webconfig;


[Serializable]
[ConfigSettings("SettingApp", "Configs/SettingApp.json", "Основные настройки игры")]
public struct SettingApp
{
    [VisualsString("Имя", "q", "Пиши имя")]
    public string name;
    [VisualsFile("Картинка", "Textures/", "png,jpg")]
    public string file;
    [VisualsString("Описание", "w", "Тут описание")]
    public string description;
    [VisualsString("Тип", "e")]
    public string type;
    [VisualsBool("Булевое", "тоже поле", true)]
    public bool typeBool;
    [VisualsStruct("Структура", "это что-то сложное")]
    public Test testStruct;
    [VisualsList("Лист", true, description: "с секретными данными")]
    public List<Test> list;
    [VisualsButton("Кнопка", "")]
    public bool button;

    [VisualsList("Лист", false, description: "с секретными данными")]
    public List<TestEnum> listEnums;

    [VisualsList("Лист", false, description: "с секретными данными")]
    public List<Test> listTest;
}

[Serializable]
public enum TestEnum
{
    One, Two, Three
}

[Serializable]
public struct Test
{
    [VisualsString("Какой-то тип", "w", "и описание этого типа")]
    public string type;
    public string name;
    [VisualsList("Лист testEnums", false, description: "с секретными данными")]
    public List<TestEnum> testEnums;
}


 public class ConfigApp : ConfigurableBehaivour<SettingApp>
 {
     public event Action<SettingApp> UpdateSettingEvent; 
     
     public override void OnInit(SettingApp config)
     {
         Debug.Log("Init: " + config.name);
         this.config = config; 
         UpdateSettingEvent?.Invoke(config);
     }

     public override void OnUpdate(SettingApp oldConfig, SettingApp newConfig)
     {
         Debug.Log("Updated");
         config = newConfig;
         UpdateSettingEvent?.Invoke(config);
     }

     public override void OnButtonClicked(string button)
     {
         switch (button)
         {
             case "resetButton":
                 Debug.Log("Reset!");
                 break;
             
             case "saveButton":
                 Debug.Log("Save");
                 
                 
                 Save();
                 break;
         }
     }
     
     
   /*  public override void OnInit(SettingApp config)
     {
         print("Init");
         this.config = config; 
         // you can use NAME from parent WebServer.SaveConfigValue(configName);
     }

     public async override void OnButtonClicked(string button)
     {
         switch (button) 
         {
             case "resetButton":
                 this.config.typeBool = !this.config.typeBool;
                 await Save();
                 Debug.Log("Button clicked!");
                 break;
         }
     }

     public override void OnUpdate(SettingApp oldConfig, SettingApp newConfig)
     {
         Debug.Log("OnUpdate");
         config = newConfig;
         UpdateSettingEvent?.Invoke(config);
     }
     */
   
   
   
 }