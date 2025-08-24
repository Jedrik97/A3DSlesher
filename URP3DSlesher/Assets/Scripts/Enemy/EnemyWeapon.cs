/*using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private int damage = 20;

    private void OnEnable()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    public void EnableCollider()
    {
        if (weaponCollider == null)
        {
            return;
        }

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
        if (health != null)
            health.TakeDamage(damage);
    }
}*/