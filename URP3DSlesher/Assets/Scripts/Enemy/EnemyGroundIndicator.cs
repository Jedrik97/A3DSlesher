using UnityEngine;

public class EnemyGroundIndicator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float offsetFromGround = 0.02f;

    private void LateUpdate()
    {
        Vector3 origin = transform.position + Vector3.up;
        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayDistance, groundMask))
            return;

        transform.position = hit.point + hit.normal * offsetFromGround;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
    }
}