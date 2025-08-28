using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerInventory _inventory;
    public PlayerInventory Inventory => _inventory;

    private void Awake()
    {
        // Тут можно грузить данные из SaveSystem
        _inventory = new PlayerInventory();
        _inventory.LoadFromSave();
    }

    public void SaveGame()
    {
        _inventory.SaveToDisk();
        // + сохранить статы игрока
    }

    public void LoadGame()
    {
        _inventory.LoadFromSave();
        // + загрузить статы игрока
    }

    public void ShowDeathUI()
    {
        Debug.Log("Game Over UI");
    }
}