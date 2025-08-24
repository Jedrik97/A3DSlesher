/*using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float, bool> OnMove;
    public static event Action<bool> OnJump;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpForce = 2f;
    public float gravity = 9.81f;
    public float rotationSmoothTime = 0.1f;

    private CharacterController characterController;
    private Transform cameraTransform;

    private Vector3 moveDirection = Vector3.zero;
    private bool isJumping = false;
    private bool isRunning = false;
    private bool canMove = true;
    private float rotationVelocity = 0f;

    private void OnEnable()
    {
        PlayerInput.OnMoveInput += HandleMoveInput;
        PlayerInput.OnJumpInput += HandleJumpInput;
        PlayerCombat.OnAttackStateChanged += HandleAttackStateChanged;
    }

    private void OnDisable()
    {
        PlayerInput.OnMoveInput -= HandleMoveInput;
        PlayerInput.OnJumpInput -= HandleJumpInput;
        PlayerCombat.OnAttackStateChanged -= HandleAttackStateChanged;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void HandleMoveInput(Vector2 input)
    {
        if (!canMove)
        {
            OnMove?.Invoke(0f, 0f, false);
            return;
        }


        isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;


        if (input.magnitude < 0.1f)
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;
            OnMove?.Invoke(0f, 0f, isRunning);
            return;
        }


        Vector3 f = cameraTransform.forward;
        f.y = 0f;
        f.Normalize();
        Vector3 r = cameraTransform.right;
        r.y = 0f;
        r.Normalize();
        Vector3 desiredDir = (f * input.y + r * input.x).normalized;

        moveDirection.x = desiredDir.x * speed;
        moveDirection.z = desiredDir.z * speed;


        if (input.y >= 0f && desiredDir.sqrMagnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(desiredDir.x, desiredDir.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }


        OnMove?.Invoke(input.x, input.y, isRunning);
    }

    private void HandleJumpInput()
    {
        if (canMove && characterController.isGrounded && !isJumping)
        {
            isJumping = true;
            moveDirection.y = jumpForce;
            OnJump?.Invoke(true);
        }
    }

    private void Update()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else if (isJumping)
        {
            isJumping = false;
            OnJump?.Invoke(false);
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleAttackStateChanged(bool isAttacking)
    {
        canMove = !isAttacking;
    }
}*/