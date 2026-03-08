using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WizardController : MonoBehaviour
{
    [SerializeField] private EntityHealth _wizardHealth;
    [SerializeField] private ObjectPoolMonoBehaviour _pool;
    [SerializeField] private int _maxGhosts = 3;
    [SerializeField] private float _minSpawnInterval = 2.5f;
    [SerializeField] private float _maxSpawnInterval = 4.5f;
    private readonly List<GhostController> _ghosts = new();

    private void Awake()
    {
        _wizardHealth.OnDeath += () =>
        {
            while (_ghosts.Count > 0)
            {
                RemoveGhost(_ghosts[0]);
            }

            Destroy(gameObject);
        };

        StartCoroutine(StartGhostSpawner());
    }

    private void OnDestroy()
    {
        GetComponentInParent<Room>().EnemiesAlive--;
    }

    private IEnumerator StartGhostSpawner()
    {
        yield return new WaitForSeconds(Random.Range(_minSpawnInterval, _maxSpawnInterval));
        if (_ghosts.Count < _maxGhosts)
        {
            SpawnGhost();
        }

        StartCoroutine(StartGhostSpawner());
    }

    private void SpawnGhost()
    {
        Vector3 position = transform.position + transform.forward * 0.75f;

        Debug.Assert(_pool);
        Debug.Assert(_pool.Pool != null);
        _pool.Pool.ActivateObject(obj =>
        {
            Debug.Assert(obj);
            obj.transform.position = position;

            GhostController ghost = obj.GetComponent<GhostController>();
            ghost.Wizard = this;

            _ghosts.Add(ghost);
        });
    }

    internal void RemoveGhost(GhostController ghostController)
    {
        Debug.Assert(ghostController.Wizard == this);
        ghostController.Wizard = null;
        _ghosts.Remove(ghostController);
        _pool.Pool.ReleaseObject(ghostController.gameObject);
    }
}