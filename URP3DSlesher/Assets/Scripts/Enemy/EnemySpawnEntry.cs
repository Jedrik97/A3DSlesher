using UnityEngine;

[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyStatsConfig stats;
    public GameObject prefab;
    public int count = 5;
    public float interval = 0.5f;
}