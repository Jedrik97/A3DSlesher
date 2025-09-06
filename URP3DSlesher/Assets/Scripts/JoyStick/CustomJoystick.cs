using UnityEngine;
using UnityEngine.EventSystems;

public class CustomJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public RectTransform background;
    [SerializeField] public RectTransform handle;
    public Vector2 InputDirection { get; private set; }

    private Vector2 center;
    private float radius;

    private void Start()
    {
        center = background.position;
        radius = background.sizeDelta.x * 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - center;
        InputDirection = Vector2.ClampMagnitude(delta / radius, 1f);
        handle.localPosition = InputDirection * radius;
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handle.localPosition = Vector2.zero;
    }
}