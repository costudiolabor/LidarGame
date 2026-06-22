using UnityEngine;

public class Entry : MonoBehaviour
{
    [SerializeField] private LiDARHandler liDarHandler;
    [SerializeField] private ObjectsHandler objectsHandler;
    [SerializeField] private GameHandler gameHandler;

    void Start()
    {
        liDarHandler.Initialize();
        objectsHandler.Initialize(liDarHandler);
        gameHandler.Initialize();
    }

    void Update()
    {
        liDarHandler.Update();
    }

    void OnApplicationQuit()
    {
        objectsHandler.Dispose();
    }

}
