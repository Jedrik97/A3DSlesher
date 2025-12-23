using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int prewarmCount = 10;
        public bool allowExpand = true;
        public Transform container;
    }

    [SerializeField] private PoolEntry[] pools;

    private readonly Dictionary<int, Queue<GameObject>> map = new Dictionary<int, Queue<GameObject>>(32);
    private readonly Dictionary<int, PoolEntry> entryMap = new Dictionary<int, PoolEntry>(32);

    private void Awake()
    {
        if (pools == null) return;

        for (int i = 0; i < pools.Length; i++)
        {
            PoolEntry e = pools[i];
            if (e == null || !e.prefab) continue;

            int key = e.prefab.GetInstanceID();
            if (entryMap.ContainsKey(key)) continue;

            entryMap.Add(key, e);
            map.Add(key, new Queue<GameObject>(Mathf.Max(4, e.prewarmCount)));

            if (!e.container) e.container = transform;

            int count = Mathf.Max(0, e.prewarmCount);
            for (int k = 0; k < count; k++)
            {
                GameObject go = CreateNew(e, key);
                ReturnInternal(go, key);
            }
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!prefab) return null;

        int key = prefab.GetInstanceID();
        if (!entryMap.TryGetValue(key, out PoolEntry e)) return null;

        Queue<GameObject> q = map[key];
        GameObject go;

        if (q.Count > 0)
        {
            go = q.Dequeue();
        }
        else
        {
            if (!e.allowExpand) return null;
            go = CreateNew(e, key);
        }

        Transform t = go.transform;
        t.SetParent(null, false);
        t.SetPositionAndRotation(position, rotation);
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go, int prefabKey)
    {
        if (!go) return;
        ReturnInternal(go, prefabKey);
    }

    private GameObject CreateNew(PoolEntry e, int key)
    {
        GameObject go = Instantiate(e.prefab, e.container);
        go.SetActive(false);

        EnemyPoolMember m = go.GetComponent<EnemyPoolMember>();
        if (!m) m = go.AddComponent<EnemyPoolMember>();
        m.Bind(this, key);

        return go;
    }

    private void ReturnInternal(GameObject go, int key)
    {
        if (!entryMap.TryGetValue(key, out PoolEntry e))
        {
            go.SetActive(false);
            return;
        }

        Transform t = go.transform;
        t.SetParent(e.container ? e.container : transform, false);
        go.SetActive(false);

        if (!map.TryGetValue(key, out Queue<GameObject> q))
        {
            q = new Queue<GameObject>(16);
            map.Add(key, q);
        }

        q.Enqueue(go);
    }
}
