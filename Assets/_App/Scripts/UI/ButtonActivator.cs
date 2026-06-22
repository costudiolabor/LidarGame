using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonActivator : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceActive;
    public enum PositionSource { RectTransform, Transform3D }
    public PositionSource source = PositionSource.RectTransform;
    public Camera sceneCamera;
    public float holdDuration = 1.5f;
    public Vector3 checkOffset = Vector3.zero;
    public Image fillImage;
    public GameObject fillRoot;
    public UnityEngine.Events.UnityEvent<Button> onButtonActivated;

    private float holdTimer;
    private Button currentButton;
    private RectTransform selfRect;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerData;

    private void Awake()
    {
        selfRect = GetComponent<RectTransform>();
        if (sceneCamera == null) sceneCamera = Camera.main;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (EventSystem.current != null)
            pointerData = new PointerEventData(EventSystem.current);
        if (fillRoot != null) fillRoot.SetActive(false);
        if (fillImage != null) fillImage.fillAmount = 0f;
    }

    private void Update()
    {
        Vector2 screenPos = GetScreenPosition();
        Button buttonUnder = FindButtonAtScreenPosition(screenPos);
        if (buttonUnder != null && buttonUnder.interactable)
        {
            if (buttonUnder != currentButton)
            {
                currentButton = buttonUnder;
                holdTimer = 0f;
                ShowFill(true);
               // Debug.Log($"Наведение на кнопку: {buttonUnder.name}");
               audioSourceActive.Play();
            }
            holdTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTimer / holdDuration);
            if (fillImage != null)
                fillImage.fillAmount = progress;
            if (holdTimer >= holdDuration)
            {
                ActivateButton(currentButton);
                holdTimer = 0f;
            }
        }
        else
        {
            if (currentButton != null)
            {
                //Debug.Log($"Покинули кнопку: {currentButton.name}");
                currentButton = null;
                holdTimer = 0f;
                ShowFill(false);
            }
        }
    }

    private Vector2 GetScreenPosition()
    {
        Vector3 worldPos = transform.position + transform.TransformVector(checkOffset);
        if (source == PositionSource.RectTransform && selfRect != null)
        {
            return selfRect.position + (Vector3)checkOffset;
        }
        else
        {
            if (sceneCamera == null) sceneCamera = Camera.main;
            return sceneCamera.WorldToScreenPoint(worldPos);
        }
    }

    private Button FindButtonAtScreenPosition(Vector2 screenPos)
    {
        if (raycaster == null || pointerData == null) return null;

        pointerData.position = screenPos;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        foreach (var result in results)
        {
            if (result.gameObject == gameObject) continue;
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn != null) return btn;
            btn = result.gameObject.GetComponentInParent<Button>();
            if (btn != null) return btn;
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
       // Debug.Log($"✅ Кнопка активирована: {button.name}");
        button.onClick.Invoke();
        onButtonActivated?.Invoke(button);
        currentButton = null;
        holdTimer = 0f;
        ShowFill(false);
    }
}