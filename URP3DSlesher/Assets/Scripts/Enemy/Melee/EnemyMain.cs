using UnityEngine;
using System;
using System.Collections;

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

    [Header("Death Settings")]
    [SerializeField] private float fallDelay = 5f;
    [SerializeField] private float destroyDelayAfterFall = 5f;

    public event Action<float> OnHealthChanged;
    public event Action<int> OnHitAnim;
    public event Action<int> OnDeathAnim;
    public event Action<GameObject> OnDeath;

    private EnemyWeapon _enemyWeapon;
    private Rigidbody _rb;
    private Collider[] _colliders;
    private bool _dead;

    public bool IsDead => _dead;

    protected virtual void OnEnable()
    {
        _dead = false;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        CacheWeapon();
        CachePhysics();

        _enemyWeapon?.SetupDamage(baseWeaponDamage);
        _enemyWeapon?.DisableCollider();
    }

    private void CacheWeapon()
    {
        if (!_enemyWeapon)
            _enemyWeapon = GetComponentInChildren<EnemyWeapon>(true);
    }

    private void CachePhysics()
    {
        if (!_rb)
            _rb = GetComponent<Rigidbody>();

        if (_colliders == null || _colliders.Length == 0)
            _colliders = GetComponentsInChildren<Collider>(true);

        if (_rb)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_dead || !gameObject.activeSelf)
            return;

        float newHealth = currentHealth - damage;

        if (newHealth <= 0f)
        {
            _dead = true;
            currentHealth = 0f;

            _enemyWeapon?.DisableCollider();

            var animatorController = GetComponent<EnemyAnimatorController>();
            if (animatorController)
                animatorController.ForceCancelAttack();

            OnHealthChanged?.Invoke(currentHealth);

            int deathVariant = UnityEngine.Random.Range(0, deathVariants);
            OnDeathAnim?.Invoke(deathVariant);
            OnDeath?.Invoke(gameObject);

            return;
        }

        currentHealth = newHealth;
        OnHealthChanged?.Invoke(currentHealth);

        int hitVariant = UnityEngine.Random.Range(0, hitVariants);
        OnHitAnim?.Invoke(hitVariant);
    }

    public void OnDeathAnimationFinished()
    {
        StartCoroutine(FallWithDelay());
    }

    private IEnumerator FallWithDelay()
    {
        yield return new WaitForSeconds(fallDelay);
        StartFalling();
    }

    private void StartFalling()
    {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent)
            agent.enabled = false;

        foreach (var col in _colliders)
            col.enabled = false;

        if (_rb)
        {
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.linearDamping = 15f;
            _rb.mass = 0.1f;
        }

        StartCoroutine(DestroyAfterFall());
    }


    private IEnumerator DestroyAfterFall()
    {
        yield return new WaitForSeconds(destroyDelayAfterFall);
        Destroy(gameObject);
    }
}
