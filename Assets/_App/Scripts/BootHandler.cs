using System;
using Object = UnityEngine.Object;

[Serializable]
public class BootHandler : IDisposable
{
    private MapHandler _prefabMapHandler;
    private MapHandler _mapHandler;
    private CanvasHandler _canvasHandler;
    private ConfigGame _configGame;
    private ConfigEnemy _configEnemy;
    private ConfigTower _configTower;

    public void Initialize(CanvasHandler canvasHandler, ConfigGame configGame, OSCHandler oscHandler, 
        ConfigMap configMap, ConfigEnemy configEnemy, ConfigTower configTower)
    {
        _canvasHandler = canvasHandler;
        _configGame = configGame;
        _prefabMapHandler = configMap.PrefabMapHandler;
        _configEnemy = configEnemy;
        _configTower = configTower;
        _canvasHandler.StartEvent += Start;
    }

    public void Start()
    {
        _mapHandler = Object.Instantiate(_prefabMapHandler);
        _mapHandler.Initialize(_canvasHandler, _configGame, _configEnemy, _configTower);
    }

    public void Dispose()
    {
        _canvasHandler.StartEvent -= Start;
    }
}
