using UnityEngine;
using Zenject;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float currentHP;
    [SerializeField] private float xpReward = 10f;

    private SignalBus _signalBus;

    public event Action<EnemyHealth> OnDied;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void Setup(float maxHP)
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        _signalBus.Fire(new EnemyDiedSignal(xpReward));
        OnDied?.Invoke(this);
        gameObject.SetActive(false);
    }
}