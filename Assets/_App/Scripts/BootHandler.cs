using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class BootHandler : IDisposable
{
    [SerializeField] private GameHandler prefabGameHandler;
    
    private GameHandler _gameHandler;
    private CanvasHandler _canvasHandler;
    private ConfigGame _configGame;
    private OSCHandler _oscHandler;
    public void Initialize(CanvasHandler canvasHandler, ConfigGame configGame, OSCHandler oscHandler)
    {
        _canvasHandler = canvasHandler;
        _configGame = configGame;
        _canvasHandler.StartEvent += Start;
    }

    public void Start()
    {
        _gameHandler = Object.Instantiate(prefabGameHandler);
        _gameHandler.Initialize(_canvasHandler, _configGame, _oscHandler);
    }

    public void Dispose()
    {
        _canvasHandler.StartEvent -= Start;
    }
}
