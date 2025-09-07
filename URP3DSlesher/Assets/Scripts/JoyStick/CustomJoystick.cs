using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CustomJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(0f, 1f)] private float handleRange = 0.6f;

    public Vector2 InputDirection { get; private set; }
    public event Action<Vector2, bool> OnPointerStream;

    float radius;

    void Start()
    {
        radius = background.sizeDelta.x * 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out var localPoint);

        float maxRange = radius * handleRange;
        Vector2 clamped = Vector2.ClampMagnitude(localPoint, maxRange);
        handle.localPosition = clamped;
        InputDirection = clamped / maxRange;

        OnPointerStream?.Invoke(eventData.position, true);
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handle.localPosition = Vector2.zero;
        OnPointerStream?.Invoke(eventData.position, false);
    }
}