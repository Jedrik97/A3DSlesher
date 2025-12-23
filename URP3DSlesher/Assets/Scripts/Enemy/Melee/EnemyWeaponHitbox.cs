using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyWeaponHitbox : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;
    [SerializeField] private float damage = 10f;
    private Collider col;
    private bool active = true;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    public void Initialize(EnemyBase enemyBase, float dmg)
    {
        owner = enemyBase;
        damage = dmg;
        active = true;
        gameObject.SetActive(true);
    }

    public void DisableHit()
    {
        active = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (!owner) return;

        Component damageComp = owner.PlayerDamageComponent;
        if (damageComp && other.gameObject == owner.PlayerTransform.gameObject)
        {
            var method = damageComp.GetType().GetMethod("ReceiveDamage");
            if (method != null)
            {
                method.Invoke(damageComp, new object[] { damage, owner.gameObject });
                active = false;
            }
            else
            {
                var alt = damageComp.GetType().GetMethod("TakeDamage");
                if (alt != null)
                {
                    alt.Invoke(damageComp, new object[] { damage, owner.gameObject });
                    active = false;
                }
            }
        }
    }
}