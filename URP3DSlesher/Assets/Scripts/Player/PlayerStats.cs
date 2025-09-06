using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Level & Experience")] 
    [SerializeField] private int level = 1;
    [SerializeField] private float currentExp = 0f;
    [SerializeField] private float expToNextLevel = 100f;

    [Header("Attributes")] 
    public int strength = 10;  
    public int agility = 10;  
    public int vitality = 10;   

    [Header("Stat Points")] 
    public int availableStatPoints = 0;

    [Header("UI Elements")] 
    public Image expBarFill;
    public TextMeshProUGUI expText;
    
    private void Start()
    {
        UpdateExpBar();
    }
    
    public void GainExperience(float amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
            LevelUp();

        UpdateExpBar();
    }

    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        
        if (level < 5)
            expToNextLevel = 100 + level * 75;
        else
            expToNextLevel = 500 * Mathf.Pow(1.3f, level - 5);
        
        strength++;
        agility++;
        vitality++;
        
        availableStatPoints += 2;

        UpdateExpBar();
    }

    public void SpendStatPoint(string stat)
    {
        if (availableStatPoints <= 0) return;

        switch (stat)
        {
            case "Strength": strength++; break;
            case "Agility": agility++; break;
            case "Vitality": vitality++; break;
            default: Debug.LogWarning("[PlayerStats] Неверный stat key"); return;
        }

        availableStatPoints--;
    }
    
    public int MaxHP => 100 + (vitality * 20);
    public float Regen => 0.5f + (vitality * 0.15f);
    public float DefensePercent => Mathf.Min(50f, vitality * 1.5f);

    public int BaseWeaponDamage(int weaponDamage = 10) => weaponDamage + (strength * 2);
    public float CritDamagePercent => 150 + (strength * 2);

    public float CritChance => Mathf.Min(50f, agility * 1.2f);
    public float AttackSpeed => 1 + (agility * 0.03f);
    public float MoveSpeed => 5 + (agility * 0.1f);
    
    private void UpdateExpBar()
    {
        if (expBarFill != null)
            expBarFill.fillAmount = currentExp / expToNextLevel;

        if (expText != null)
            expText.text = $"{Mathf.FloorToInt(currentExp)} / {Mathf.FloorToInt(expToNextLevel)} EXP";
    }
    
    public int GetLevel() => level;
    public float GetCurrentExp() => currentExp;
    public float GetExpToNextLevel() => expToNextLevel;
}
