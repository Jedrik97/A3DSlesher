using UnityEngine;

[CreateAssetMenu(menuName = "Configs/WaveConfig", fileName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public EnemySpawnEntry[] entries;
    public float startDelay = 0.5f;
    public float betweenEntriesDelay = 0.5f;
}