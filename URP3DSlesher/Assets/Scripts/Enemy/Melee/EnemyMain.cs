using UnityEngine;
using System;

public class EnemyMain : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Weapon")]
    [SerializeField] private float baseWeaponDamage = 10f;

    [Header("Animation Variants")]
    [SerializeField] private int hitVariants = 2;
    [SerializeField] private int deathVariants = 2;
    
    public event Action<float> OnHealthChanged;
    public event Action<int> OnHitAnim;
    public event Action<int> OnDeathAnim;
    public event Action<GameObject> OnDeath;

    private EnemyWeapon _enemyWeapon;

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        CacheWeapon();
        _enemyWeapon?.SetupDamage(baseWeaponDamage);
    }

    private void CacheWeapon()
    {
        if (!_enemyWeapon)
            _enemyWeapon = GetComponentInChildren<EnemyWeapon>(true);
    }

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeSelf)
            return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth > 0f)
        {
            int hitVariant = UnityEngine.Random.Range(0, hitVariants);
            OnHitAnim?.Invoke(hitVariant);
            return;
        }

        currentHealth = 0f;
        Die();
    }

    private void Die()
    {
        int deathVariant = UnityEngine.Random.Range(0, deathVariants);
        OnDeathAnim?.Invoke(deathVariant);
        OnDeath?.Invoke(gameObject);
    }
}