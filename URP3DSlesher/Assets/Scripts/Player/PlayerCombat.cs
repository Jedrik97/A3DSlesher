using System;
using UnityEngine;
using Zenject;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<bool> OnAttackStateChanged;
    public static event Action OnAoeTriggered;
    public static event Action<int> OnComboStepChanged;

    [Header("Attack Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;
    [SerializeField] private int maxComboSteps = 5;

    private bool isAttacking;
    private bool canComboWindow;
    private bool bufferedPress;
    private bool consumedThisWindow;
    private int currentComboStep;

    private PlayerStats playerStats;
    private GameManager gameManager;

    [Inject]
    public void Construct(PlayerStats ps, GameManager gm)
    {
        playerStats = ps;
        gameManager = gm;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnAoeTriggered?.Invoke();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            OnAttackButton();
    }

    public void OnAttackButton()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            currentComboStep = 1;
            OnAttackStateChanged?.Invoke(true);
            OnComboStepChanged?.Invoke(1);
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
            return;
        }

        if (canComboWindow && currentComboStep < maxComboSteps)
        {
            if (!consumedThisWindow)
            {
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack");
                consumedThisWindow = true;
                bufferedPress = false;
            }
        }
        else
        {
            bufferedPress = true;
        }
    }

    public void Anim_EnterStep(int step)
    {
        currentComboStep = Mathf.Clamp(step, 1, maxComboSteps);
        OnComboStepChanged?.Invoke(currentComboStep);
    }

    public void EnableComboWindow()
    {
        canComboWindow = true;
        animator.SetBool("CanCombo", true);

        if (bufferedPress && currentComboStep < maxComboSteps && !consumedThisWindow)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
            consumedThisWindow = true;
            bufferedPress = false;
        }
    }

    public void DisableComboWindow()
    {
        canComboWindow = false;
        animator.SetBool("CanCombo", false);
        consumedThisWindow = false;
    }

    public void EndAttack()
    {
        isAttacking = false;
        currentComboStep = 0;
        bufferedPress = false;
        canComboWindow = false;
        consumedThisWindow = false;
        animator.SetBool("CanCombo", false);
        OnAttackStateChanged?.Invoke(false);
        weapon?.DisableCollider();
        animator.ResetTrigger("Attack");
    }

    public void EnableWeaponHitbox()  => weapon?.EnableCollider(true);
    public void DisableWeaponHitbox() => weapon?.EnableCollider(false);
}
