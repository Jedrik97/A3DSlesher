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
        cam = Camera.main.transform;
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

            Vector3 idle = new Vector3(dodge.AdditiveVelocity.x, verticalVelocity, dodge.AdditiveVelocity.z);
            controller.Move(idle * Time.deltaTime);

            OnMove?.Invoke(0f, 0f, false);
            return;
        }

        Vector3 f = cam.forward; f.y = 0f; f.Normalize();
        Vector3 r = cam.right;   r.y = 0f; r.Normalize();
        Vector3 desiredDir = (f * input.y + r * input.x).normalized;

        if (currentDir.sqrMagnitude < 0.0001f) currentDir = desiredDir;
        else currentDir = Vector3.RotateTowards(currentDir, desiredDir, Mathf.Deg2Rad * velocityTurnRateDeg * Time.deltaTime, float.PositiveInfinity);

        float targetRunBlend = Mathf.InverseLerp(runThreshold, 1f, m);
        runBlend = Mathf.MoveTowards(runBlend, targetRunBlend, runLerpSpeed * Time.deltaTime);

        float targetSpeed = runBlend <= 0f
            ? walkSpeed
            : Mathf.Lerp(walkSpeed, Mathf.Min(stats.MoveSpeed * runMultiplier, maxRunSpeed), runBlend);

        if (!wasMoving)
        {
            currentSpeed = walkSpeed;
            wasMoving = true;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }

        float targetAngle = Mathf.Atan2(desiredDir.x, desiredDir.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

        Vector3 planar = currentDir * currentSpeed + dodge.AdditiveVelocity;
        Vector3 move = new Vector3(planar.x, verticalVelocity, planar.z);
        controller.Move(move * Time.deltaTime);

        float moveX = Mathf.Lerp(-1f, 1f, runBlend);
        bool isRunning = m >= runThreshold;
        OnMove?.Invoke(moveX, 1f, isRunning);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = groundedOffset;
        else verticalVelocity += gravity * Time.deltaTime;
    }
}
