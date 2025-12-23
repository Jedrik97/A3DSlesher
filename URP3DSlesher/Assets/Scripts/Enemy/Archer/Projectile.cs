using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;

    private Transform target;
    private IDamageReceiver receiver;
    private float damage;
    private object source;

    private float dieAt;
    private bool active;

    private ProjectilePool pool;

    private void Awake()
    {
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    public void BindPool(ProjectilePool projectilePool)
    {
        pool = projectilePool;
    }

    public void Initialize(Transform targetTransform, IDamageReceiver damageReceiver, float dmg, float projectileSpeed, float ttl, object src)
    {
        target = targetTransform;
        receiver = damageReceiver;
        damage = dmg;
        speed = projectileSpeed;
        lifetime = ttl;
        source = src;

        dieAt = Time.time + Mathf.Max(0.05f, lifetime);
        active = true;
    }

    private void OnEnable()
    {
        active = false;
    }

    private void Update()
    {
        if (!active) return;

        if (Time.time >= dieAt)
        {
            Release();
            return;
        }

        Vector3 aimPoint = target ? target.position : (transform.position + transform.forward);
        Vector3 dir = aimPoint - transform.position;
        float dist = dir.magnitude;
        if (dist < 0.001f) return;

        transform.position += dir.normalized * speed * Time.deltaTime;

        Vector3 look = dir;
        look.y = 0;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (!target) return;

        if (other.transform == target)
        {
            receiver?.ReceiveDamage(damage, source);
            Release();
        }
    }

    public void Arm()
    {
        active = true;
    }

    private void Release()
    {
        active = false;
        target = null;
        receiver = null;
        source = null;

        if (pool) pool.Release(this);
        else gameObject.SetActive(false);
    }
}
