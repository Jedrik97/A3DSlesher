using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Speed")]
    [SerializeField] private float maxMoveSpeed = 2.5f;
    [SerializeField] private float speedSmoothing = 12f;

    private EnemyMain enemy;
    private EnemyMeleeAI meleeAI;
    private NavMeshAgent agent;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private float _speed01;

    private void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        enemy = GetComponent<EnemyMain>();
        meleeAI = GetComponent<EnemyMeleeAI>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        if (enemy)
        {
            enemy.OnTakeDamageAnim += PlayHit;
            enemy.OnDeathAnim += PlayDeath;
        }

        if (meleeAI)
            meleeAI.OnAttackAnim += PlayAttack;

        if (animator)
        {
            animator.SetBool(IsDeadHash, false);
            _speed01 = 0f;
            animator.SetFloat(SpeedHash, 0f);
        }
    }

    private void OnDisable()
    {
        if (enemy)
        {
            enemy.OnTakeDamageAnim -= PlayHit;
            enemy.OnDeathAnim -= PlayDeath;
        }

        if (meleeAI)
            meleeAI.OnAttackAnim -= PlayAttack;
    }

    private void Update()
    {
        if (!animator)
            return;

        float target01 = 0f;

        if (agent && agent.enabled)
        {
            float v = agent.velocity.magnitude;
            float denom = maxMoveSpeed > 0.01f ? maxMoveSpeed : 0.01f;
            target01 = Mathf.Clamp01(v / denom);
        }

        _speed01 = Mathf.Lerp(_speed01, target01, Time.deltaTime * speedSmoothing);
        animator.SetFloat(SpeedHash, _speed01);
    }

    private void PlayAttack()
    {
        if (!animator)
            return;

        if (animator.GetBool(IsDeadHash))
            return;

        animator.SetTrigger(AttackHash);
    }

    private void PlayHit()
    {
        if (!animator)
            return;

        if (animator.GetBool(IsDeadHash))
            return;

        animator.ResetTrigger(HitHash);
        animator.SetTrigger(HitHash);
    }

    private void PlayDeath()
    {
        if (!animator)
            return;

        animator.SetBool(IsDeadHash, true);
        animator.SetTrigger(DieHash);
    }
}
