using TMPro;
using UnityEngine;

public class UpdateValue : MonoBehaviour
{
    [SerializeField] private ConfigApp configApp;
    [SerializeField] private TMP_Text textInfo;

    void Start()
    {
        configApp.UpdateSettingEvent += OnUpdateConfig;
    }

    private void OnUpdateConfig(SettingApp settingApp)
    {
        string namePlayer = settingApp.name;
        textInfo.text = namePlayer;
    }

    private void OnDestroy()
    {
        configApp.UpdateSettingEvent -= OnUpdateConfig;
    }
}
