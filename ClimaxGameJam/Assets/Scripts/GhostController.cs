using System;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _rotationSpeed = 90;
    [SerializeField] private float _teleportDistance = 0.25f;
    [SerializeField] private GameObject _player;
    private EntityHealth _playerHealth;
    private bool _hit = false;

    private void Awake()
    {
        _playerHealth = _player.GetComponent<EntityHealth>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != _player) return;

        HitPlayer();
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

        // glitches out when it gets too close
        if (Vector3.Distance(_player.transform.position, transform.position) < _teleportDistance)
        {
            HitPlayer();
        }
    }

    private void HitPlayer()
    {
        if (_hit) return;
        _hit = true;
        _playerHealth.Health -= _damage;
        Destroy(gameObject);
    }
}