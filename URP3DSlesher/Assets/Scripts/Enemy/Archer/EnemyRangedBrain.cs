/*using UnityEngine;

public class EnemyRangedBrain : MonoBehaviour
{
    [SerializeField] private EnemyArcherBase owner;
    [SerializeField] private WeaponConfig weapon;

    [SerializeField] private float minDistanceFactor = 0.55f;
    [SerializeField] private float maxDistanceFactor = 1.0f;

    private void Awake()
    {
        if (!owner) owner = GetComponent<EnemyArcherBase>();
    }

    public void Setup(EnemyArcherBase enemyBase, WeaponConfig cfg)
    {
        owner = enemyBase;
        weapon = cfg;

        if (owner && owner.Mover && weapon)
        {
            float maxD = Mathf.Max(0.5f, weapon.attackRange * maxDistanceFactor);
            float minD = Mathf.Max(0.3f, maxD * minDistanceFactor);
            owner.Mover.SetDistanceBand(minD, maxD);
        }
    }

    private void Update()
    {
        if (!owner) return;
        if (!weapon) return;

        Transform player = owner.PlayerTransform;
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= weapon.attackRange)
        {
            if (owner.Combat) owner.Combat.RequestShot();
        }
    }
}*/