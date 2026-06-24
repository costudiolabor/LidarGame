using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerView : MonoBehaviour
{
  [SerializeField] private Renderer rendererObject;
  [SerializeField] private float moveSpeed = 5f;
  [SerializeField] private TMP_Text stateText;  
  private Vector3 _targetPosition;
  public Transform ThisTransform { get; set; }
  
  private void Start() => ThisTransform = transform;

  public void SetColor() => rendererObject.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
  private void Update() 
  {
      transform.position = Vector3.MoveTowards(transform.position, _targetPosition, 
          moveSpeed * Time.deltaTime);
  }
  public void SetNewTargetPosition(Vector3 newPosition) => _targetPosition = newPosition;
  public void SetState(string state) => stateText.text = state;
  
}
