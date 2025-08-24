using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _poolQueue;
    private List<T> _prefabs;
    private Transform _parent;


    private List<T> _activeList;

    public int InactiveCount => _poolQueue.Count;

    public ObjectPool(List<T> prefabs, int initialSizePerPrefab, Transform parent = null)
    {
        _prefabs = prefabs;
        _parent = parent;
        _poolQueue = new Queue<T>();
        _activeList = new List<T>();

        Shuffle(_prefabs);
        foreach (var prefab in _prefabs)
        {
            for (int i = 0; i < initialSizePerPrefab; i++)
            {
                T obj = Object.Instantiate(prefab, _parent);
                var nav = obj.GetComponent<NavMeshAgent>();
                if (nav != null) nav.enabled = false;

                obj.gameObject.SetActive(false);
                _poolQueue.Enqueue(obj);
            }
        }
    }

    public T Get()
    {
        T obj = _poolQueue.Count > 0
            ? _poolQueue.Dequeue()
            : Object.Instantiate(_prefabs[Random.Range(0, _prefabs.Count)], _parent);

        obj.gameObject.SetActive(true);

        if (!_activeList.Contains(obj))
            _activeList.Add(obj);

        return obj;
    }

    public void ReturnToPool(T obj)
    {
        var nav = obj.GetComponent<NavMeshAgent>();
        if (nav != null)
            nav.enabled = false;

        obj.gameObject.SetActive(false);

        _activeList.Remove(obj);

        _poolQueue.Enqueue(obj);
    }

    private void Shuffle(IList<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public T[] GetActiveObjects()
    {
        return _activeList.ToArray();
    }
}