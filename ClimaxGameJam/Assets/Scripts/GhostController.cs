using System;
using System.Collections;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private EntityHealth _ghostHealth;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _rotationSpeed = 90;
    [SerializeField] private AnimatedSprite _animatedSprite;
    [SerializeField] private GameSound _dieSound;
    [SerializeField] private GameSound _spawnSound;
    private GameObject _player;
    private EntityHealth _playerHealth;
    private bool _hit;
    private bool _dead;
    public WizardController Wizard { get; internal set; }

    private void Awake()
    {
        _player = FindFirstObjectByType<PlayerController>().gameObject;
        _playerHealth = _player.GetComponent<EntityHealth>();
        _ghostHealth.OnDeath += () =>
        {
            StartCoroutine(HandleDeath());
            StartCoroutine(_dieSound.Play(transform));
        };
    }

    private void OnEnable()
    {
        transform.forward = Vector3.Normalize(transform.position - _player.transform.position);
        _hit = false;
        _dead = false;
        PlaySpawnAnimation();
        StartCoroutine(_spawnSound.Play(transform));
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void PlaySpawnAnimation()
    {
        _animatedSprite.Play("Summon");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != _player) return;

        StartCoroutine(HitPlayer());
    }

    private void HandleRotation()
    {
        Vector3 currentForward = transform.forward;
        Vector3 targetForward = Vector3.Normalize(transform.position - _player.transform.position);

        Vector3 newForward = Vector3.RotateTowards(currentForward, targetForward,
            _rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newForward);
    }

    private void HandleMovement()
    {
        Vector3 dir = Vector3.Normalize(_player.transform.position - transform.position);
        _rigidBody.linearVelocity = dir * _speed;
    }

    private IEnumerator HitPlayer()
    {
        if (_hit || _dead) yield break;
        _hit = true;

        _animatedSprite.Play("Attack");
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        _playerHealth.Health -= _damage;
        RemoveGhost();
    }

    private IEnumerator HandleDeath()
    {
        if (_hit || _dead) yield break;
        _dead = true;

        _animatedSprite.Play("Death");
        yield return new WaitForSeconds(_animatedSprite.SecondsUntilLoop());
        RemoveGhost();
    }

    private void RemoveGhost()
    {
        if (Wizard)
        {
            Wizard.RemoveGhost(this);
        }
    }
}