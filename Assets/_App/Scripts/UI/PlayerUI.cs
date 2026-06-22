using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private ControlPlayer controlPlayer;
    private Vector2 _targetPosition;

    public void Set3DPlayer(Player player3D) => controlPlayer.Player3D = player3D;
    public void SetCamera(Camera cameraMain) => controlPlayer.CameraMain = cameraMain; 
    private void Update() 
    {
        rectTransform.position = Vector2.Lerp(rectTransform.position, _targetPosition, Time.deltaTime * smoothSpeed);
    }
    public void SetNewTargetPosition(Vector2 newPosition) => _targetPosition = newPosition;
    public void SetPosition(Vector2 position) => rectTransform.position = position;
    public void Destroy3DPlayer() => DestroyImmediate(controlPlayer.Player3D.gameObject);
    
}
