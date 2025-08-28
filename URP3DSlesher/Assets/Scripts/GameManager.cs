using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private PlayerInventory _inventory;
    public PlayerInventory Inventory => _inventory;

    [Inject]
    public void Construct(PlayerInventory inventory)
    {
        _inventory = inventory;
    }

    private void Awake()
    {
        // Загружаем данные
        _inventory.LoadFromSave();
    }

    public void SaveGame()
    {
        _inventory.SaveToDisk();
        Debug.Log("💾 Game saved!");
    }

    public void LoadGame()
    {
        _inventory.LoadFromSave();
        Debug.Log("📂 Game loaded!");
    }

    public void ShowDeathUI()
    {
        Debug.Log("☠️ Game Over UI");
        // Тут можно включать панель смерти
    }
}