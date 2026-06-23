using UnityEngine;

public class Entry : MonoBehaviour
{
    [SerializeField] private UDPProtobufReceiver receiver;
    [SerializeField] private ObjectsHandler objectsHandler;
    [SerializeField] private GameHandler gameHandler;

    void Start()
    {
        receiver.StartReceiving();
        objectsHandler.Initialize(receiver);
        gameHandler.Initialize();
    }

    void OnApplicationQuit()
    {
        objectsHandler.Dispose();
        gameHandler.Dispose();
    }

}
