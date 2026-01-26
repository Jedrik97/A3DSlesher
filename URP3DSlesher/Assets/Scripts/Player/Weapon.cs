using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private int baseDamage = 25;
    [SerializeField] private float activeHitWindow = 2f;
    [SerializeField] private LayerMask enemyLayer;

    private PlayerStats playerStats;
    private int weaponDamage;
    private Coroutine disableRoutine;
    private readonly HashSet<int> hitIds = new HashSet<int>();

    [Inject]
    public void Construct(PlayerStats stats)
    {
        playerStats = stats;
    }

    private void OnEnable()
    {
        if (weaponCollider) weaponCollider.enabled = false;
        hitIds.Clear();
    }

    public void EnableCollider(bool enable)
    {
        if (!weaponCollider) return;

        weaponCollider.enabled = enable;
        if (enable)
        {
            UpdateWeaponDamage();
            hitIds.Clear();
            if (disableRoutine != null) StopCoroutine(disableRoutine);
            disableRoutine = StartCoroutine(DisableAfter(activeHitWindow));
        }
    }

    public void DisableCollider()
    {
        if (weaponCollider) weaponCollider.enabled = false;
        if (disableRoutine != null) StopCoroutine(disableRoutine);
        disableRoutine = null;
        hitIds.Clear();
    }

    private IEnumerator DisableAfter(float t)
    {
        yield return new WaitForSeconds(t);
        DisableCollider();
    }

    private void UpdateWeaponDamage()
    {
        if (!playerStats) return;
        weaponDamage = baseDamage + Mathf.RoundToInt(playerStats.strength * 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        int id = other.GetInstanceID();
        if (hitIds.Contains(id)) return;
        hitIds.Add(id);

        if (other.TryGetComponent<EnemyMain>(out var enemy))
        {
            enemy.TakeDamage(weaponDamage);
        }
    }
}
