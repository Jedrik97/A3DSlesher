using System;
using UnityEngine;
using Zenject;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<bool> OnAttackStateChanged;
    public static event Action OnAoeTriggered;
    public static event Action<int> OnComboStepChanged;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string comboParam = "ComboStep";
    [SerializeField] private string attackTrigger = "Attack";

    [Header("Combat")]
    [SerializeField, Range(1, 10)] private int maxComboSteps = 5;
    [SerializeField] private Weapon weapon;

    private bool isAttacking;
    private bool canComboWindow;
    private int currentComboStep;
    private int queuedComboStep;

    private bool hasComboParam;
    private bool hasAttackTrigger;

    private PlayerStats playerStats;
    private GameManager gameManager;

    [Inject]
    public void Construct(PlayerStats ps, GameManager gm)
    {
        playerStats = ps;
        gameManager = gm;
    }

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!animator || animator.runtimeAnimatorController == null) return;
        foreach (var p in animator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Int && p.name == comboParam) hasComboParam = true;
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == attackTrigger) hasAttackTrigger = true;
        }
    }

    public void AttackPressed()
    {
        if (!isAttacking)
        {
            TriggerAttack(1);
        }
        else if (canComboWindow)
        {
            int next = Mathf.Min(currentComboStep + 1, maxComboSteps);
            if (next > currentComboStep) queuedComboStep = next;
            canComboWindow = false;
        }
    }

    public void AoePressed()
    {
        OnAoeTriggered?.Invoke();
    }

    private void TriggerAttack(int step)
    {
        if (hasComboParam) animator.SetInteger(comboParam, step);
        if (hasAttackTrigger)
        {
            animator.ResetTrigger(attackTrigger);
            animator.SetTrigger(attackTrigger);
        }
        else
        {
            animator.Play($"Attack{step}", 0, 0f);
        }

        if (queuedComboStep == step)
        {
            isAttacking = false;
            OnAttackStateChanged?.Invoke(false);
        }

        currentComboStep = step;
        queuedComboStep = 0;
        canComboWindow = false;
        isAttacking = true;
        OnAttackStateChanged?.Invoke(true);
        OnComboStepChanged?.Invoke(step);
    }

    public void EnableComboWindow()
    {
        canComboWindow = true;
        if (queuedComboStep == currentComboStep + 1 && queuedComboStep <= maxComboSteps)
            TriggerAttack(queuedComboStep);
    }

    public void DisableComboWindow()
    {
        canComboWindow = false;
        if (queuedComboStep == currentComboStep + 1 && queuedComboStep <= maxComboSteps)
            TriggerAttack(queuedComboStep);
    }

    public void EndAttack()
    {
        isAttacking = false;
        currentComboStep = 0;
        queuedComboStep = 0;
        canComboWindow = false;
        if (hasComboParam) animator.SetInteger(comboParam, 0);
        OnAttackStateChanged?.Invoke(false);
    }

    public void EnableWeaponHitbox()
    {
        weapon?.EnableCollider(true);
    }

    public void DisableWeaponHitbox()
    {
        weapon?.EnableCollider(false);
    }
}
