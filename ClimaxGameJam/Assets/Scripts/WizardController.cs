using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WizardController : MonoBehaviour
{
    [SerializeField] private EntityHealth _wizardHealth;
    [SerializeField] private ObjectPoolMonoBehaviour _ghostPool;
    [SerializeField] private ObjectPoolMonoBehaviour _fireballPool;
    [SerializeField] private int _maxGhosts = 3;
    [SerializeField] private float _minSpawnIntervalGhosts = 2.5f;
    [SerializeField] private float _maxSpawnIntervalGhosts = 4.5f;
    [SerializeField] private float _minSpawnIntervalFireballs = 1.5f;
    [SerializeField] private float _maxSpawnIntervalFireballs = 3.5f;
    private Collider _playerCollider;
    private readonly List<GhostController> _ghosts = new();
    private readonly List<Fireball> _fireballs = new();

    private void Awake()
    {
        _playerCollider = FindFirstObjectByType<PlayerController>().gameObject.GetComponent<Collider>();
        _wizardHealth.OnDeath += () =>
        {
            while (_ghosts.Count > 0)
            {
                RemoveGhost(_ghosts[0]);
            }

            Destroy(gameObject);
        };

        StartCoroutine(StartGhostSpawner());
        StartCoroutine(StartFireballSpawner());
        Debug.Assert(GetComponentInParent<Room>());
    }

    private void OnDestroy()
    {
        if (transform.parent) transform.parent.GetComponent<Room>().EnemiesAlive--;
    }

    private IEnumerator StartGhostSpawner()
    {
        yield return new WaitForSeconds(Random.Range(_minSpawnIntervalGhosts, _maxSpawnIntervalGhosts));
        if (_ghosts.Count < _maxGhosts)
        {
            SpawnGhost();
        }

        StartCoroutine(StartGhostSpawner());
    }

    private IEnumerator StartFireballSpawner()
    {
        yield return new WaitForSeconds(Random.Range(_minSpawnIntervalFireballs, _maxSpawnIntervalFireballs));
        _fireballPool.Pool.ActivateObject(obj =>
        {
            Fireball fireball = obj.GetComponent<Fireball>();
            fireball.Wizard = this;
            obj.transform.position = transform.position;
            obj.transform.forward = Vector3.Normalize(_playerCollider.bounds.center - transform.position);
            _fireballs.Add(fireball);
        });

        StartCoroutine(StartFireballSpawner());
    }

    private void SpawnGhost()
    {
        Vector3 position = transform.position + transform.forward * 0.75f;

        Debug.Assert(_ghostPool);
        Debug.Assert(_ghostPool.Pool != null);
        _ghostPool.Pool.ActivateObject(obj =>
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
        _ghostPool.Pool.ReleaseObject(ghostController.gameObject);
    }

    internal void RemoveFireball(Fireball fireball)
    {
        Debug.Assert(fireball.Wizard == this);
        fireball.Wizard = null;
        _fireballs.Remove(fireball);
        _fireballPool.Pool.ReleaseObject(fireball.gameObject);
    }
}