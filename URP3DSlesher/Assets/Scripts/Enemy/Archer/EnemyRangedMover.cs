using UnityEngine;

public class EnemyRangedMover : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 7f;

    [SerializeField] private float rotateSpeed = 12f;

    public void Setup(Transform player, float speed)
    {
        target = player;
        moveSpeed = speed;
    }

    public void SetDistanceBand(float min, float max)
    {
        minDistance = min;
        maxDistance = max;
    }

    private void Update()
    {
        if (!target) return;

        Vector3 to = target.position - transform.position;
        to.y = 0;

        float dist = to.magnitude;
        if (to.sqrMagnitude > 0.001f)
        {
            Quaternion q = Quaternion.LookRotation(to.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, rotateSpeed * Time.deltaTime);
        }

        if (dist > maxDistance)
        {
            transform.position += to.normalized * moveSpeed * Time.deltaTime;
            return;
        }

        if (dist < minDistance)
        {
            transform.position -= to.normalized * moveSpeed * Time.deltaTime;
            return;
        }
    }
}