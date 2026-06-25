using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPlayer", menuName = "Scriptable Objects/ConfigPlayer")]
public class ConfigPlayer : ScriptableObject
{
    [SerializeField] private PlayerUI playerUIPrefab;
    [SerializeField] private Player player3DPrefab;
    
    public PlayerUI PlayerUIPrefab => playerUIPrefab;
    public Player Player3DPrefab => player3DPrefab;
}
