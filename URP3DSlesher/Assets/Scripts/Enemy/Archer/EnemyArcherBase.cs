/*using UnityEngine;

public class EnemyArcherBase : MonoBehaviour
{
    [SerializeField] private EnemyStatsConfig statsConfig;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private MonoBehaviour playerDamageReceiver;

    [field: SerializeField] public EnemyHealth Health { get; private set; }
    [field: SerializeField] public EnemyRangedMover Mover { get; private set; }
    [field: SerializeField] public EnemyRangedBrain Brain { get; private set; }
    [field: SerializeField] public EnemyRangedCombat Combat { get; private set; }

    public EnemyStatsConfig StatsConfig => statsConfig;
    public Transform PlayerTransform => playerTransform;
    public IDamageReceiver PlayerDamageReceiver => playerDamageReceiver as IDamageReceiver;

    private void Reset()
    {
        Health = GetComponent<EnemyHealth>();
        Mover = GetComponent<EnemyRangedMover>();
        Brain = GetComponent<EnemyRangedBrain>();
        Combat = GetComponent<EnemyRangedCombat>();
    }

    public void Initialize(EnemyStatsConfig config, Transform player, MonoBehaviour playerDamage)
    {
        statsConfig = config;
        playerTransform = player;
        playerDamageReceiver = playerDamage;

        if (Health) Health.Setup(statsConfig.maxHP);
        if (Mover) Mover.Setup(playerTransform, statsConfig.moveSpeed);
        if (Brain) Brain.Setup(this, statsConfig.weaponConfig);
        if (Combat) Combat.Setup(this, statsConfig.weaponConfig);
    }
}*/