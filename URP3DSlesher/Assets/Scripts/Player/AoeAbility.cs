using System.Collections;
using UnityEngine;

public class AoeAbility : MonoBehaviour
{
    [Header("AOE Settings")]
    [SerializeField] private float aoeRadius = 3f;
    [SerializeField] private int aoeDamagePerSecond = 10;
    [SerializeField] private float aoeDuration = 3f;
    [SerializeField] private float aoeCooldown = 6f;
    [SerializeField] private GameObject magicEffect;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Animator animator;

    private bool isAoeActive;
    private float lastCastTime = -999f;

    public void OnAoeButton()
    {
        if (isAoeActive) return;
        if (Time.time < lastCastTime + aoeCooldown) return;
        StartCoroutine(AoeRoutine());
    }

    private IEnumerator AoeRoutine()
    {
        isAoeActive = true;
        lastCastTime = Time.time;

        if (animator) animator.SetTrigger("AoeAttack");
        if (magicEffect) magicEffect.SetActive(true);

        float elapsed = 0f;
        while (elapsed < aoeDuration)
        {
            PerformAoeDamage();
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }

        if (magicEffect) magicEffect.SetActive(false);
        isAoeActive = false;
    }

    private void PerformAoeDamage()
    {
        var hits = Physics.OverlapSphere(transform.position, aoeRadius, enemyLayer, QueryTriggerInteraction.Collide);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.TakeDamage(aoeDamagePerSecond);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}