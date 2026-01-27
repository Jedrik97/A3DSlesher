using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;

    private float _cachedDamage;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    public void SetupDamage(float damage)
    {
        _cachedDamage = damage;
    }

    public void EnableCollider()
    {
        if (weaponCollider)
            weaponCollider.enabled = true;
    }

    public void DisableCollider()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var health = other.GetComponent<HealthPlayerController>();
        if (!health)
            return;

        health.TakeDamage(_cachedDamage);
    }
}