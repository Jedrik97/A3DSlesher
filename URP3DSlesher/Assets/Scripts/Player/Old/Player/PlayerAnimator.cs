using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private void OnEnable()
    {
        PlayerMovement.OnMove += HandleMovement;
        PlayerMovement.OnJump += HandleJump;
        PlayerMovement.OnDodge += HandleDodge;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void HandleMovement(float x, float z, bool isMoving)
    {
        animator.SetFloat("MoveX", x);
        animator.SetFloat("MoveZ", z);
        animator.SetBool("IsRunning", isMoving);
    }

    private void HandleJump(bool isJumping)
    {
        if (isJumping)
        {
            animator.SetBool("IsJumping", true);
            animator.SetTrigger("Jump");
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
    }

    private void HandleDodge()
    {
        animator.SetTrigger("Dodge");
    }

    private void OnDisable()
    {
        PlayerMovement.OnMove -= HandleMovement;
        PlayerMovement.OnJump -= HandleJump;
        PlayerMovement.OnDodge -= HandleDodge;
    }
}