using UnityEngine;
using System;
using System.Collections;

public class FieldOfView : MonoBehaviour
{
    private Transform player = null;
    private Coroutine fovCheckCoroutine = null;

    [Header("Field of View")] 
    [SerializeField] private float viewRadius = 15f;

    [SerializeField, Range(0, 360)] private float viewAngle = 180f;

    [Header("Eye Heights")] 
    [SerializeField] private float eyeHeight = 1.6f;

    [SerializeField] private float playerEyeHeight = 1.0f;

    [Header("Check Interval")]
    [SerializeField] private float checkInterval = 0.5f;

    [Header("Layers")] 
    [SerializeField] private LayerMask obstacleMask;

    public event Action<bool> OnPlayerVisibilityChanged;
    public Transform Player => player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;

            if (fovCheckCoroutine == null)
                fovCheckCoroutine = StartCoroutine(CheckFOVRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fovCheckCoroutine != null)
            {
                StopCoroutine(fovCheckCoroutine);
                fovCheckCoroutine = null;
            }

            player = null;
            OnPlayerVisibilityChanged?.Invoke(false);
        }
    }

    private IEnumerator CheckFOVRoutine()
    {
        while (player)
        {
            if (CheckPlayerInFOV())
            {
                OnPlayerVisibilityChanged?.Invoke(true);

                yield break;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private bool CheckPlayerInFOV()
    {
        if (!player)
            return false;

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * playerEyeHeight;

        Vector3 dir = (target - origin);
        float distance = dir.magnitude;
        Vector3 dirNormalized = dir / distance;

        if (distance > viewRadius)
            return false;

        Vector3 dirFlat = new Vector3(dirNormalized.x, 0f, dirNormalized.z).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, dirFlat);
        if (angleToPlayer > viewAngle / 2f)
            return false;

        if (Physics.Raycast(origin, dirNormalized, out RaycastHit hit, distance, obstacleMask))
        {
            return false;
        }

        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2f, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2f, false);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + leftBoundary * viewRadius);
        Gizmos.DrawLine(origin, origin + rightBoundary * viewRadius);

        Gizmos.color = Color.cyan;
        int segments = 50;
        float angleStep = viewAngle / segments;
        Vector3 previousPoint = origin + DirFromAngle(-viewAngle / 2f, false) * viewRadius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -viewAngle / 2f + angleStep * i;
            Vector3 nextPoint = origin + DirFromAngle(angle, false) * viewRadius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal) angleInDegrees += transform.eulerAngles.y;
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
#endif
}