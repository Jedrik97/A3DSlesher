using UnityEngine;
using Zenject;

public class EnemyMain : MonoBehaviour
{
    [Header("Enemy Start Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Enemy Weapon Scrpt")]
    [SerializeField] private EnemyWeapon _enemyWeapon;

    private float _attackDamage;
    private GameManager _gameManager;
    protected Animator animator;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    protected virtual void OnEnable()
    {
        int playerLevel = _gameManager.GetPlayerLevel();

        maxHealth = 100f + playerLevel * 10f;
        currentHealth = maxHealth;
        
        _attackDamage = 10f + playerLevel * 5f;

        if (_enemyWeapon != null)
        {
            _enemyWeapon.SetupDamage(_attackDamage, playerLevel);
        }

        animator = GetComponent<Animator>();
    }

    public float GetAttackDamage()
    {
        return _attackDamage;
    }
}