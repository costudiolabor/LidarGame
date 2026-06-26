using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private ControlPlayer controlPlayer;
    
    private Vector2 _targetPosition;
    private const float SmoothSpeed = 5.0f;

    public void Initialize(Camera cameraMain, Player player3D, Vector2 position)
    {
        controlPlayer.CameraMain = cameraMain;
        controlPlayer.Player3D = player3D.transform;
        rectTransform.position = position;
        controlPlayer.Initialize();
    }

    private void Update() 
    {
        rectTransform.position = Vector2.Lerp(rectTransform.position, _targetPosition, 
            Time.deltaTime * SmoothSpeed);
        controlPlayer.Update();
    }
    public void SetNewTargetPosition(Vector2 newPosition) => _targetPosition = newPosition;
    public void Destroy3DPlayer() => DestroyImmediate(controlPlayer.Player3D.gameObject);
    
}
