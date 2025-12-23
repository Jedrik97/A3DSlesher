using UnityEngine;
using System;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyStatsConfig statsConfig;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private MonoBehaviour playerDamageReceiver;

    [field: SerializeField] public EnemyHealth Health { get; private set; }
    [field: SerializeField] public EnemyMover Mover { get; private set; }
    [field: SerializeField] public EnemyMeleeBrain Brain { get; private set; }
    [field: SerializeField] public EnemyMeleeCombat Combat { get; private set; }

    public EnemyStatsConfig StatsConfig => statsConfig;
    public Transform PlayerTransform => playerTransform;
    public Component PlayerDamageComponent => playerDamageReceiver as Component;

    private void Reset()
    {
        Health = GetComponent<EnemyHealth>();
        Mover = GetComponent<EnemyMover>();
        Brain = GetComponent<EnemyMeleeBrain>();
        Combat = GetComponent<EnemyMeleeCombat>();
    }

    public void Initialize(EnemyStatsConfig config, Transform player, Component playerDamageComp)
    {
        statsConfig = config;
        playerTransform = player;
        playerDamageReceiver = playerDamageComp as MonoBehaviour;

        if (Health) Health.Setup(statsConfig.maxHP);
        if (Mover) Mover.Setup(playerTransform, statsConfig.moveSpeed);
        if (Brain) Brain.Setup(this, statsConfig.weaponConfig);
        if (Combat) Combat.Setup(this, statsConfig.weaponConfig);
    }
}