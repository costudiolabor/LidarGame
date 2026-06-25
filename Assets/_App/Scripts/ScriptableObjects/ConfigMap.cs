using UnityEngine;

[CreateAssetMenu(fileName = "ConfigMap", menuName = "Scriptable Objects/ConfigMap")]
public class ConfigMap : ScriptableObject
{
    [SerializeField] private MapHandler prefabMapHandler;
    public MapHandler PrefabMapHandler => prefabMapHandler;
}
