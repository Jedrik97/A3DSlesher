using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerMovement.OnMove += HandleMovement;
    }

    private void OnDisable()
    {
        PlayerMovement.OnMove -= HandleMovement;
    }

    private void HandleMovement(float moveX, float moveZ, bool isRunning)
    {
        bool moving = Mathf.Abs(moveZ) > 0.001f || Mathf.Abs(moveX) > 0.001f;

        if (!moving)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", 0f);
            animator.SetBool("IsRunning", false);
            return;
        }

        animator.SetFloat("MoveX", Mathf.Clamp(moveX, -1f, 1f));
        animator.SetFloat("MoveZ", Mathf.Clamp(moveZ, -1f, 1f));
        animator.SetBool("IsRunning", isRunning);
    }
}