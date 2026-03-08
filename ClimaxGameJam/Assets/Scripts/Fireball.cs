using System;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _velocity = 10;
    [SerializeField] private float _damage = 1;
    public WizardController Wizard { get; internal set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<PlayerController>(out _)) return;
        EntityHealth health = other.gameObject.GetComponent<EntityHealth>();
        health.Health -= _damage;
        Wizard.RemoveFireball(this);
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = transform.forward * _velocity;
    }
}