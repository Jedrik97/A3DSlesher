using UnityEngine;
using UnityEngine.AI;
using System;
using ModestTree;

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
        _agent.speed = chaseSpeed;
        _agent.updateRotation = false;

        OnDeath += HandleDeath;

        _state = EnemyState.Chasing;
        _nextAttackTime = 0f;
        _movementLocked = false;
    }

    private void OnDisable()
    {
        OnDeath -= HandleDeath;
    }
    

    public void Anim_LockMovement()
    {
        _movementLocked = true;
        _rotationLocked = true;
        if (_agent)
            _agent.isStopped = true;
    }

    public void Anim_UnlockMovement()
    {
        _movementLocked = false;
        _rotationLocked = false;
    }

    
    public void Anim_EnableWeapon() => weapon?.EnableCollider();
    public void Anim_DisableWeapon() => weapon?.DisableCollider();

    // ============================

    private void Update()
    {
        if (_state == EnemyState.Dead || !player)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (_state)
        {
            case EnemyState.Chasing:
            {
                // 🔹 ещё далеко — подходим
                if (dist > attackRange)
                {
                    if (!_movementLocked)
                    {
                        _agent.isStopped = false;
                        _agent.SetDestination(player.position);
                    }

                    RotateTowardsPlayer();
                    return;
                }

                // 🔥 дошли — СТОП и ПЕРЕХОД В АТАКУ
                _agent.isStopped = true;
                _agent.ResetPath();
                _state = EnemyState.Attacking;
                return;
            }

            case EnemyState.Attacking:
            {
                // ❌ В ЭТОМ СТЕЙТЕ МЫ ВООБЩЕ НЕ ДВИГАЕМСЯ
                _agent.isStopped = true;

                RotateTowardsPlayer();

                // 🔹 игрок убежал — выходим из атаки
                if (dist > attackRange)
                {
                    animatorController.CancelAttack();
                    _state = EnemyState.Chasing;
                    return;
                }

                // 🔥 игрок в радиусе — просто бьём по кулдауну
                if (!_movementLocked && Time.time >= _nextAttackTime)
                {
                    PerformAttack();
                }

                return;
            }
        }
    }


    private void PerformAttack()
    {
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


    private void HandleDeath(GameObject obj)
    {
        _state = EnemyState.Dead;

        if (_agent)
            _agent.enabled = false;

        foreach (var c in GetComponents<Collider>())
            c.enabled = false;
    }
}
