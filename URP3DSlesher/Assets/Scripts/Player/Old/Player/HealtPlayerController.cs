/*using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class HealthPlayerController : MonoBehaviour
{
    [Header("Health Settings")] public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI for Health")] public Image healthOrb;

    public TextMeshProUGUI healthText;

    [Header("Animator & Death Settings")] public Animator animator;
    public string deadLayerName = "DeadPlayer";
    public string deadTagName = "DeadPlayer";

    private Coroutine regenCoroutine;

    private PlayerInventory _inventory;
    private TemporaryMessageUI _tempMessageUI;
    private GameManager _gameManager;

    private bool isDead = false;

    [Inject]
    public void Construct(PlayerInventory inventory, TemporaryMessageUI tempMessageUI, GameManager gameManager)
    {
        _inventory = inventory;
        _tempMessageUI = tempMessageUI;
        _gameManager = gameManager;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        regenCoroutine = StartCoroutine(RegenerateHealth());
    }

    private void OnEnable()
    {
        PlayerInput.OnUsePotion += HandleUsePotion;
    }

    private void OnDisable()
    {
        PlayerInput.OnUsePotion -= HandleUsePotion;
    }

    private void HandleUsePotion()
    {
        if (isDead) return;

        bool used = _inventory.UseHealthPotion(this);
        if (!used)
        {
            _tempMessageUI.ShowMessage("No Potions to use!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0f, maxHealth);
        UpdateHealthBar();
    }

    public void SetHealth(float h)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(h, 0f, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(1f);
            if (currentHealth < maxHealth)
            {
                Heal(1f);
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthOrb) healthOrb.fillAmount = currentHealth / maxHealth;
        if (healthText) healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
    }


    private void Die()
    {
        isDead = true;

        gameObject.tag = deadTagName;
        int deadLayer = LayerMask.NameToLayer(deadLayerName);
        if (deadLayer == -1)
        {
            Debug.LogError($"Слой '{deadLayerName}' не найден! Добавьте его в Tags & Layers.");
        }
        else
        {
            gameObject.layer = deadLayer;
            SetLayerRecursively(transform, deadLayer);
        }

        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        var charCtrl = GetComponent<CharacterController>();
        if (charCtrl != null) charCtrl.enabled = false;

        var navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null) navAgent.enabled = false;


        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            if (_gameManager != null)
            {
                _gameManager.ShowDeathUI();
            }
        }
    }

    public void OnDeathAnimationComplete()
    {
        if (_gameManager != null)
        {
            _gameManager.ShowDeathUI();
        }
    }

    private void SetLayerRecursively(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        foreach (Transform child in t)
        {
            SetLayerRecursively(child, layer);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    public void UseHealthPotion(float healAmount)
    {
        Heal(healAmount);
    }
}*/