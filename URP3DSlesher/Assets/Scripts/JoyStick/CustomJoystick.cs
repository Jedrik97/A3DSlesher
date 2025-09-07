using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CustomJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(0f, 1f)] private float handleRange = 0.6f;

    [Header("Sensitivity Sectors")]
    [SerializeField] private float forwardSector = 1.2f;
    [SerializeField] private float backwardSector = 1.2f;
    [SerializeField] private float leftSector = 0.8f;
    [SerializeField] private float rightSector = 0.8f;

    [Header("Debug Draw")]
    [SerializeField] private bool drawSectors = true;
    [SerializeField] private Color forwardColor = new Color(0f, 1f, 0f, 0.25f);
    [SerializeField] private Color backwardColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] private Color leftColor = new Color(0f, 0f, 1f, 0.25f);
    [SerializeField] private Color rightColor = new Color(1f, 1f, 0f, 0.25f);
    [SerializeField, Range(16, 256)] private int gizmoSegments = 64;

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

        Vector2 norm = clamped / maxRange;

        // усиливаем вертикаль
        if (norm.y > 0) norm.y *= forwardSector;
        else if (norm.y < 0) norm.y *= backwardSector;

        // усиливаем горизонталь
        if (norm.x > 0) norm.x *= rightSector;
        else if (norm.x < 0) norm.x *= leftSector;

        InputDirection = new Vector2(Mathf.Clamp(norm.x, -1f, 1f), Mathf.Clamp(norm.y, -1f, 1f));

        OnPointerStream?.Invoke(eventData.position, true);
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handle.localPosition = Vector2.zero;
        OnPointerStream?.Invoke(eventData.position, false);
    }

    void OnDrawGizmos()
    {
        if (!drawSectors || background == null) return;

        var rt = background;
        var rect = rt.rect;
        var centerLocal = rect.center;
        float r = radius > 0f ? radius : rect.width * 0.5f;
        float maxRange = r * handleRange;
        var centerWorld = rt.TransformPoint(centerLocal);

        // вперёд
        DrawSector(centerWorld, rt, maxRange, 90f, forwardSector, forwardColor);
        // назад
        DrawSector(centerWorld, rt, maxRange, 270f, backwardSector, backwardColor);
        // вправо
        DrawSector(centerWorld, rt, maxRange, 0f, rightSector, rightColor);
        // влево
        DrawSector(centerWorld, rt, maxRange, 180f, leftSector, leftColor);
    }

    void DrawSector(Vector3 centerWorld, RectTransform rt, float radiusLocal, float angleCenter, float scale, Color col)
    {
        if (gizmoSegments < 2) return;

        float halfAngle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Max(0.001f, scale));
        float start = angleCenter - halfAngle;
        float end = angleCenter + halfAngle;

        Gizmos.color = col;
        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= gizmoSegments; i++)
        {
            float t = i / (float)gizmoSegments;
            float ang = Mathf.Lerp(start, end, t) * Mathf.Deg2Rad;
            Vector2 pLocal = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radiusLocal + (Vector2)rt.rect.center;
            Vector3 pWorld = rt.TransformPoint(pLocal);
            if (i > 0) Gizmos.DrawLine(prev, pWorld);
            else prev = pWorld;
            prev = pWorld;
        }
        Vector3 a = rt.TransformPoint(PolarLocal(rt, radiusLocal, start));
        Vector3 b = rt.TransformPoint(PolarLocal(rt, radiusLocal, end));
        Gizmos.DrawLine(centerWorld, a);
        Gizmos.DrawLine(centerWorld, b);
    }

    Vector2 PolarLocal(RectTransform rt, float radiusLocal, float angDeg)
    {
        float rad = angDeg * Mathf.Deg2Rad;
        return (Vector2)rt.rect.center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radiusLocal;
    }
}
