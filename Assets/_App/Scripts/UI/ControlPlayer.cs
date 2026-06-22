using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    [SerializeField] private RectTransform uiElement;
    [SerializeField] private float groundLevel = 0f;
    [SerializeField] private float smoothSpeed = 5f;
    public Player Player3D { get; set; }
    public Camera CameraMain { get; set; }
    private Vector3 _targetPosition;

    private void Update()
    {
        if (uiElement == null || Player3D == null) return;
        Vector3 uiWorldPosition = uiElement.position;
        Ray ray = CameraMain.ScreenPointToRay(uiWorldPosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, groundLevel, 0));
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            _targetPosition = new Vector3(
                worldPoint.x, 
                Player3D.transform.position.y,
                worldPoint.z
            );
        }
        Player3D.transform.position = Vector3.Lerp(Player3D.transform.position, _targetPosition, Time.deltaTime * smoothSpeed);
    }
}