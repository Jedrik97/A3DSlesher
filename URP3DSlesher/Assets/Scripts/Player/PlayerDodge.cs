using UnityEngine;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeDuration = 0.25f; // время рывка
    [SerializeField] private float dodgeCooldown = 1f;

    private float lastDodgeTime;
    private bool isDodging;

    public void DodgeLeft() => TryDodge(-transform.right);
    public void DodgeRight() => TryDodge(transform.right);
    public void DodgeBack() => TryDodge(-transform.forward);

    private void TryDodge(Vector3 direction)
    {
        if (isDodging) return;
        if (Time.time - lastDodgeTime < dodgeCooldown) return;

        StartCoroutine(DodgeRoutine(direction));
    }

    private IEnumerator DodgeRoutine(Vector3 direction)
    {
        isDodging = true;
        lastDodgeTime = Time.time;

        float elapsed = 0f;
        Vector3 dodgeVelocity = direction.normalized * (dodgeDistance / dodgeDuration);

        while (elapsed < dodgeDuration)
        {
            controller.Move(dodgeVelocity * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }
}