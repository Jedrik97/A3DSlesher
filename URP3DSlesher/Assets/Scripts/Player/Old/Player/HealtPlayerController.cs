using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class HealthPlayerController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float baseHealth = 100f;

    private float currentHealth;
    private float maxHealth;
    private bool isDead = false;

    [Header("UI for Health")]
    [SerializeField] private Image healthOrb;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Animator & Death Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deadLayerName = "DeadPlayer";
    [SerializeField] private string deadTagName = "DeadPlayer";

    private Coroutine regenCoroutine;

    // === Инъекции Zenject ===
    private PlayerStats _stats;
    private PlayerInventory _inventory;
    private GameManager _gameManager;

    [Inject]
    public void Construct(PlayerStats stats, PlayerInventory inventory, GameManager gameManager)
    {
        _stats = stats;
        _inventory = inventory;
        _gameManager = gameManager;
    }

    private void Start()
    {
        RecalculateMaxHealth();
        currentHealth = maxHealth;
        UpdateHealthBar();

        regenCoroutine = StartCoroutine(RegenerateHealth());
    }

    private void RecalculateMaxHealth()
    {
        // HP = 100 + (VIT * 20)
        maxHealth = baseHealth + (_stats != null ? _stats.vitality * 20f : 0f);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0f, maxHealth);
        UpdateHealthBar();
    }

    public void UseHealthPotion(float healAmount)
    {
        if (isDead) return;

        bool used = _inventory.UseHealthPotion(healAmount, this);
        if (used)
            Heal(healAmount);
        else
            Debug.Log("⚠ Нет зелий!");
    }

    private IEnumerator RegenerateHealth()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(1f);

            if (currentHealth < maxHealth && _stats != null)
            {
                // Regen = 0.5 + (VIT * 0.15)
                float regen = 0.5f + _stats.vitality * 0.15f;
                Heal(regen);
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
        if (deadLayer != -1)
        {
            gameObject.layer = deadLayer;
            SetLayerRecursively(transform, deadLayer);
        }

        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        var charCtrl = GetComponent<CharacterController>();
        if (charCtrl != null) charCtrl.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else if (_gameManager != null)
        {
            _gameManager.ShowDeathUI();
        }
    }

    public void OnDeathAnimationComplete()
    {
        if (_gameManager != null)
            _gameManager.ShowDeathUI();
    }

    private void SetLayerRecursively(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        foreach (Transform child in t)
            SetLayerRecursively(child, layer);
    }

    // === Getters ===
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
