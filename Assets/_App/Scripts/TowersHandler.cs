using System;
using UnityEngine;

[Serializable]
public class TowersHandler
{
    [SerializeField] private PlaceTower[] placeTowers;
    [SerializeField] private TowerMain towerPrefab;
    public void Initialize(EnemyHandler enemyHandler)
    {
        for (int i = 0; i < placeTowers.Length; i++)
        {
            placeTowers[i].Initialize(enemyHandler, towerPrefab);
        }
    }
    
    public void Stop()
    {
        for (int i = 0; i < placeTowers.Length; i++)
        {
            placeTowers[i].IsPlay(false);
        }   
    }
    
}
