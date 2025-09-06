using UnityEngine;
using UnityEngine.EventSystems;

public class CustomJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle;
    public Vector2 InputDirection { get; private set; }
    private Vector2 center;
    private void Start()
    {
        center = background.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        Vector2 delta = pos - center;
        float radius = background.sizeDelta.x / 2f;
        InputDirection = Vector2.ClampMagnitude(delta / radius, 1f);
        handle.localPosition = InputDirection * radius;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handle.localPosition = Vector2.zero;
    }
}
