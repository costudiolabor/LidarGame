using UnityEngine;

public class Entry : MonoBehaviour
{
    [SerializeField] private ConfigGame configGame;
    [SerializeField] private UDPProtobufReceiver receiver;
    [SerializeField] private ObjectsHandler objectsHandler;
    [SerializeField] private GameHandler gameHandler;

    void Awake()
    {
        receiver.StartReceiving();
        objectsHandler.Initialize(receiver);
        gameHandler.Initialize(configGame);
    }

    void OnApplicationQuit()
    {
        objectsHandler.Dispose();
        gameHandler.Dispose();
    }

}
