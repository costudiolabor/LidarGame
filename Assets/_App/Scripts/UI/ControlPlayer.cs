using System;
using UnityEngine;

[Serializable]
public class ControlPlayer
{
    [SerializeField] private RectTransform playerUI;
    public Transform Player3D { get; set; }
    public Camera CameraMain { get; set; }
    
    private const float SmoothSpeed = 5f;
    private Vector3 _targetPosition;
    private Vector3 _uiWorldPosition;
    private Plane _groundPlane;
    private Ray _ray;
    private Vector3 _worldPoint;
    private const float GroundLevel = 0f;

    public void Initialize()
    {
        _groundPlane = new Plane(Vector3.up, new Vector3(0, GroundLevel, 0));
    }
    
    public void Update()
    {
        if (playerUI == null || Player3D == null) return;
        _uiWorldPosition = playerUI.position;
        _ray = CameraMain.ScreenPointToRay(_uiWorldPosition);
        if (_groundPlane.Raycast(_ray, out float distance))
        {
            _worldPoint = _ray.GetPoint(distance);
            _targetPosition.x = _worldPoint.x;
            _targetPosition.y = Player3D.position.y;
            _targetPosition.z = _worldPoint.z;
        }
        
        Player3D.position = Vector3.Lerp(Player3D.position, _targetPosition, Time.deltaTime * SmoothSpeed);
    }
    
}