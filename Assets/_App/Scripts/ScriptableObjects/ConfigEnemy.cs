using UnityEngine;

[CreateAssetMenu(fileName = "ConfigEnemy", menuName = "Scriptable Objects/ConfigEnemy")]
public class ConfigEnemy : ScriptableObject
{
    [SerializeField] private Enemy prefabEnemy;
    public Enemy PrefabEnemy => prefabEnemy;
}
