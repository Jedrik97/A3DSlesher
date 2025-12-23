using UnityEngine;

[CreateAssetMenu(menuName = "Configs/EnemyStatsConfig", fileName = "EnemyStatsConfig")]
public class EnemyStatsConfig : ScriptableObject
{
    public float maxHP = 100f;
    public float moveSpeed = 3.5f;
    public int rewardGold = 5;
    public WeaponConfig weaponConfig;
}
