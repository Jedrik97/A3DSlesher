using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private MonoBehaviour playerDamageReceiver;
    [SerializeField] private ProjectilePool sharedProjectilePool;
    [SerializeField] private EnemyPoolManager enemyPool;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Waves")]
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] private bool loopWaves;

    [Header("Runtime")]
    [SerializeField] private int maxAlive = 50;

    private int alive;
    private int spIndex;
    private Coroutine routine;

    public void StartWaves()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(WavesRoutine());
    }

    public void StopWaves()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator WavesRoutine()
    {
        if (!playerTransform) yield break;
        if (!playerDamageReceiver) yield break;
        if (!enemyPool) yield break;
        if (spawnPoints == null || spawnPoints.Length == 0) yield break;
        if (waves == null || waves.Length == 0) yield break;

        int waveIndex = 0;

        while (true)
        {
            WaveConfig wave = waves[waveIndex];
            if (wave && wave.entries != null)
            {
                float sd = Mathf.Max(0f, wave.startDelay);
                if (sd > 0) yield return new WaitForSeconds(sd);

                for (int e = 0; e < wave.entries.Length; e++)
                {
                    EnemySpawnEntry entry = wave.entries[e];
                    if (entry != null && entry.prefab && entry.stats)
                    {
                        for (int i = 0; i < entry.count; i++)
                        {
                            while (alive >= maxAlive) yield return null;

                            SpawnOne(entry.prefab, entry.stats);

                            float itv = Mathf.Max(0f, entry.interval);
                            if (itv > 0) yield return new WaitForSeconds(itv);
                        }
                    }

                    float bed = Mathf.Max(0f, wave.betweenEntriesDelay);
                    if (bed > 0) yield return new WaitForSeconds(bed);
                }
            }

            waveIndex++;
            if (waveIndex >= waves.Length)
            {
                if (!loopWaves) break;
                waveIndex = 0;
            }

            yield return null;
        }

        routine = null;
    }

    private void SpawnOne(GameObject prefab, EnemyStatsConfig stats)
    {
        Transform sp = spawnPoints[spIndex];
        spIndex++;
        if (spIndex >= spawnPoints.Length) spIndex = 0;

        GameObject go = enemyPool.Get(prefab, sp.position, sp.rotation);
        if (!go) return;

        IEnemyInitializable init = go.GetComponent<IEnemyInitializable>();
        if (init == null)
        {
            EnemyPoolMember m0 = go.GetComponent<EnemyPoolMember>();
            if (m0) m0.Release();
            else go.SetActive(false);
            return;
        }

        init.Initialize(stats, playerTransform, playerDamageReceiver, sharedProjectilePool);

        EnemyHealth h = go.GetComponent<EnemyHealth>();
        EnemyPoolMember m = go.GetComponent<EnemyPoolMember>();

        if (h && m)
        {
            h.OnDied += OnEnemyDied;
            alive++;
        }
        else
        {
            if (m) m.Release();
            else go.SetActive(false);
        }
    }

    private void OnEnemyDied(EnemyHealth h)
    {
        if (!h) return;

        h.OnDied -= OnEnemyDied;

        EnemyPoolMember m = h.GetComponent<EnemyPoolMember>();
        if (m) m.Release();
        else h.gameObject.SetActive(false);

        alive--;
        if (alive < 0) alive = 0;
    }
}
