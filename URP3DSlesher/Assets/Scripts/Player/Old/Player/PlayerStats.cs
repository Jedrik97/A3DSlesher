using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Level & Experience")] 
    public int level = 1;
    private float currentExp = 0f;


    public float expToNextLevel = 500f;

    [Header("Attributes")] 
    public float strength = 1f;
    public float stamina = 1f;
    public float intelligence = 1f;
    public float wisdom = 1f;

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
        {
            LevelUp();
        }

        UpdateExpBar();
    }

    public void LevelUp()
    {
        currentExp -= expToNextLevel;


        level++;


        expToNextLevel = 500f + (level - 1) * 300f;


        strength += 1f;
        stamina += 1f;
        intelligence += 1f;
        wisdom += 1f;


        availableStatPoints++;

        UpdateExpBar();
    }

    public void SpendStatPoint(string stat)
    {
        if (availableStatPoints <= 0)
        {
            Debug.LogWarning("[PlayerStats]");
            return;
        }

        switch (stat)
        {
            case "Strength":
                strength += 1f;
                break;
            case "Stamina":
                stamina += 1f;
                break;
            case "Intelligence":
                intelligence += 1f;
                break;
            case "Wisdom":
                wisdom += 1f;
                break;
            default:
                Debug.LogWarning($"[PlayerStats]");
                return;
        }

        availableStatPoints--;
        UpdateExpBar();
    }

    public void SetLevel(int newLevel, float newCurrentExp, float newExpToNextLevel)
    {
        level = newLevel;
        currentExp = newCurrentExp;
        expToNextLevel = newExpToNextLevel;
        UpdateExpBar();
    }

    public int GetLevel() => level;
    public float GetCurrentExp() => currentExp;
    public float GetExpToNextLevel() => expToNextLevel;

    public void UpdateExpBar()
    {
        if (expBarFill != null)
            expBarFill.fillAmount = currentExp / expToNextLevel;

        if (expText != null)
            expText.text = $"{Mathf.FloorToInt(currentExp)} / {Mathf.FloorToInt(expToNextLevel)} EXP";
    }
}