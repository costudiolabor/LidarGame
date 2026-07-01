using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class FactoryTower
{
    private TowerMain _towerPrefab;
    private ConfigTower _configTower;
    private EnemyHandler _enemyHandler;
    
    public FactoryTower(ConfigTower configTower, EnemyHandler enemyHandler)
    {
        _configTower = configTower;
        _enemyHandler = enemyHandler;
        _towerPrefab = configTower.PrefabTowerMain;
    }
    
    public TowerMain Get(Transform parent) {
        TowerMain tower = Object.Instantiate(_towerPrefab, parent);  
        tower.Initialize(_enemyHandler, _configTower);
        tower.gameObject.SetActive(false);
        return tower;
    }
}
