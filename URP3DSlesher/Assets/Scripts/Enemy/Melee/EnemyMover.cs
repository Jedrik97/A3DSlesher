using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 0.9f;

    public void Setup(Transform player, float speed)
    {
        target = player;
        moveSpeed = speed;
    }

    private void Update()
    {
        if (!target) return;
        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude;
        if (dist > stopDistance)
        {
            Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;
            Vector3 look = dir;
            look.y = 0;
            if (look.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 10f * Time.deltaTime);
        }
    }
}