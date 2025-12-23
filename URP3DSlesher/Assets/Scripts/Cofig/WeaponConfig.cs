using UnityEngine;

public enum WeaponType { Melee, Ranged, Magic }

[CreateAssetMenu(menuName = "Configs/WeaponConfig", fileName = "WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public WeaponType weaponType = WeaponType.Melee;
    public float damage = 15f;
    public float attackRange = 1.6f;
    public float attackCooldown = 1.2f;
    
    public GameObject hitboxPrefab; // optional: prefab with EnemyWeaponHitbox
    public Vector3 hitboxLocalPosition = Vector3.zero;
    
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float projectileLifetime = 3.5f;
    public float projectileSpawnDelay = 0.15f;
    
}