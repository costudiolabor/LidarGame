using System;
using UnityEngine;

[Serializable]
public class TowersHandler
{
    [SerializeField] private PlaceTower[] placeTowers;

    public void Initialize(EnemyHandler enemyHandler, ConfigGame configGame, ConfigTower configTower)
    {
        for (int i = 0; i < placeTowers.Length; i++)
        {
            placeTowers[i].Initialize(enemyHandler, configTower, configGame);
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
