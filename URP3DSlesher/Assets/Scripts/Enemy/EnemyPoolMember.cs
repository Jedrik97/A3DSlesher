using UnityEngine;

public class EnemyPoolMember : MonoBehaviour
{
    private EnemyPoolManager manager;
    private int prefabKey;

    public int PrefabKey => prefabKey;

    public void Bind(EnemyPoolManager poolManager, int key)
    {
        manager = poolManager;
        prefabKey = key;
    }

    public void Release()
    {
        if (manager) manager.Release(gameObject, prefabKey);
        else gameObject.SetActive(false);
    }
}