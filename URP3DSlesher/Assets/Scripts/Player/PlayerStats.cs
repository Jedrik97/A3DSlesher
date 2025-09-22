using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int level = 1;

    [Header("Attributes")]
    public int strength = 10;
    public int agility = 10;
    public int vitality = 10;

    [Header("Stat Points")]
    public int availableStatPoints = 0;

    public void SpendStatPoint(string stat)
    {
        if (availableStatPoints <= 0) return;

        switch (stat)
        {
            case "Strength": strength++; break;
            case "Agility": agility++; break;
            case "Vitality": vitality++; break;
            default: Debug.LogWarning("[PlayerStats] Invalid stat key"); return;
        }

        availableStatPoints--;
    }

    public void SetLevel(int value)
    {
        level = Mathf.Max(1, value);
    }

    public int GetLevel() => level;

    public int MaxHP => 100 + (vitality * 20);
    public float Regen => 0.5f + (vitality * 0.15f);
    public float DefensePercent => Mathf.Min(50f, vitality * 1.5f);

    public int BaseWeaponDamage(int weaponDamage = 10) => weaponDamage + (strength * 2);
    public float CritDamagePercent => 150 + (strength * 2);

    public float CritChance => Mathf.Min(50f, agility * 1.2f);
    public float AttackSpeed => 1 + (agility * 0.03f);
    public float MoveSpeed => 5 + (agility * 0.1f);
}