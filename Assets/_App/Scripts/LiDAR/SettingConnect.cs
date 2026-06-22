using UnityEngine;

[CreateAssetMenu(fileName = "SettingConnect", menuName = "Scriptable Objects/SettingConnect")]
public class SettingConnect : ScriptableObject
{
    [SerializeField] private string ip;
    [SerializeField] private int port;

    public int Port => port;
    public string IP => ip;

    




}
