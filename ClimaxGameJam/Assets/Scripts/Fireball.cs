using System;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _velocity = 10;
    [SerializeField] private float _damage = 1;
    [SerializeField] private LayerMask _ignoreLayers;
    public WizardController Wizard { get; internal set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!Wizard) return;
        if ((_ignoreLayers & 1 << other.gameObject.layer) != 0) return;
        Wizard.RemoveFireball(this);
        if (!other.gameObject.TryGetComponent<PlayerController>(out _)) return;
        EntityHealth health = other.gameObject.GetComponent<EntityHealth>();
        health.Health -= _damage;
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = transform.forward * _velocity;
    }
}