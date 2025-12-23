using UnityEngine;
using System.Collections;

public class EnemyMeleeCombat : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;
    [SerializeField] private WeaponConfig weapon;
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";

    private bool canAttack = true;
    private GameObject spawnedHitbox;

    public void Setup(EnemyBase enemyBase, WeaponConfig cfg)
    {
        owner = enemyBase;
        weapon = cfg;
    }

    public void RequestAttack()
    {
        if (!canAttack) return;
        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        canAttack = false;
        if (animator && attackTrigger.Length > 0) animator.SetTrigger(attackTrigger);

        yield return new WaitForSeconds(weapon.attackCooldown * 0.25f);

        ActivateHitWindow();

        float activeWindow = weapon.attackCooldown * 0.2f;
        yield return new WaitForSeconds(activeWindow);

        DeactivateHitWindow();

        float remaining = weapon.attackCooldown - weapon.attackCooldown * 0.25f - activeWindow;
        if (remaining > 0) yield return new WaitForSeconds(remaining);

        canAttack = true;
    }

    private void ActivateHitWindow()
    {
        if (weapon.hitboxPrefab)
        {
            spawnedHitbox = Instantiate(weapon.hitboxPrefab, transform);
            spawnedHitbox.transform.localPosition = weapon.hitboxLocalPosition;
            EnemyWeaponHitbox hb = spawnedHitbox.GetComponent<EnemyWeaponHitbox>();
            if (hb) hb.Initialize(owner, weapon.damage);
        }
        else
        {
            EnemyWeaponHitbox hb = GetComponentInChildren<EnemyWeaponHitbox>();
            if (hb) hb.Initialize(owner, weapon.damage);
        }
    }

    private void DeactivateHitWindow()
    {
        if (spawnedHitbox) Destroy(spawnedHitbox);
        else
        {
            EnemyWeaponHitbox hb = GetComponentInChildren<EnemyWeaponHitbox>();
            if (hb) hb.DisableHit();
        }
    }
}
