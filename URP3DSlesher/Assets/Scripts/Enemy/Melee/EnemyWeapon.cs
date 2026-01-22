using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;

    private float _cachedDamage;
    private int _playerLevel;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    // вызывается EnemyMain.OnEnable
    public void SetupDamage(float damage, int playerLevel)
    {
        _cachedDamage = damage;
        _playerLevel = playerLevel;
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
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponent<HealthPlayerController>();
        if (health == null) return;

        // ❗ никакой логики — только применение
        health.TakeDamage(_cachedDamage);
    }
}