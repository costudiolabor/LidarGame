using System;
using UnityEngine;


public class ControlPlayer : MonoBehaviour
{
    [SerializeField] private RectTransform uiElement;
    [SerializeField] private float groundLevel = 0f;
    [SerializeField] private float smoothSpeed = 5f;
    public Transform Player3D { get; set; }
    public Camera CameraMain { get; set; }
    private Vector3 _targetPosition;
    private Vector3 _uiWorldPosition;
    private Plane _groundPlane;
    private Ray _ray;
    private Vector3 _worldPoint;
  
    public void Initialize()
    {
        _groundPlane = new Plane(Vector3.up, new Vector3(0, groundLevel, 0));
    }
    private void Update()
    {
        if (uiElement == null || Player3D == null) return;
        _uiWorldPosition = uiElement.position;
        _ray = CameraMain.ScreenPointToRay(_uiWorldPosition);
        if (_groundPlane.Raycast(_ray, out float distance))
        {
            _worldPoint = _ray.GetPoint(distance);
            _targetPosition.x = _worldPoint.x;
            _targetPosition.y = Player3D.position.y;
            _targetPosition.z = _worldPoint.z;
        }
        
        Player3D.position = Vector3.Lerp(Player3D.position, _targetPosition, Time.deltaTime * smoothSpeed);
    }
    
}