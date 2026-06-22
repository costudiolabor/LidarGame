using UnityEngine;
using System;
using System.Collections.Generic;
using Tracking.Protos;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[Serializable]
public class ObjectsHandler : IDisposable
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform parentUI;
    [SerializeField] private PlayerUI playerUIPrefab;
    [SerializeField] private Camera cameraMain;
    [SerializeField] private Player player3DPrefab;
    
    private float _scaleX = 1.0f;
    private float _scaleY = 1.0f;
    private Dictionary<string, PlayerUI> activeObjects = new Dictionary<string, PlayerUI>();
    private LiDARHandler _liDarHandler;

    public void Initialize(LiDARHandler liDarHandler)
    {
        _liDarHandler = liDarHandler;
        _liDarHandler.DataDoneEvent += ProcessNewFrame;

        _scaleX = canvas.rect.size.x;
        _scaleY = canvas.rect.size.y;
    }

    public void ProcessNewFrame(TrackedObjectsBatchProto batch)
    {
        if (batch == null || batch.Objects == null) return;
        HashSet<string> currentFrameIds = new HashSet<string>();
        HashSet<string> idsToRemove = new HashSet<string>();
        foreach (var data in batch.Objects)
        {
            if (data.State == ObjectStateProto.StateDisappeared)
            {
                if (activeObjects.ContainsKey(data.ObjectId))
                {
                    idsToRemove.Add(data.ObjectId);
                }

                continue;
            }

            currentFrameIds.Add(data.ObjectId);
            if (activeObjects.ContainsKey(data.ObjectId))
            {
                Vector2 position = new Vector2(data.X, data.Y);
                ObjectStateProto state = data.State;
                UpdateUIObject(activeObjects[data.ObjectId], position, state);
            }
            else
            {
                Vector2 position = new Vector2(data.X, data.Y);
                SpawnObject(data.ObjectId, position, parentUI);
            }
        }

        foreach (var activeObject in activeObjects)
        {
            if (!currentFrameIds.Contains(activeObject.Key))
            {
                idsToRemove.Add(activeObject.Key);
            }
        }

        foreach (string id in idsToRemove)
        {
            if (activeObjects.TryGetValue(id, out var trackedObject))
            {
                Object.Destroy(trackedObject.gameObject);
                trackedObject.Destroy3DPlayer();
                activeObjects.Remove(id);
                Debug.Log($"Объект с ID {id} удален со сцены.");
            }
        }
    }

    private void SpawnObject(string id, Vector2 position, Transform parent)
    {
        PlayerUI player = Object.Instantiate(playerUIPrefab, parent);
        Player player3D = Object.Instantiate(player3DPrefab);
        player.name = $"LidarObj_ID_{id}";
        player.SetPosition(position);
        player.Set3DPlayer(player3D);
        player.SetCamera(cameraMain);
        activeObjects.Add(id, player);
        Debug.Log($"Создан новый объект с ID {id}");
    }
    
    private void UpdateUIObject(PlayerUI player, Vector2 position, ObjectStateProto state = ObjectStateProto.StateNew)
    {
        float positionX = MathF.Round(position.x, 2);
        float positionY = MathF.Round(position.y, 2);
        Vector2 targetPosition = new Vector2(positionX * _scaleX,  positionY * _scaleY);
        player.SetNewTargetPosition(targetPosition);
    }

    private void OnDisable()
    {
        foreach (var player in activeObjects.Values)
        {
            if (player != null)
            {
                player.Destroy3DPlayer();
                Object.DestroyImmediate(player);
            }
        }

        activeObjects.Clear();
    }
    public void Dispose() => _liDarHandler.DataDoneEvent -= ProcessNewFrame;
}