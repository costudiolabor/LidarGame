using UnityEngine;

[CreateAssetMenu(fileName = "ConfigTower", menuName = "Scriptable Objects/ConfigTower")]
public class ConfigTower : ScriptableObject
{
    [SerializeField] private TowerMain prefabTowerMain;
    [SerializeField] private Bullet bulletPrefab;
    
    public TowerMain PrefabTowerMain => prefabTowerMain;
    public Bullet BulletPrefab => bulletPrefab;
}
