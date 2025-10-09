using UnityEngine;

public enum XpFormulaType { Linear, Quadratic, Exponential, Hybrid }

[CreateAssetMenu(menuName = "Configs/Stats Formula Config")]
public class StatsFormulaConfig : ScriptableObject
{
    [Header("Base values")]
    public int baseHp = 100;
    public float baseMoveSpeed = 5f;

    [Header("Per Stat Point")]
    public int hpPerVit = 20;
    public float damagePerStr = 0.02f;
    public float attackSpeedPerAgi = 0.03f;

    [Header("Xp Formula")]
    public XpFormulaType xpFormula = XpFormulaType.Hybrid;

    [Header("Linear")] 
    public int linearBase = 100;
    public int linearStep = 50;
    
    [Header("Quadratic")]
    public int quadBase = 50;
    public float quadA = 8f;
    public float quadB = 30f;

    [Header("Exponential")] 
    public int expBaseValue = 120;
    public float expMultiplier = 1.18f;

    [Header("Hybrid (Exp + Linear)")] 
    public int hybridBaseValue = 120;
    public float hybridMultiplier = 1.18f;
    public int hybridLinearStep = 25;

    public int GetMaxHp(int vit) => baseHp + vit * hpPerVit;
    public float GetDamageMultiplier(int str) => 1f + str * damagePerStr;
    public float GetAttackSpeedMultiplier(int agi) => 1f + agi * attackSpeedPerAgi;

    public int GetXpToNextLevel(int level)
    {
        switch (xpFormula)
        {
            case XpFormulaType.Linear:
                return linearBase + linearStep * level;
            
            case XpFormulaType.Quadratic:
                return Mathf.CeilToInt(quadBase + quadA * level * level + quadB * level);
            
            case XpFormulaType.Exponential:
                return Mathf.CeilToInt(expBaseValue * Mathf.Pow(expMultiplier, level - 1));
            
            default:
                return Mathf.CeilToInt(hybridBaseValue * Mathf.Pow(hybridMultiplier, level - 1) + hybridLinearStep * level);
        }
    }
}