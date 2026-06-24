using UnityEngine;

public class Entry : MonoBehaviour
{
    [SerializeField] private ConfigGame configGame;
    [SerializeField] private UDPProtobufReceiver udpReceiver;
    [SerializeField] private ObjectsHandler objectsHandler;
    [SerializeField] private GameHandler gameHandler;
    [SerializeField] private OSCHandler oscHandler;

    void Awake()
    {
        udpReceiver.StartReceiving();
        objectsHandler.Initialize(udpReceiver);
        oscHandler.Initialize();
        gameHandler.Initialize(configGame, oscHandler);
    }

    void OnDestroy()
    {
        objectsHandler.Dispose();
        gameHandler.Dispose();
        oscHandler.Dispose();
    }

}
