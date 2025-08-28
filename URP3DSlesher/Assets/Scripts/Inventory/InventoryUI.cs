using UnityEngine;
using TMPro;
using Zenject;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI potionText;

    private PlayerInventory _inventory;
    private const int MaxPotions = 5;

    [Inject]
    public void Construct(PlayerInventory inventory)
    {
        _inventory = inventory;

        // Инициализация UI
        UpdateGold(_inventory.Gold);
        UpdatePotions(_inventory.HealthPotions);

        // Подписки на события
        _inventory.OnGoldChanged += UpdateGold;
        _inventory.OnPotionsChanged += UpdatePotions;
    }

    private void OnDestroy()
    {
        if (_inventory != null)
        {
            _inventory.OnGoldChanged -= UpdateGold;
            _inventory.OnPotionsChanged -= UpdatePotions;
        }
    }

    private void UpdateGold(int value)
    {
        if (goldText != null)
            goldText.text = $"Gold: {value}";
    }

    private void UpdatePotions(int value)
    {
        if (potionText != null)
            potionText.text = $"Potions: {value}/{MaxPotions}";
    }
}