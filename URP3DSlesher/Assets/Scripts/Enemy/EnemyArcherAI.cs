/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcherAI : EnemyBase
{
    [Header("Components & Prefabs")] [SerializeField]
    private FieldOfView fieldOfView;

    [SerializeField] private EnemyPathFollower pathFollower;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private int initialArrowCount = 10;

    [Header("Movement & Detection")] [SerializeField]
    private float chaseSpeed = 2.5f;

    [SerializeField] private float maxChaseDistance = 30f;
    [SerializeField] private float attackRange = 15f;

    private ObjectPool<Arrow> arrowPool;
    private NavMeshAgent agent;
    private Transform player;

    private bool hasSeenPlayer;
    private Vector3 chaseStartPoint;

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
        pathFollower = GetComponent<EnemyPathFollower>();
        arrowPool = new ObjectPool<Arrow>(new List<Arrow> { arrowPrefab }, initialArrowCount, transform);

        if (fieldOfView != null)
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;

        OnHealthChanged += HandleDamageInterrupt;
        OnDeath += HandleDeath;

        pathFollower?.ResumePatrol();
        currentState = EnemyState.Patrolling;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
    }

    private void OnDisable()
    {
        if (fieldOfView != null)
            fieldOfView.OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;

        OnHealthChanged -= HandleDamageInterrupt;
        OnDeath -= HandleDeath;
    }

    private void HandlePlayerVisibilityChanged(bool isVisible)
    {
        if (currentState == EnemyState.Dead)
            return;

        if (isVisible)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
                chaseStartPoint = transform.position;
            }

            player = fieldOfView.Player;
            currentState = EnemyState.Chasing;
        }
        else
        {
            hasSeenPlayer = false;
            currentState = EnemyState.Returning;
            agent.isStopped = false;
        }
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead)
            return;

        UpdateWalkingAnimation();

        if (player == null && fieldOfView)
            player = fieldOfView.Player;
        if (player == null)
            return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distFromStart = Vector3.Distance(transform.position, chaseStartPoint);

        switch (currentState)
        {
            case EnemyState.Patrolling:

                if (distToPlayer <= maxChaseDistance)
                {
                    currentState = EnemyState.Chasing;
                }

                break;

            case EnemyState.Chasing:
                if (distFromStart > maxChaseDistance)
                {
                    currentState = EnemyState.Returning;
                    agent.isStopped = false;
                    agent.SetDestination(chaseStartPoint);
                }
                else if (distToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                    animator.SetBool("IsWalking", false);
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                    RotateTowardsPlayer();
                }

                break;

            case EnemyState.Attacking:
                if (distToPlayer <= attackRange)
                {
                    animator.SetBool("IsAttacking", true);
                    agent.isStopped = true;
                    RotateTowardsPlayer();
                }
                else if (distToPlayer > attackRange)
                {
                    animator.SetBool("IsAttacking", false);
                    currentState = EnemyState.Chasing;
                }

                break;

            case EnemyState.Returning:
                hasSeenPlayer = false;
                agent.isStopped = false;
                agent.SetDestination(chaseStartPoint);
                if (distFromStart <= 1f)
                {
                    agent.isStopped = true;
                    currentState = EnemyState.Patrolling;
                    ReturnHeal();

                    pathFollower?.ResumePatrol();
                }

                break;
        }
    }

    private void UpdateWalkingAnimation()
    {
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
    }

    private void Shoot()
    {
        if (currentState == EnemyState.Dead || player == null)
            return;

        Vector3 spawnPos = arrowSpawnPoint.position;
        Arrow arrow = arrowPool.Get();
        arrow.transform.position = spawnPos;

        Vector3 targetPos = player.position + Vector3.up;
        Vector3 lookDir = (targetPos - spawnPos).normalized;

        arrow.transform.rotation = Quaternion.LookRotation(lookDir);
        arrow.Initialize(lookDir, arrowPool);
    }

    private void RotateTowardsPlayer()
    {
        if (player == null)
            return;
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 5f
        );
    }

    private void HandleDamageInterrupt(float newHealth)
    {
        if (currentState == EnemyState.Attacking)
        {
            currentState = EnemyState.Chasing;
            animator.SetBool("IsAttacking", false);
            agent.isStopped = false;
        }
    }

    private void HandleDeath(GameObject obj)
    {
        currentState = EnemyState.Dead;
        agent.updateRotation = false;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);

        Collider[] cols = GetComponents<Collider>();

        foreach (var col in cols)
        {
            col.enabled = false;
        }

        agent.enabled = false;
    }
}*/