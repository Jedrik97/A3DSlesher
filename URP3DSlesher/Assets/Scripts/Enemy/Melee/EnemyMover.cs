using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.2f;

    public void Setup(Transform player, float speed, float desiredStopDistance)
    {
        target = player;
        moveSpeed = speed;
        stopDistance = Mathf.Max(0.2f, desiredStopDistance * 0.9f);
    }

    private void Update()
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude;

        if (dist > stopDistance)
        {
            transform.position += dir.normalized * moveSpeed * Time.deltaTime;

            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }
}