/*using UnityEngine;
using System.Collections;

public class EnemyRangedCombat : MonoBehaviour
{
    [SerializeField] private EnemyArcherBase owner;
    [SerializeField] private WeaponConfig weapon;

    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator animator;
    [SerializeField] private string shootTrigger = "Shoot";

    [SerializeField] private ProjectilePool projectilePool;

    private bool canShoot = true;

    public void Setup(EnemyArcherBase enemyBase, WeaponConfig cfg)
    {
        owner = enemyBase;
        weapon = cfg;
    }

    public void RequestShot()
    {
        if (!canShoot) return;
        if (!weapon) return;
        if (!owner) return;
        if (!owner.PlayerTransform) return;
        if (!projectilePool) return;

        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        canShoot = false;

        if (animator && shootTrigger.Length > 0) animator.SetTrigger(shootTrigger);

        float delay = Mathf.Max(0f, weapon.projectileSpawnDelay);
        if (delay > 0) yield return new WaitForSeconds(delay);

        SpawnProjectile();

        float cd = Mathf.Max(0.05f, weapon.attackCooldown);
        yield return new WaitForSeconds(cd);
        canShoot = true;
    }

    private void SpawnProjectile()
    {
        Transform fp = firePoint ? firePoint : transform;

        Projectile p = projectilePool.Get(fp.position, fp.rotation);
        if (!p) return;

        p.Initialize(
            owner.PlayerTransform,
            owner.PlayerDamageReceiver,
            weapon.damage,
            weapon.projectileSpeed,
            weapon.projectileLifetime,
            owner.gameObject
        );

        p.Arm();
    }
}*/