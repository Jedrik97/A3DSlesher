/*
using System.Collections;
using UnityEngine;

public class AoeAbility : MonoBehaviour
{
    [Header("AOE Settings")] 
    [SerializeField] private float aoeRadius = 3f;
    [SerializeField] private int aoeDamagePerSecond = 10;
    [SerializeField] private float aoeDuration = 10f;
    [SerializeField] private float aoeCooldown = 6f;
    [SerializeField] private GameObject magicEffect;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Animator animator;

    /*[SerializeField] private PlayerStats playerStats;#1#

    private bool isAoeActive = false;

    void OnEnable()
    {
        PlayerCombat.OnAoeTriggered += StartAoeAttack;
    }

    void OnDisable()
    {
        PlayerCombat.OnAoeTriggered -= StartAoeAttack;
    }

    private void StartAoeAttack()
    {
        if (isAoeActive) return;
        StartCoroutine(AoeRoutine());
    }

    private IEnumerator AoeRoutine()
    {
        isAoeActive = true;

        if (animator != null)
        {
            animator.SetTrigger("AoeAttack");
        }

        if (magicEffect)
        {
            magicEffect.SetActive(true);
            StartCoroutine(DisableMagicEffectAfterDelay(aoeDuration));
        }

        float elapsedTime = 0f;
        /*UpdateAoeStats();#1#

        while (elapsedTime < aoeDuration)
        {
            PerformAoeDamage();
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        yield return new WaitForSeconds(aoeCooldown);
        isAoeActive = false;
    }

    private void PerformAoeDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, aoeRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            if (!enemy.CompareTag("Enemy") && !enemy.CompareTag("Boss")) 
                continue;

            if (enemy.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
            {
                enemyBase.TakeDamage(aoeDamagePerSecond);
            }
        }
    }

    /*private void UpdateAoeStats()
    {
        aoeDamagePerSecond = 10 + (int)(playerStats.intelligence * 2);
        aoeRadius = 3f + (playerStats.wisdom * 0.1f);
        aoeCooldown = Mathf.Max(0.1f, 6f - (playerStats.wisdom * 0.1f));
        aoeDuration = 10f + (playerStats.intelligence * 0.1f);
    }#1#

    private IEnumerator DisableMagicEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (magicEffect)
            magicEffect.SetActive(false);
    }
    
}
*/
