using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    public event System.Action<float> OnHealthChanged;
    public event System.Action<GameObject> OnDeath;
    public event System.Action OnTakeDamageAnim;
    public event System.Action OnDeathAnim;

    [Header("Weapon")]
    [SerializeField] private float baseWeaponDamage = 10f;

    private EnemyWeapon _enemyWeapon;

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        CacheWeapon();
        CalculateAndApplyWeaponDamage();
    }

    private void CacheWeapon()
    {
        if (!_enemyWeapon)
            _enemyWeapon = GetComponentInChildren<EnemyWeapon>(true);
    }

    protected virtual float CalculateWeaponDamage() => baseWeaponDamage;

    private void CalculateAndApplyWeaponDamage()
    {
        if (!_enemyWeapon)
            return;

        _enemyWeapon.SetupDamage(CalculateWeaponDamage());
    }

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeSelf)
            return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth > 0f)
        {
            OnTakeDamageAnim?.Invoke();
            return;
        }

        currentHealth = 0f;
        Die();
    }

    private void Die()
    {
        OnDeathAnim?.Invoke();
        OnDeath?.Invoke(gameObject);
    }
}