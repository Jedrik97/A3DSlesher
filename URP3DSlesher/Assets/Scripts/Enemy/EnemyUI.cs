/*using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private TMPro.TMP_Text enemyNameText;
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] private UnityEngine.UI.Image enemyCircle;


    [Header("Enemy Base")] 
    [SerializeField] private EnemyBase enemyBase;

    private Collider enemyCollider;

    private void OnEnable()
    {
        if (enemyBase)
        {
            enemyBase.OnHealthChanged += UpdateHealthUI;
        }

        enemyCollider = GetComponent<Collider>();

        InitializeUI();
        HideUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideUI();
        }
    }

    private void OnDisable()
    {
        if (enemyBase)
        {
            enemyBase.OnHealthChanged -= UpdateHealthUI;
        }
    }

    public void InitializeUI()
    {
        if (enemyNameText)
        {
            enemyNameText.text = enemyBase.enemyName;
        }

        if (healthBar)
        {
            healthBar.maxValue = enemyBase.maxHealth;
            healthBar.value = enemyBase.currentHealth;
        }
    }

    public void UpdateHealthUI(float currentHealth)
    {
        if (healthBar)
        {
            healthBar.value = currentHealth;
        }
    }

    public void ShowUI()
    {
        if (enemyNameText) enemyNameText.gameObject.SetActive(true);
        if (healthBar) healthBar.gameObject.SetActive(true);
        if (enemyCircle) enemyCircle.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        if (enemyNameText) enemyNameText.gameObject.SetActive(false);
        if (healthBar) healthBar.gameObject.SetActive(false);
        if (enemyCircle) enemyCircle.gameObject.SetActive(false);
    }
}*/