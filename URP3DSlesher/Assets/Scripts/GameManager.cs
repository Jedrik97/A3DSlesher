using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, IInitializable
{
    private PlayerInventory _inventory;
    public PlayerInventory Inventory => _inventory;

    [Inject]
    public void Construct(PlayerInventory inventory)
    {
        _inventory = inventory;
    }

    // Called by Zenject after all bindings are set, before any Start()
    public void Initialize()
    {
        _inventory.LoadFromSave();
        Debug.Log("📂 Save loaded (Initialize).");
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
        // тут только сигнал/вызов UI-контроллера, без Find... и CanvasGroup
    }

    // Mobile-friendly autosave
    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}