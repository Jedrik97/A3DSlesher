using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyMeleeAI : EnemyMain
{
    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float attackRange = 3f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int attackVariants = 3;

    [Header("Refs")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyWeapon weapon;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyAnimatorController animatorController;

    public event Action<int> OnAttackAnim;

    private NavMeshAgent _agent;
    private float _nextAttackTime;

    private bool _movementLocked;
    private bool _rotationLocked;

    private static readonly int AttackVariantHash =
        Animator.StringToHash("AttackVariant");

    private enum EnemyState
    {
        Chasing,
        Attacking,
        Dead
    }

    private EnemyState _state;

    protected override void OnEnable()
    {
        base.OnEnable();

        _agent = GetComponent<NavMeshAgent>();

        if (_agent)
        {
            _agent.speed = chaseSpeed;
            _agent.updateRotation = false;
            _agent.isStopped = false;
        }

        _state = EnemyState.Chasing;
        _nextAttackTime = 0f;
        _movementLocked = false;
        _rotationLocked = false;

        OnDeath += HandleDeath;
        OnHitAnim += HandleHitInterrupt;
    }

    private void OnDisable()
    {
        OnDeath -= HandleDeath;
        OnHitAnim -= HandleHitInterrupt;
    }

    public void Anim_LockMovement()
    {
        if (IsDead)
            return;

        _movementLocked = true;
        _rotationLocked = true;

        if (_agent && _agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }

    public void Anim_UnlockMovement()
    {
        if (IsDead)
            return;

        _movementLocked = false;
        _rotationLocked = false;
    }

    public void Anim_EnableWeapon()
    {
        if (IsDead)
            return;

        weapon?.EnableCollider();
    }

    public void Anim_DisableWeapon()
    {
        weapon?.DisableCollider();
    }

    private void Update()
    {
        if (IsDead || _state == EnemyState.Dead || !player)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (_state)
        {
            case EnemyState.Chasing:
            {
                if (dist > attackRange)
                {
                    if (!_movementLocked && _agent && _agent.isActiveAndEnabled)
                    {
                        _agent.isStopped = false;
                        _agent.SetDestination(player.position);
                    }

                    RotateTowardsPlayer();
                    return;
                }

                if (_agent && _agent.isActiveAndEnabled)
                {
                    _agent.isStopped = true;
                    _agent.ResetPath();
                }

                _state = EnemyState.Attacking;
                return;
            }

            case EnemyState.Attacking:
            {
                if (_agent && _agent.isActiveAndEnabled)
                    _agent.isStopped = true;

                RotateTowardsPlayer();

                if (dist > attackRange)
                {
                    animatorController.CancelAttack();
                    _state = EnemyState.Chasing;
                    return;
                }

                if (!_movementLocked && Time.time >= _nextAttackTime)
                    PerformAttack();

                return;
            }
        }
    }

    private void PerformAttack()
    {
        if (IsDead || _state == EnemyState.Dead)
            return;

        _nextAttackTime = Time.time + attackCooldown;

        int variant = UnityEngine.Random.Range(0, attackVariants);
        animator.SetInteger(AttackVariantHash, variant);

        OnAttackAnim?.Invoke(variant);
    }

    private void RotateTowardsPlayer()
    {
        if (_rotationLocked)
            return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 6f
        );
    }

    private void HandleHitInterrupt(int variant)
    {
        if (IsDead || _state == EnemyState.Dead)
            return;

        animatorController.CancelAttack();

        _state = EnemyState.Chasing;
        _movementLocked = false;
        _rotationLocked = false;
        _nextAttackTime = Time.time + 0.1f;

        if (_agent && _agent.isActiveAndEnabled)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }

    private void HandleDeath(GameObject obj)
    {
        _state = EnemyState.Dead;

        _movementLocked = true;
        _rotationLocked = true;

        if (_agent && _agent.isActiveAndEnabled)
            _agent.enabled = false;

        weapon?.DisableCollider();

        enabled = false;
    }
}
