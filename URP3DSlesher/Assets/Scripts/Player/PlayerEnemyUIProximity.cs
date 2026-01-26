using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyUIProximity : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float radius = 10f;

    private readonly HashSet<EnemyUIController> _nearby = new HashSet<EnemyUIController>();
    private SphereCollider _sphere;

    private void Awake()
    {
        _sphere = GetComponent<SphereCollider>();
        if (_sphere)
        {
            _sphere.isTrigger = true;
            _sphere.radius = radius;
        }
    }

    private void OnValidate()
    {
        if (radius < 0.1f)
            radius = 0.1f;

        if (!_sphere)
            _sphere = GetComponent<SphereCollider>();

        if (_sphere)
        {
            _sphere.isTrigger = true;
            _sphere.radius = radius;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var ui = other.GetComponentInParent<EnemyUIController>();
        if (!ui)
            return;

        if (_nearby.Add(ui))
            ui.SetVisible(true);
    }

    private void OnTriggerExit(Collider other)
    {
        var ui = other.GetComponentInParent<EnemyUIController>();
        if (!ui)
            return;

        if (_nearby.Remove(ui))
            ui.SetVisible(false);
    }

    private void OnDisable()
    {
        foreach (var ui in _nearby)
        {
            if (ui)
                ui.SetVisible(false);
        }
        _nearby.Clear();
    }
}