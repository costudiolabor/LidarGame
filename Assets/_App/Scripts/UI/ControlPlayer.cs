using System;
using UnityEngine;

[Serializable]
public class ControlPlayer
{
    [SerializeField] private RectTransform playerUI;
    public Player Player3D { get; set; }
    public Camera CameraMain { get; set; }
   
    private Vector3 _targetPosition;
    private Vector3 _uiWorldPosition;
    private Plane _groundPlane;
    private Ray _ray;
    private Vector3 _worldPoint;
    private Transform _transformPlayer3D;
    private int _runParameterHash;

    private const float SmoothSpeed = 5f;
    private const float GroundLevel = 0f;
    private const float RotationSpeed = 10f; 
    private const float StopDistance = 0.35f;
    private const string RunParameterName = "Run";
    private float _sqrDistance;
    private float _sqrStopDistance;
    
    public void Initialize()
    {
        _groundPlane = new Plane(Vector3.up, new Vector3(0, GroundLevel, 0));
        _runParameterHash = Animator.StringToHash(RunParameterName);
        _transformPlayer3D = Player3D.transform;
        _targetPosition = _transformPlayer3D.position;
        
    }
    
    public void Update()
    {
        MoveToTarget();
        RotateTowardsTarget();
        UpdateAnimation();
    }
    
    private void MoveToTarget()
    {
        if (playerUI == null || Player3D == null) return;
        _uiWorldPosition = playerUI.position;
        _ray = CameraMain.ScreenPointToRay(_uiWorldPosition);
        if (_groundPlane.Raycast(_ray, out float distance))
        {
            _worldPoint = _ray.GetPoint(distance);
            _targetPosition.x = _worldPoint.x;
            _targetPosition.y = _transformPlayer3D.position.y;
            _targetPosition.z = _worldPoint.z;
        }

        _transformPlayer3D.position =
            Vector3.Lerp(_transformPlayer3D.position, _targetPosition, Time.deltaTime * SmoothSpeed);
        
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = _targetPosition - _transformPlayer3D.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _transformPlayer3D.rotation = Quaternion.Slerp(
            _transformPlayer3D.rotation, 
            targetRotation, 
            Time.deltaTime * RotationSpeed
        );
    }

    private void UpdateAnimation()
    {
        _sqrDistance = (_targetPosition - _transformPlayer3D.position).sqrMagnitude;
        _sqrStopDistance = StopDistance * StopDistance;

        bool isRunning = _sqrDistance > _sqrStopDistance;
        Player3D.Animator.SetBool(_runParameterHash, isRunning);
    }
    
}