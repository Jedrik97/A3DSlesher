using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int prewarmCount = 32;
    [SerializeField] private bool allowExpand = true;
    [SerializeField] private Transform container;

    private readonly Queue<Projectile> pool = new Queue<Projectile>(128);

    public Projectile Prefab => projectilePrefab;

    private void Awake()
    {
        if (!container) container = transform;
        Prewarm();
    }

    private void Prewarm()
    {
        if (!projectilePrefab) return;
        for (int i = 0; i < prewarmCount; i++)
        {
            Projectile p = CreateNew();
            ReturnToPool(p);
        }
    }

    private Projectile CreateNew()
    {
        Projectile p = Instantiate(projectilePrefab, container);
        p.gameObject.SetActive(false);
        p.BindPool(this);
        return p;
    }

    public Projectile Get(Vector3 position, Quaternion rotation)
    {
        Projectile p;
        if (pool.Count > 0)
        {
            p = pool.Dequeue();
        }
        else
        {
            if (!allowExpand) return null;
            p = CreateNew();
        }

        Transform t = p.transform;
        t.SetParent(null, false);
        t.SetPositionAndRotation(position, rotation);
        p.gameObject.SetActive(true);
        return p;
    }

    public void Release(Projectile projectile)
    {
        if (!projectile) return;
        ReturnToPool(projectile);
    }

    private void ReturnToPool(Projectile projectile)
    {
        Transform t = projectile.transform;
        t.SetParent(container, false);
        projectile.gameObject.SetActive(false);
        pool.Enqueue(projectile);
    }
}