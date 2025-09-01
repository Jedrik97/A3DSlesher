/*using System;
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


    private bool isAttacking = false;
    private bool canComboWindow = false;
    private int currentComboStep = 0;
    private int queuedComboStep = 0;

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


        if (!isAttacking)
        {
            for (int step = 1; step <= 5; step++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + step))
                {
                    TriggerAttack(step);
                    break;
                }
            }
        }
        else
        {
            if (canComboWindow)
            {
                int nextStep = currentComboStep + 1;
                if (nextStep <= 5 && Input.GetKeyDown(KeyCode.Alpha0 + nextStep))
                {
                    queuedComboStep = nextStep;

                    canComboWindow = false;
                }
            }
        }
    }


    private void TriggerAttack(int step)
    {
        animator.SetTrigger($"Attack{step}");


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

        if (queuedComboStep == currentComboStep + 1)
        {
            TriggerAttack(queuedComboStep);
        }
    }

    public void DisableComboWindow()
    {
        canComboWindow = false;

        if (queuedComboStep == currentComboStep + 1)
        {
            TriggerAttack(queuedComboStep);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        currentComboStep = 0;
        queuedComboStep = 0;
        canComboWindow = false;
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
}*/