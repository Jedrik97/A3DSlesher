using System.Collections;
using UnityEngine;
using Zenject;

public class EnemyMain : MonoBehaviour
{
    [Header("Enemy Stats")] public string enemyName;
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Enemy Weapon")] 
    [SerializeField] private EnemyWeapon _enemyWeapon;
    
    private GameManager gameManager;

    public event System.Action<float> OnHealthChanged;
    public event System.Action<GameObject> OnDeath;

    private ObjectPool<EnemyBase> _pool;

    protected Animator animator;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        if (gameManager)
        {
            ApplyLevelBasedStats(gameManager.GetPlayerLevel());
        }

        animator = GetComponent<Animator>();
    }

    public void SetPool(ObjectPool<EnemyBase> pool)
    {
        this._pool = pool;
    }

    public void ApplyLevelBasedStats(int playerLevel)
    {
        if (_enemyWeapon != null)
        {
            attackDamage = 10f + (playerLevel * 5f);
        }
        maxHealth = 100f + (playerLevel * 10f);
    }

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeSelf) return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth > 0f)
        {
            animator.ResetTrigger("TakeDamage");
            animator.SetTrigger("TakeDamage");
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (animator)
            animator.SetTrigger("Die");

        OnDeath?.Invoke(gameObject);
        StartCoroutine(WaitAndReturnToPool());
    }

    private IEnumerator WaitAndReturnToPool()
    {
        yield return new WaitForSeconds(10f);
        _pool?.ReturnToPool(this);
    }
}