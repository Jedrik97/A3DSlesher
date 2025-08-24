/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossAI : EnemyBase
{
    [Header("Components & Prefabs")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private Fireball fireballPrefab;

    [Header("Object Pool Settings")]
    [SerializeField] private int initialFireballCount = 10;

    [Header("Movement & Detection")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float maxChaseDistance = 30f;
    [SerializeField] private float rangeTriggerDistance = 30f;
    [SerializeField] private float aoeDistance = 10f;
    [SerializeField] private float meleeTriggerDistance = 2f;

    [Header("Boss Attack Settings")]
    [SerializeField] private float meleeDuration = 10f;

    [Header("Teleportation")]
    [SerializeField] private Transform[] teleportPoints;

    [Header("Death Settings")]
    [SerializeField] private float deathDuration = 5f;

    [Header("Boss AOE Settings")]
    [SerializeField] private float aoeDamageRadius = 10f;
    [SerializeField] private float aoeDamageAmount = 20f;

    [Header("Weapons")]
    [SerializeField] private EnemyWeaponBoss leftWeapon;
    [SerializeField] private EnemyWeaponBoss rightWeapon;

    private ObjectPool<Fireball> fireballPool;
    private NavMeshAgent agent;
    private CharacterController controller;
    private Transform player;

    private bool hasSeenPlayer;
    private bool isPlayerVisible;
    private Vector3 chaseStartPoint;

    private enum BossState { Idle, Chasing, Ranged, AOE, Melee, Teleporting, Returning, Dead }
    private BossState currentState = BossState.Idle;
    
    private float stateTimer;

    // ===================================================================
    // Публичный метод для задания точек телепортации из GameManager
    public void SetTeleportPoints(Transform[] points)
    {
        teleportPoints = points;
    }
    // ===================================================================

    protected override void OnEnable()
    {
        base.OnEnable();
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        fireballPool = new ObjectPool<Fireball>(new List<Fireball> { fireballPrefab }, initialFireballCount, transform);

        if (fieldOfView != null)
            fieldOfView.OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;

        OnHealthChanged += HandleDamageInterrupt;
        OnDeath += HandleDeath;

        currentState = BossState.Idle;
    }

    private void OnDisable()
    {
        if (fieldOfView)
            fieldOfView.OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;

        OnHealthChanged -= HandleDamageInterrupt;
        OnDeath -= HandleDeath;
    }

    private void HandlePlayerVisibilityChanged(bool isVisible)
    {
        isPlayerVisible = isVisible;
        if (currentState == BossState.Dead) return;

        if (isVisible && !hasSeenPlayer)
        {
            hasSeenPlayer = true;
            chaseStartPoint = transform.position;
            player = fieldOfView.Player;
            currentState = BossState.Chasing;
        }
        else if (!isVisible)
        {
            hasSeenPlayer = false;
            currentState = BossState.Returning;
            agent.isStopped = false;
            agent.SetDestination(chaseStartPoint);
        }
    }

    private void Update()
    {
        if (currentState == BossState.Dead) return;

        UpdateWalkingAnimation();
        if (player == null && fieldOfView != null)
            player = fieldOfView.Player;
        if (player == null && currentState != BossState.Returning) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distFromStart = Vector3.Distance(transform.position, chaseStartPoint);

        switch (currentState)
        {
            case BossState.Idle:
                break;

            case BossState.Chasing:
                if (distFromStart > maxChaseDistance)
                {
                    currentState = BossState.Returning;
                    agent.isStopped = false;
                    agent.SetDestination(chaseStartPoint);
                }
                else if (distToPlayer <= rangeTriggerDistance && isPlayerVisible)
                {
                    currentState = BossState.Ranged;
                    agent.isStopped = true;
                    animator.SetTrigger("RangedAttack");
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                    RotateTowardsPlayer();
                }
                break;

            case BossState.Ranged:
                RotateTowardsPlayer();
                if (distToPlayer <= aoeDistance)
                {
                    currentState = BossState.AOE;
                    animator.SetTrigger("AOEAttack");
                }
                break;

            case BossState.AOE:
                RotateTowardsPlayer();
                break;

            case BossState.Melee:
                RotateTowardsPlayer();
                if (Time.time >= stateTimer)
                {
                    animator.SetTrigger("Teleport");
                    currentState = BossState.Teleporting;
                    break;
                }
                
                if (distToPlayer > meleeTriggerDistance)
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                    RotateTowardsPlayer();
                    animator.SetBool("IsWalking", true);
                }
                else
                {
                    agent.isStopped = true;
                    animator.SetBool("IsWalking", false);
                    animator.SetTrigger("MeleeAttack");
                }
                break;

            case BossState.Teleporting:
                RotateTowardsPlayer();
                break;

            case BossState.Returning:
                hasSeenPlayer = false;
                agent.isStopped = false;
                agent.SetDestination(chaseStartPoint);
                if (distFromStart <= 1f)
                {
                    currentState = BossState.Idle;
                    agent.isStopped = true;
                    ReturnHeal();
                }
                break;
        }
    }

    private void UpdateWalkingAnimation()
    {
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
    }

    public void ShootFireball()
    {
        if (currentState == BossState.Dead || player == null)
            return;

        Vector3 spawnPos = fireballSpawnPoint.position;
        Fireball fireball = fireballPool.Get();
        fireball.transform.position = spawnPos;

        Vector3 targetPos = player.position + Vector3.up;
        Vector3 lookDir = (targetPos - spawnPos).normalized;

        fireball.transform.rotation = Quaternion.LookRotation(lookDir);
        fireball.Initialize(lookDir, fireballPool);
    }

    public void OnAOEEnd()
    {
        PerformAOEDamage();
        currentState = BossState.Melee;
        stateTimer = Time.time + meleeDuration;
    }

    public void PerformAOEDamage()
    {
        if (!isPlayerVisible || player == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= aoeDamageRadius)
        {
            var health = player.GetComponent<HealthPlayerController>();
            if (health != null)
                health.TakeDamage(aoeDamageAmount);
        }
    }

    public void OnTeleport()
    {
        TeleportAway();
        currentState = BossState.Ranged;
        animator.SetTrigger("RangedAttack");
    }

    private void TeleportAway()
    {
        agent.isStopped = true;
        if (teleportPoints == null || teleportPoints.Length == 0) return;
        int idx = Random.Range(0, teleportPoints.Length);
        controller.enabled = false;
        transform.position = teleportPoints[idx].position;
        controller.enabled = true;
        agent.Warp(transform.position);
    }

    private void HandleDamageInterrupt(float newHealth)
    {
        animator.SetTrigger("TakeDamage");
        if (currentState == BossState.Melee || currentState == BossState.AOE)
        {
            currentState = BossState.Chasing;
            agent.isStopped = false;
        }
    }

    private void HandleDeath(GameObject obj)
    {
        currentState = BossState.Dead;
        agent.isStopped = true;
        agent.updateRotation = false;
        animator.SetTrigger("Die");
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDuration);
        gameObject.SetActive(false);
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 5f);
    }

    public void EnableLeftWeapon() => leftWeapon?.EnableCollider();
    public void DisableLeftWeapon() => leftWeapon?.DisableCollider();
    public void EnableRightWeapon() => rightWeapon?.EnableCollider();
    public void DisableRightWeapon() => rightWeapon?.DisableCollider();
}
*/
