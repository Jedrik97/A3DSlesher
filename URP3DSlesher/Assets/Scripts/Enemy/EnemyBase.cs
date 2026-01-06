using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private EnemyStatsConfig stats;

    [Header("Links")]
    [SerializeField] private EnemyHealth health;
    [SerializeField] private EnemyMover mover;
    [SerializeField] private EnemyMeleeBrain brain;
    [SerializeField] private EnemyMeleeCombat combat;

    public Transform PlayerTransform { get; private set; }
    public IDamageReceiver PlayerDamageReceiver { get; private set; }
    public EnemyMeleeCombat Combat => combat;
    public WeaponConfig Weapon => stats ? stats.weaponConfig : null;

    private void Awake()
    {
        if (!health) health = GetComponent<EnemyHealth>();
        if (!mover) mover = GetComponent<EnemyMover>();
        if (!brain) brain = GetComponent<EnemyMeleeBrain>();
        if (!combat) combat = GetComponent<EnemyMeleeCombat>();
    }

    public void Initialize(EnemyStatsConfig cfg, Transform playerTransform, IDamageReceiver playerDamageReceiver)
    {
        stats = cfg;
        PlayerTransform = playerTransform;
        PlayerDamageReceiver = playerDamageReceiver;

        if (health) health.Setup(this, cfg.maxHP);
        if (mover) mover.Setup(playerTransform, cfg.moveSpeed, cfg.weaponConfig ? cfg.weaponConfig.attackRange : 1.6f);
        if (brain) brain.Setup(this, cfg.weaponConfig);
        if (combat) combat.Setup(this, cfg.weaponConfig);
    }
}