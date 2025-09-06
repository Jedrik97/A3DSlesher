using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float, bool> OnMove;
    public static event Action OnDodge;                  

    [SerializeField] private CustomJoystick joystick;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dodge Settings")]
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private float dodgeCooldown = 1f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundedOffset = -0.1f;

    private CharacterController controller;
    private Vector3 currentVelocity;
    private float lastDodgeTime;
    private bool isDodging;
    private float verticalVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!isDodging)
            HandleMovement();
        ApplyGravity();
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

        Vector3 move = currentVelocity;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
        
        OnMove?.Invoke(input.x, input.y, inputDir.magnitude > 0.1f);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedOffset; 
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

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
            Vector3 move = dodgeVelocity;
            move.y = verticalVelocity;
            controller.Move(move * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }
}
