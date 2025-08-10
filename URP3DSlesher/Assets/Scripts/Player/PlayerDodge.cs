using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeCooldown = 1f;

    private float lastDodgeTime;

    public void DodgeLeft()
    {
        TryDodge(-transform.right);
    }

    public void DodgeRight()
    {
        TryDodge(transform.right);
    }

    public void DodgeBack()
    {
        TryDodge(-transform.forward);
    }

    private void TryDodge(Vector3 direction)
    {
        if (Time.time - lastDodgeTime < dodgeCooldown) return;

        controller.Move(direction.normalized * dodgeDistance);
        lastDodgeTime = Time.time;
    }
}
