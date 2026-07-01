using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class FactoryPlayer {
    private Player _prefab;
    private RectTransform _prefabUI;
    
    public FactoryPlayer(ConfigPlayer configPlayer)
    {
        _prefab = configPlayer.Player3DPrefab;
        _prefabUI = configPlayer.PlayerUIPrefab;
    }
    
    public Player Get(Camera cameraMain, Vector3 position, Transform parent) {
        RectTransform playerUI = Object.Instantiate(_prefabUI, parent);
        Player player = Object.Instantiate(_prefab, null);  
        player.Initialize(cameraMain, playerUI, position);
        return player;
    }
}