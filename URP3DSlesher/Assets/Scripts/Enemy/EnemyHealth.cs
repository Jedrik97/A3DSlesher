using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float hp;

    private EnemyBase owner;

    public void Setup(EnemyBase enemyBase, float max)
    {
        owner = enemyBase;
        maxHp = Mathf.Max(1f, max);
        hp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        hp -= Mathf.Max(0f, amount);
        if (hp <= 0f) Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}