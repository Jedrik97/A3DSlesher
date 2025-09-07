using UnityEngine;
using System;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    public static event Action OnDodge;

    [Header("Dodge Settings")]
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private float dodgeCooldown = 1f;

    [Header("UI Refs")]
    [SerializeField] private CustomJoystick joystick;
    [SerializeField] private RectTransform leftZone;
    [SerializeField] private RectTransform rightZone;
    [SerializeField] private RectTransform backZone;

    private float lastDodgeTime;
    private bool isDodging;
    private Vector3 additiveVelocity;

    private bool pressed;
    private bool inLeft, inRight, inBack;

    public Vector3 AdditiveVelocity => additiveVelocity;

    private void OnEnable()
    {
        joystick.OnPointerStream += OnPointerStream;
    }

    private void OnDisable()
    {
        joystick.OnPointerStream -= OnPointerStream;
    }

    public void DodgeLeft()  => TryDodge(-transform.right);
    public void DodgeRight() => TryDodge( transform.right);
    public void DodgeBack()  => TryDodge(-transform.forward);

    private void OnPointerStream(Vector2 screenPos, bool isPressed)
    {
        pressed = isPressed;
        if (!pressed)
        {
            inLeft = false;
            inRight = false;
            inBack = false;
            return;
        }

        bool nowLeft  = leftZone  && RectTransformUtility.RectangleContainsScreenPoint(leftZone,  screenPos);
        bool nowRight = rightZone && RectTransformUtility.RectangleContainsScreenPoint(rightZone, screenPos);
        bool nowBack  = backZone  && RectTransformUtility.RectangleContainsScreenPoint(backZone,  screenPos);

        if (nowLeft  && !inLeft)  DodgeLeft();
        if (nowRight && !inRight) DodgeRight();
        if (nowBack  && !inBack)  DodgeBack();

        inLeft  = nowLeft;
        inRight = nowRight;
        inBack  = nowBack;
    }

    private void TryDodge(Vector3 direction)
    {
        if (isDodging) return;
        if (Time.time - lastDodgeTime < dodgeCooldown) return;
        StartCoroutine(DodgeRoutine(direction));
    }

    private IEnumerator DodgeRoutine(Vector3 direction)
    {
        isDodging = true;
        lastDodgeTime = Time.time;
        OnDodge?.Invoke();

        float elapsed = 0f;
        additiveVelocity = direction.normalized * (dodgeDistance / dodgeDuration);

        while (elapsed < dodgeDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        additiveVelocity = Vector3.zero;
        isDodging = false;
    }
}
