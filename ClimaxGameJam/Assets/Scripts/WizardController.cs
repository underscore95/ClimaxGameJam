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
    [SerializeField] private float _minSpawnInterval = 1.75f;
    [SerializeField] private float _maxSpawnInterval = 2.5f;
    [SerializeField] private float _ghostChance = 0.5f;
    [SerializeField] private AnimatedSprite _animatedSprite;
    [SerializeField] private GameSound _deathSound;
    [SerializeField] private GameSound _fireballSound;
    private Collider _playerCollider;
    private readonly List<GhostController> _ghosts = new();
    private readonly List<Fireball> _fireballs = new();

    private void Awake()
    {
        _playerCollider = FindFirstObjectByType<PlayerController>().gameObject.GetComponent<Collider>();
        _wizardHealth.OnDeath += () =>
        {
            StartCoroutine(_deathSound.Play(transform));

            while (_ghosts.Count > 0)
            {
                RemoveGhost(_ghosts[0]);
            }

            Destroy(gameObject);
        };

        StartCoroutine(StartSpawnRoutine());
        Debug.Assert(GetComponentInParent<Room>());
    }

    private void OnDestroy()
    {
        if (transform.parent) transform.parent.GetComponent<Room>().EnemiesAlive--;
    }

    private IEnumerator StartSpawnRoutine()
    {
        yield return new WaitForSeconds(Random.Range(_minSpawnInterval, _maxSpawnInterval));

        bool spawnGhost = Random.Range(0.0f, 1.0f) < _ghostChance;
        if (_ghosts.Count < _maxGhosts && spawnGhost)
        {
            yield return SpawnGhost();
        }
        else
        {
            yield return SpawnFireball();
        }

        _animatedSprite.PlayDefaultAnimation();
        StartCoroutine(StartSpawnRoutine());
    }

    private IEnumerator SpawnFireball()
    {
        StartCoroutine(_fireballSound.Play(transform));
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        _animatedSprite.Play("FireCharge");
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        _animatedSprite.Play("FireThrow");

        _fireballPool.Pool.ActivateObject(obj =>
        {
            Fireball fireball = obj.GetComponent<Fireball>();
            fireball.Wizard = this;
            obj.transform.position = transform.position;
            obj.transform.forward = Vector3.Normalize(_playerCollider.bounds.center - transform.position);
            _fireballs.Add(fireball);
        });

        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
    }

    private IEnumerator SpawnGhost()
    {
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        _animatedSprite.Play("SummonGhost");
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        _animatedSprite.Play("SpawnGhost");

        Vector3 position = transform.position + transform.forward * 0.75f;

        Debug.Assert(_ghostPool);
        Debug.Assert(_ghostPool.Pool != null);
        _ghostPool.Pool.ActivateObject(obj =>
        {
            Debug.Assert(obj);
            obj.transform.position = position + Vector3.down * 0.05f;

            GhostController ghost = obj.GetComponent<GhostController>();
            ghost.Wizard = this;

            _ghosts.Add(ghost);
        });

        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
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