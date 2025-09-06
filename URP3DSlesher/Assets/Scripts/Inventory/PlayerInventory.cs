using UnityEngine;
using System;

[System.Serializable]
public class PlayerInventory
{
    public int Gold { get; private set; } = 10;
    public int HealthPotions { get; private set; } = 0;

    private const int MaxPotions = 5;
    private const float PotionHealAmount = 50f;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnPotionsChanged;
    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }
        return false;
    }
    
    public bool BuyHealthPotion(int cost = 2)
    {
        if (HealthPotions >= MaxPotions)
            return false;

        if (SpendGold(cost))
        {
            HealthPotions++;
            OnPotionsChanged?.Invoke(HealthPotions);
            return true;
        }
        return false;
    }

    public bool UseHealthPotion(HealthPlayerController healthController)
    {
        if (HealthPotions > 0)
        {
            HealthPotions--;
            OnPotionsChanged?.Invoke(HealthPotions);
            
            healthController.UseHealthPotion(PotionHealAmount);

            return true;
        }
        return false;
    }
    
    public void SetGold(int amount)
    {
        Gold = amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public void SetHealthPotions(int count)
    {
        HealthPotions = Mathf.Clamp(count, 0, MaxPotions);
        OnPotionsChanged?.Invoke(HealthPotions);
    }
    
    public void SaveToDisk()
    {
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.SetInt("Potions", HealthPotions);
        PlayerPrefs.Save();
    }

    public void LoadFromSave()
    {
        Gold = PlayerPrefs.GetInt("Gold", 10);
        HealthPotions = PlayerPrefs.GetInt("Potions", 0);

        OnGoldChanged?.Invoke(Gold);
        OnPotionsChanged?.Invoke(HealthPotions);
    }
}
