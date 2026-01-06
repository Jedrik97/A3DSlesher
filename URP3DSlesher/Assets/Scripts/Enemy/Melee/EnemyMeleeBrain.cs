using UnityEngine;

public class EnemyMeleeBrain : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;
    [SerializeField] private WeaponConfig weapon;
    [SerializeField] private float attackDistanceBuffer = 0.15f;

    private void Awake()
    {
        if (!owner) owner = GetComponent<EnemyBase>();
    }

    public void Setup(EnemyBase enemyBase, WeaponConfig cfg)
    {
        owner = enemyBase;
        weapon = cfg;
    }

    private void Update()
    {
        if (!owner) return;
        Transform player = owner.PlayerTransform;
        if (!player || !weapon) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= weapon.attackRange + attackDistanceBuffer)
            owner.Combat?.RequestAttack();
    }
}