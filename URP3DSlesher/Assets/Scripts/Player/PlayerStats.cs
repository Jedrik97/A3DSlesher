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
    public int strength = 10;   // STR
    public int agility = 10;    // AGI
    public int vitality = 10;   // VIT

    [Header("Stat Points")] 
    public int availableStatPoints = 0;

    [Header("UI Elements")] 
    public Image expBarFill;
    public TextMeshProUGUI expText;

    // ===================== START =====================
    private void Start()
    {
        UpdateExpBar();
    }

    // ===================== EXPERIENCE =====================
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

        // Новая формула опыта (из документа)
        if (level < 5)
            expToNextLevel = 100 + level * 75;
        else
            expToNextLevel = 500 * Mathf.Pow(1.3f, level - 5);

        // Автоматически растут статы
        strength++;
        agility++;
        vitality++;

        // Игроку даются свободные очки
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

    // ===================== FORMULAS =====================
    public int MaxHP => 100 + (vitality * 20);
    public float Regen => 0.5f + (vitality * 0.15f);
    public float DefensePercent => Mathf.Min(50f, vitality * 1.5f);

    public int BaseWeaponDamage(int weaponDamage = 10) => weaponDamage + (strength * 2);
    public float CritDamagePercent => 150 + (strength * 2);

    public float CritChance => Mathf.Min(50f, agility * 1.2f);
    public float AttackSpeed => 1 + (agility * 0.03f);
    public float MoveSpeed => 5 + (agility * 0.1f);

    // ===================== UI =====================
    private void UpdateExpBar()
    {
        if (expBarFill != null)
            expBarFill.fillAmount = currentExp / expToNextLevel;

        if (expText != null)
            expText.text = $"{Mathf.FloorToInt(currentExp)} / {Mathf.FloorToInt(expToNextLevel)} EXP";
    }

    // ===================== GETTERS =====================
    public int GetLevel() => level;
    public float GetCurrentExp() => currentExp;
    public float GetExpToNextLevel() => expToNextLevel;
}
