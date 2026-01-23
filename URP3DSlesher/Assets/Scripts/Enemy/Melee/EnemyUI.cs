using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image enemyCircle;

    [Header("Enemy Base")]
    [SerializeField] private EnemyMain enemyMain;

    private Transform playerCamera;
    private bool uiVisible;

    private void OnEnable()
    {
        if (enemyMain != null)
            enemyMain.OnHealthChanged += UpdateHealthUI;

        if (Camera.main)
            playerCamera = Camera.main.transform;

        InitializeUI();
        HideUI();
    }

    private void OnDisable()
    {
        if (enemyMain != null)
            enemyMain.OnHealthChanged -= UpdateHealthUI;
    }

    private void LateUpdate()
    {
        if (!uiVisible || playerCamera == null)
            return;

        Vector3 direction = transform.position - playerCamera.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        rotation.x = 0f;
        rotation.z = 0f;

        transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ShowUI();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            HideUI();
    }

    private void InitializeUI()
    {
        if (!enemyMain) return;

        if (enemyNameText)
            enemyNameText.text = enemyMain.enemyName;

        if (healthBar)
        {
            healthBar.maxValue = enemyMain.maxHealth;
            healthBar.value = enemyMain.currentHealth;
        }
    }

    private void UpdateHealthUI(float currentHealth)
    {
        if (healthBar)
            healthBar.value = currentHealth;
    }

    private void ShowUI()
    {
        uiVisible = true;

        if (enemyNameText) enemyNameText.gameObject.SetActive(true);
        if (healthBar) healthBar.gameObject.SetActive(true);
        if (enemyCircle) enemyCircle.gameObject.SetActive(true);
    }

    private void HideUI()
    {
        uiVisible = false;

        if (enemyNameText) enemyNameText.gameObject.SetActive(false);
        if (healthBar) healthBar.gameObject.SetActive(false);
        if (enemyCircle) enemyCircle.gameObject.SetActive(false);
    }
}
