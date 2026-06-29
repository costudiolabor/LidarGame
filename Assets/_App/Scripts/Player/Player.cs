using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
   [SerializeField] private TMP_Text textBullet;
   [SerializeField] private Animator animator;
   
   private RectTransform _rectTransform;
   private int _bullet;
   private Vector2 _targetUIPosition;
   private Camera _cameraMain;
   private Vector3 _targetPosition;
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
   public bool IsLoad { get; set; }
  
   public void Initialize(Camera cameraMain, RectTransform playerUI, Vector2 position)
   {
      _rectTransform = playerUI;
      _rectTransform.position = position;
      _cameraMain = cameraMain;
      _groundPlane = new Plane(Vector3.up, new Vector3(0, GroundLevel, 0));
      _runParameterHash = Animator.StringToHash(RunParameterName);
      _transformPlayer3D = transform;
   }
   
   private void Update() 
   {
      _rectTransform.position = Vector2.Lerp(_rectTransform.position, _targetUIPosition, 
         Time.deltaTime * SmoothSpeed);
      MoveToTarget();
      RotateTowardsTarget();
      UpdateAnimation();
   }
   
   private void MoveToTarget()
   {
      if (_rectTransform == null) return;
      _ray = _cameraMain.ScreenPointToRay(_rectTransform.position);
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
      animator.SetBool(_runParameterHash, isRunning);
   }
   
   public void SetNewTargetPosition(Vector2 newPosition) => _targetUIPosition = newPosition;
   public void DestroyPlayerUI() => DestroyImmediate(_rectTransform.gameObject);

   public void UpdateBullet(int bullet)
   {
      _bullet = bullet;
      textBullet.text = _bullet.ToString();
   }

   public int GetBullet() => _bullet;

}

