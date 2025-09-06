/*using UnityEngine;
using Zenject;

public class Weapon : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private Collider weaponCollider;

    private int weaponDamage;
    private float deactivateWeaponTime = 2f;

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }


    public void EnableCollider(bool enable)
    {
        if (weaponCollider == null)
        {
            Debug.LogError("weaponCollider");
            return;
        }

        weaponCollider.enabled = enable;

        if (enable)
        {
            UpdateWeaponDamage();
            Invoke(nameof(DisableCollider), deactivateWeaponTime);
        }
    }

    public void DisableCollider()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    private void UpdateWeaponDamage()
    {
        if (playerStats == null)
        {
            Debug.LogError("playerStats");
            return;
        }

        weaponDamage = 25 + Mathf.RoundToInt(playerStats.strength * 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Boss")) return;

        if (other.TryGetComponent<EnemyBase>(out var enemyBase))
        {
            enemyBase.TakeDamage(weaponDamage);
        }
    }
}*/