using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float, bool> OnMove;

    [SerializeField] private CustomJoystick joystick;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerDodge dodge;

    [Header("Acceleration")]
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Walk Settings")]
    [SerializeField] private float walkSpeed = 3f;

    [Header("Run Settings")]
    [SerializeField] private float runThreshold = 0.5f;
    [SerializeField] private float runMultiplier = 1.6f;
    [SerializeField] private float runLerpSpeed = 6f;

    [Header("Speed Limit")]
    [SerializeField] private float maxRunSpeed = 10f;

    [Header("Velocity Direction Settings")]
    [SerializeField] private float velocityTurnRateDeg = 720f;

    [Header("Backpedal")]
    [SerializeField] private bool faceForwardWhileBackpedaling = true;
    [SerializeField, Range(0.3f, 1f)] private float backpedalSpeedMultiplier = 0.75f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundedOffset = -0.1f;

    private CharacterController controller;
    private Transform cam;

    private Vector3 currentDir;
    private float currentSpeed;
    private float verticalVelocity;
    private float rotationVelocity;
    private float runBlend;
    private bool wasMoving;

    private const float deadZone = 0.1f;

    public float VerticalVelocity => verticalVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform; // всегда берём основную камеру
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector2 input = joystick.InputDirection;
        float m = input.magnitude;

        if (m < deadZone)
        {
            currentSpeed = 0f;
            runBlend = Mathf.MoveTowards(runBlend, 0f, runLerpSpeed * Time.deltaTime);
            wasMoving = false;

            Vector3 dodgeVel = dodge ? dodge.AdditiveVelocity : Vector3.zero;
            Vector3 idle = new Vector3(dodgeVel.x, verticalVelocity, dodgeVel.z);
            controller.Move(idle * Time.deltaTime);

            OnMove?.Invoke(0f, 0f, false);
            return;
        }

        Vector3 f = cam.forward; f.y = 0f; f.Normalize();
        Vector3 r = cam.right;   r.y = 0f; r.Normalize();

        bool isBack = input.y < 0f;
        Vector3 desiredMoveDir = ((isBack ? -f * Mathf.Abs(input.y) : f * input.y) + r * input.x).normalized;

        if (currentDir.sqrMagnitude < 0.0001f) currentDir = desiredMoveDir;
        else currentDir = Vector3.RotateTowards(currentDir, desiredMoveDir, Mathf.Deg2Rad * velocityTurnRateDeg * Time.deltaTime, float.PositiveInfinity);

        bool shouldRun = m >= runThreshold;
        float targetRunBlend = shouldRun ? 1f : 0f;
        runBlend = Mathf.MoveTowards(runBlend, targetRunBlend, runLerpSpeed * Time.deltaTime);

        float runSpeed = Mathf.Min(stats.MoveSpeed * runMultiplier, maxRunSpeed);
        float baseTarget = Mathf.Lerp(walkSpeed, runSpeed, runBlend);
        float targetSpeed = isBack ? baseTarget * backpedalSpeedMultiplier : baseTarget;

        if (!wasMoving)
        {
            currentSpeed = walkSpeed * (isBack ? backpedalSpeedMultiplier : 1f);
            wasMoving = true;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }

        Vector3 rotDir = isBack && faceForwardWhileBackpedaling ? f : currentDir;
        float targetAngle = Mathf.Atan2(rotDir.x, rotDir.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

        Vector3 planar = currentDir * currentSpeed + (dodge ? dodge.AdditiveVelocity : Vector3.zero);
        Vector3 move = new Vector3(planar.x, verticalVelocity, planar.z);
        controller.Move(move * Time.deltaTime);

        bool isRunning = runBlend >= 0.5f;
        float animMoveZ = isBack ? -1f : 1f;
        float animMoveX = isBack ? (isRunning ? 1f : -1f) : Mathf.Lerp(-1f, 1f, runBlend);

        OnMove?.Invoke(animMoveX, animMoveZ, isRunning);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = groundedOffset;
        else verticalVelocity += gravity * Time.deltaTime;
    }
}
