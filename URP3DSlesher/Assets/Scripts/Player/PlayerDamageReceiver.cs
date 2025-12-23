using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour, IDamageReceiver
{
    [SerializeField] private HealthPlayerController health;

    public void ReceiveDamage(float amount, object source)
    {
        if (health) health.TakeDamage(amount);
    }
}