using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonActivator : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceActive;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject fillRoot;
    
    private GraphicRaycaster _raycaster;
    private float _holdTimer;
    private Button _currentButton;
    private PointerEventData _pointerData;
    private const float HoldDuration = 1.5f;
    private Button _buttonUnder;
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

    private void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null) _raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (EventSystem.current != null) _pointerData = new PointerEventData(EventSystem.current);
        if (fillRoot != null) fillRoot.SetActive(false);
        if (fillImage != null) fillImage.fillAmount = 0f;
    }

    private void Update()
    {
        Vector2 screenPos = GetScreenPosition();
        _buttonUnder = FindButtonAtScreenPosition(screenPos);
        if (_buttonUnder != null && _buttonUnder.interactable)
        {
            if (_buttonUnder != _currentButton)
            {
                _currentButton = _buttonUnder;
                _holdTimer = 0f;
                ShowFill(true);
                // Debug.Log($"Наведение на кнопку");
               audioSourceActive.Play();
            }
            _holdTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(_holdTimer / HoldDuration);
            if (fillImage != null)
                fillImage.fillAmount = progress;
            if (_holdTimer >= HoldDuration)
            {
                ActivateButton(_currentButton);
                _holdTimer = 0f;
            }
        }
        else
        {
            if (_currentButton != null)
            {
                // Debug.Log($"Покинули кнопку");
                _currentButton = null;
                _holdTimer = 0f;
                ShowFill(false);
            }
        }
    }

    private Vector2 GetScreenPosition() => rectTransform.position;
    
    private Button FindButtonAtScreenPosition(Vector2 screenPos)
    {
        if (_raycaster == null || _pointerData == null) return null;
        _pointerData.position = screenPos;
        _raycastResults.Clear();
        _raycaster.Raycast(_pointerData, _raycastResults);
        GameObject currentGameObject = gameObject; 
        for (int i = 0; i < _raycastResults.Count; i++)
        {
            GameObject hitGameObject = _raycastResults[i].gameObject;
            if (hitGameObject == currentGameObject) continue;
            if (hitGameObject.TryGetComponent(out Button button)) return button;
        }

        return null;
    }


    private void ShowFill(bool show)
    {
        if (fillRoot != null) fillRoot.SetActive(show);
    }

    private void ActivateButton(Button button)
    {
        if (button == null) return;
        // Debug.Log($" Кнопка активирована: {button.name}");
        button.onClick.Invoke();
        _currentButton = null;
        _holdTimer = 0f;
        ShowFill(false);
    }
}