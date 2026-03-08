using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject _door;
    [SerializeField] private bool _isStartingRoom = false;
    [SerializeField] private Room _next;

    public int EnemiesAlive { get; internal set; }
    private bool _hasStarted = false;

    private void Awake()
    {
        foreach (WizardController wizard in GetComponentsInChildren<WizardController>())
        {
            EnemiesAlive++;
            wizard.enabled = false;
        }

        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                child.gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
            }
        }

        if (_isStartingRoom)
        {
            SpawnEnemies();
        }
    }

    private void Update()
    {
        if (_hasStarted && EnemiesAlive <= 0)
        {
            MarkCompleted();
        }
    }

    private void SpawnEnemies()
    {
        foreach (WizardController wizard in GetComponentsInChildren<WizardController>())
        {
            wizard.enabled = true;
        }

        _hasStarted = true;
    }

    private void MarkCompleted()
    {
        if (_door) Destroy(_door);
        if (_next) _next.SpawnEnemies();
    }
}