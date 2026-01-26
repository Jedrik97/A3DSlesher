using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAI : EnemyMain
{
    [Header("Movement & Detection")]
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float attackRange = 3f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.2f;

    [Header("Refs")]
    [SerializeField] private Transform player;

    public event System.Action OnAttackAnim;

    private NavMeshAgent agent;
    private float _nextAttackTime;

    private enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
        Returning,
        Dead
    }

    private EnemyState currentState = EnemyState.Patrolling;

    protected override void OnEnable()
    {
        base.OnEnable();

        agent = GetComponent<NavMeshAgent>();

        OnHealthChanged += HandleDamageInterrupt;
        OnDeath += HandleDeath;

        currentState = EnemyState.Patrolling;
        _nextAttackTime = 0f;
    }

    private void OnDisable()
    {
        OnHealthChanged -= HandleDamageInterrupt;
        OnDeath -= HandleDeath;
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead)
            return;

        if (!player)
            return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Chasing:
            {
                if (distToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                    agent.isStopped = true;
                    break;
                }

                agent.isStopped = false;
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                RotateTowardsPlayer();
                break;
            }

            case EnemyState.Attacking:
            {
                if (distToPlayer > attackRange)
                {
                    currentState = EnemyState.Chasing;
                    agent.isStopped = false;
                    break;
                }

                agent.isStopped = true;
                RotateTowardsPlayer();

                if (Time.time >= _nextAttackTime)
                {
                    _nextAttackTime = Time.time + attackCooldown;
                    OnAttackAnim?.Invoke();
                }

                break;
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 5f
        );
    }

    private void HandleDamageInterrupt(float newHealth)
    {
        if (currentState != EnemyState.Attacking)
            return;

        currentState = EnemyState.Chasing;
        agent.isStopped = false;
    }

    private void HandleDeath(GameObject obj)
    {
        currentState = EnemyState.Dead;

        if (agent)
            agent.enabled = false;

        foreach (var col in GetComponents<Collider>())
            col.enabled = false;
    }
}
