using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<bool> OnAttackStateChanged;
    public static event Action<int> OnComboStepChanged;

    [Header("Animator/Weapon")]
    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;

    [Header("Combo")]
    [SerializeField] private int maxComboSteps = 5;

    private bool isAttacking;
    private bool canComboWindow;
    private bool bufferedPress;
    private int currentComboStep;   // фактический шаг (обновляется, когда Animator вошёл в состояние шага)
    private int queuedComboStep;    // какой шаг мы запросили триггером

    // Кнопка атаки (одна)
    public void OnAttackButton()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            OnAttackStateChanged?.Invoke(true);
            animator.SetTrigger("Attack1"); // даём старт, дальше аниматор сам
            return;
        }

        // Уже идёт удар
        if (canComboWindow && currentComboStep < maxComboSteps)
        {
            queuedComboStep = currentComboStep + 1;
            animator.SetTrigger($"Attack{queuedComboStep}"); // ставим триггер, переход сделает Animator (мягко)
            canComboWindow = false;
            bufferedPress = false;
        }
        else
        {
            // нажали рано — буферим до открытия окна
            bufferedPress = true;
        }
    }

    // ========= Методы, вызываемые ИЗ АНИМАЦИЙ (Animation Events) =========

    // Ставь событие в начале каждого клипа Attack1..Attack5
    public void Anim_EnterStep(int step)
    {
        currentComboStep = Mathf.Clamp(step, 1, maxComboSteps);
        OnComboStepChanged?.Invoke(currentComboStep);
    }

    // Открытие окна комбо — ставь событие в клипе, где должен приниматься следующий тап
    public void EnableComboWindow()
    {
        canComboWindow = true;

        // Если игрок нажал раньше — дожидаемся окна и сразу ставим триггер следующего шага
        if (bufferedPress && currentComboStep < maxComboSteps)
        {
            queuedComboStep = currentComboStep + 1;
            animator.SetTrigger($"Attack{queuedComboStep}"); // мягкий переход по настройкам Animator
            bufferedPress = false;
            canComboWindow = false;
        }
    }

    // Закрытие окна комбо — ставь событие там, где окно заканчивается
    public void DisableComboWindow()
    {
        canComboWindow = false;
    }

    // Конец всей цепочки (последний клип) — ставь событие в конце
    public void EndAttack()
    {
        isAttacking = false;
        currentComboStep = 0;
        queuedComboStep = 0;
        bufferedPress = false;
        canComboWindow = false;
        OnAttackStateChanged?.Invoke(false);
        weapon?.DisableCollider();
    }

    // Окно урона — по событиям в клипах
    public void EnableWeaponHitbox()  => weapon?.EnableCollider(true);
    public void DisableWeaponHitbox() => weapon?.DisableCollider();
}
