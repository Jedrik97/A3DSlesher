using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float, bool> OnMove; // x, z, isMoving
    public static event Action<bool> OnJump;               // isJumping
    public static event Action OnDodge;                    // уклонение

    [SerializeField] private CustomJoystick joystick;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dodge Settings")]
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private float dodgeCooldown = 1f;

    private CharacterController controller;
    private Vector3 currentVelocity;
    private float lastDodgeTime;
    private bool isDodging;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!isDodging)
            HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 input = joystick.InputDirection;
        Vector3 inputDir = new Vector3(input.x, 0f, input.y);

        float targetSpeed = stats.MoveSpeed * inputDir.magnitude;
        Vector3 desiredVelocity = inputDir.normalized * targetSpeed;

        if (inputDir.magnitude > 0.01f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputDir), rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        controller.Move(currentVelocity * Time.deltaTime);

        // отправляем событие для анимации
        OnMove?.Invoke(input.x, input.y, inputDir.magnitude > 0.1f);
    }

    // === Уклонения ===
    public void DodgeLeft() => TryDodge(-transform.right);
    public void DodgeRight() => TryDodge(transform.right);
    public void DodgeBack() => TryDodge(-transform.forward);

    private void TryDodge(Vector3 direction)
    {
        if (isDodging) return;
        if (Time.time - lastDodgeTime < dodgeCooldown) return;

        StartCoroutine(DodgeRoutine(direction));
    }

    private System.Collections.IEnumerator DodgeRoutine(Vector3 direction)
    {
        isDodging = true;
        lastDodgeTime = Time.time;
        OnDodge?.Invoke();

        float elapsed = 0f;
        Vector3 dodgeVelocity = direction.normalized * (dodgeDistance / dodgeDuration);

        while (elapsed < dodgeDuration)
        {
            controller.Move(dodgeVelocity * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }

    // === Прыжок (если нужен) ===
    public void Jump()
    {
        // тут будет твоя логика прыжка (CharacterController не имеет встроенного прыжка)
        OnJump?.Invoke(true);
    }
}
