using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyWeaponHitbox : MonoBehaviour
{
    [SerializeField] private Collider col;

    private EnemyBase owner;
    private float damage;
    private bool active;
    private bool didHit;

    private void Awake()
    {
        if (!col) col = GetComponent<Collider>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public void Bind(EnemyBase enemyBase, float dmg)
    {
        owner = enemyBase;
        damage = dmg;
    }

    public void SetActive(bool value)
    {
        active = value;
        didHit = false;

        if (col) col.enabled = value;
        if (gameObject.activeSelf != value) gameObject.SetActive(value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active || didHit || !owner) return;

        Transform player = owner.PlayerTransform;
        if (!player || other.transform != player) return;

        IDamageReceiver receiver = owner.PlayerDamageReceiver;
        if (receiver == null) return;

        receiver.ReceiveDamage(damage, owner.gameObject);
        didHit = true;
    }
}