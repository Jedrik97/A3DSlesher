using UnityEngine;

public interface IEnemyInitializable
{
    void Initialize(EnemyStatsConfig config, Transform player, MonoBehaviour playerDamageReceiver, ProjectilePool sharedProjectilePool);
    Transform Transform { get; }
}