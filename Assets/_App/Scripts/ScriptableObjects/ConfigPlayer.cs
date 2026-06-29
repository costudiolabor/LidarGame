using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPlayer", menuName = "Scriptable Objects/ConfigPlayer")]
public class ConfigPlayer : ScriptableObject
{
    [SerializeField] private RectTransform playerUIPrefab;
    [SerializeField] private Player player3DPrefab;
    
    public RectTransform PlayerUIPrefab => playerUIPrefab;
    public Player Player3DPrefab => player3DPrefab;
}
