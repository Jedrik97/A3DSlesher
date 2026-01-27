using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private EnemyMain enemy;
    private EnemyMeleeAI meleeAI;
    private NavMeshAgent agent;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int CancelAttackHash = Animator.StringToHash("CancelAttack");

    private static readonly int AttackVariantHash = Animator.StringToHash("AttackVariant");
    private static readonly int HitVariantHash = Animator.StringToHash("HitVariant");
    private static readonly int DeathVariantHash = Animator.StringToHash("DeathVariant");

    private void Awake()
    {
        enemy = GetComponent<EnemyMain>();
        meleeAI = GetComponent<EnemyMeleeAI>();
        agent = GetComponent<NavMeshAgent>();

        if (!animator)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        enemy.OnHitAnim += PlayHit;
        enemy.OnDeathAnim += PlayDeath;

        if (meleeAI)
            meleeAI.OnAttackAnim += PlayAttack;
    }

    private void OnDisable()
    {
        enemy.OnHitAnim -= PlayHit;
        enemy.OnDeathAnim -= PlayDeath;

        if (meleeAI)
            meleeAI.OnAttackAnim -= PlayAttack;
    }

    private void Update()
    {
        if (agent && agent.enabled && !enemy.IsDead)
            animator.SetFloat(SpeedHash, agent.velocity.magnitude);
    }

    public void CancelAttack()
    {
        animator.ResetTrigger(AttackHash);
        animator.SetTrigger(CancelAttackHash);
    }

    private void PlayAttack(int variant)
    {
        if (enemy.IsDead)
            return;

        animator.SetInteger(AttackVariantHash, variant);
        animator.ResetTrigger(CancelAttackHash);
        animator.SetTrigger(AttackHash);
    }

    private void PlayHit(int variant)
    {
        if (enemy.IsDead)
            return;

        animator.SetInteger(HitVariantHash, variant);
        animator.SetTrigger(HitHash);
    }

    private void PlayDeath(int variant)
    {
        animator.SetInteger(DeathVariantHash, variant);
        animator.SetTrigger(DieHash);
    }
    public void ForceCancelAttack()
    {
        animator.ResetTrigger(AttackHash);
        animator.ResetTrigger(CancelAttackHash);

        animator.SetBool(AttackHash, false);

        animator.speed = 0f;
        animator.Update(0f);
        animator.speed = 1f;
    }


}
