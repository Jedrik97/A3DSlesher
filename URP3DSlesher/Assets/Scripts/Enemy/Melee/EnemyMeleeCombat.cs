using UnityEngine;
using System.Collections;

public class EnemyMeleeCombat : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;
    [SerializeField] private WeaponConfig weapon;
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";

    [Header("Hit Window")]
    [SerializeField] private float hitOnNormalized = 0.25f;
    [SerializeField] private float hitOffNormalized = 0.45f;

    [Header("Refs")]
    [SerializeField] private EnemyWeaponHitbox hitbox;

    private bool canAttack = true;
    private Coroutine attackRoutine;

    private void Awake()
    {
        if (!owner) owner = GetComponent<EnemyBase>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!hitbox) hitbox = GetComponentInChildren<EnemyWeaponHitbox>(true);
    }

    public void Setup(EnemyBase enemyBase, WeaponConfig cfg)
    {
        owner = enemyBase;
        weapon = cfg;

        if (hitbox && weapon)
            hitbox.Bind(owner, weapon.damage);
    }

    public void RequestAttack()
    {
        if (!canAttack || !weapon) return;
        if (attackRoutine != null) return;

        attackRoutine = StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        canAttack = false;

        if (animator && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);

        float cd = Mathf.Max(0.05f, weapon.attackCooldown);

        float tOn = Mathf.Clamp01(hitOnNormalized) * cd;
        float tOff = Mathf.Clamp01(hitOffNormalized) * cd;
        if (tOff < tOn) tOff = tOn;

        if (tOn > 0) yield return new WaitForSeconds(tOn);
        HitOn();

        float activeTime = tOff - tOn;
        if (activeTime > 0) yield return new WaitForSeconds(activeTime);
        HitOff();

        float remain = cd - tOff;
        if (remain > 0) yield return new WaitForSeconds(remain);

        attackRoutine = null;
        canAttack = true;
    }

    public void HitOn()
    {
        if (hitbox) hitbox.SetActive(true);
    }

    public void HitOff()
    {
        if (hitbox) hitbox.SetActive(false);
    }
}
