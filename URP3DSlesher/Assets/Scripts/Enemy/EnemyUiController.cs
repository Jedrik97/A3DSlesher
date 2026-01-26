using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image enemyCircle;

    [Header("Enemy Base")]
    [SerializeField] private EnemyMain enemyMain;

    [Header("Enemy Name")]
    [SerializeField] private string enemyDisplayName;

    private Transform playerCamera;
    private bool uiVisible;

    private void OnEnable()
    {
        if (enemyMain)
            enemyMain.OnHealthChanged += UpdateHealthUI;

        if (Camera.main)
            playerCamera = Camera.main.transform;

        InitializeUI();
        HideUI();
    }

    private void OnDisable()
    {
        if (enemyMain)
            enemyMain.OnHealthChanged -= UpdateHealthUI;

        uiVisible = false;
    }

    private void LateUpdate()
    {
        if (!uiVisible || !playerCamera)
            return;

        Vector3 direction = transform.position - playerCamera.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void InitializeUI()
    {
        if (enemyNameText)
        {
            var nameToShow = string.IsNullOrEmpty(enemyDisplayName) ? gameObject.name : enemyDisplayName;
            enemyNameText.text = nameToShow;
        }

        if (enemyMain)
            UpdateHealthUI(enemyMain.currentHealth);
    }

    private void UpdateHealthUI(float currentHealth)
    {
        if (!healthBar || !enemyMain)
            return;

        healthBar.maxValue = enemyMain.maxHealth;
        healthBar.value = Mathf.Clamp(currentHealth, 0f, enemyMain.maxHealth);
    }

    public void SetVisible(bool value)
    {
        if (value)
            ShowUI();
        else
            HideUI();
    }

    private void ShowUI()
    {
        if (uiVisible)
            return;

        uiVisible = true;

        if (enemyNameText) enemyNameText.enabled = true;
        if (healthBar) healthBar.gameObject.SetActive(true);
        if (enemyCircle) enemyCircle.enabled = true;
    }

    private void HideUI()
    {
        if (!uiVisible)
            return;

        uiVisible = false;

        if (enemyNameText) enemyNameText.enabled = false;
        if (healthBar) healthBar.gameObject.SetActive(false);
        if (enemyCircle) enemyCircle.enabled = false;
    }
}
