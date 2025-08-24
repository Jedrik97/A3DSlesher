/*using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 launchDirection;
    [SerializeField] private float speed = 10f;

    [SerializeField] private int damage = 10;

    private Vector3 startPosition;
    private ObjectPool<Arrow> pool;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction, ObjectPool<Arrow> pool)
    {
        this.pool = pool;
        launchDirection = direction;
        startPosition = transform.position;
        gameObject.SetActive(true);

        rb.AddForce(direction * speed, ForceMode.Impulse);
        StartCoroutine(WaitAndReturnToPool());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<HealthPlayerController>();
            if (health)
            {
                health.TakeDamage(damage);
            }

            ReturnToPool();
        }
    }

    private IEnumerator WaitAndReturnToPool()
    {
        yield return new WaitForSeconds(6f);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(false);
        pool.ReturnToPool(this);
    }
}*/