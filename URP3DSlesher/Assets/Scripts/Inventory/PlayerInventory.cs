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

    // === Gold ===
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

    // === Potions ===
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

    // === Direct setters (например для загрузки сохранений) ===
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
}