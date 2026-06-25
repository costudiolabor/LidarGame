using UnityEngine;

public class Entry : MonoBehaviour
{
    [SerializeField] private ConfigGame configGame;
    [SerializeField] private ConfigEnemy configEnemy;
    [SerializeField] private ConfigTower configTower;
    [SerializeField] private ConfigMap configMap;
    
    [SerializeField] private UDPProtobufReceiver udpReceiver;
    [SerializeField] private CanvasHandler canvasHandler;
    [SerializeField] private ObjectsHandler objectsHandler;
    [SerializeField] private OSCHandler oscHandler;
    [SerializeField] private BootHandler bootHandler;
    
    void Awake()
    {
        udpReceiver.StartReceiving();
        objectsHandler.Initialize(udpReceiver);
        oscHandler.Initialize();
        canvasHandler.Initialize(oscHandler);
        bootHandler.Initialize(canvasHandler, configGame, oscHandler, configMap, configEnemy, configTower);
    }
    
    void OnDestroy()
    {
        objectsHandler.Dispose();
        oscHandler.Dispose();
        bootHandler.Dispose();
    }

}
