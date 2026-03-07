using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    enum RaycastType
    {
        Ray,
        Sphere
    }

    [SerializeField] private InputActionReference _shootInput;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _maxDistance = 50.0f;
    [SerializeField] private RaycastType _raycastType;
    [SerializeField] private float _sphereRadius = 1;

    private void Awake()
    {
        _shootInput.action.Enable();
    }

    private void Update()
    {
        if (!_shootInput.action.WasCompletedThisFrame()) return;

        if (_raycastType == RaycastType.Ray)
        {
            if (Physics.Raycast(
                    _camera.transform.position,
                    _camera.transform.forward,
                    out var hit,
                    _maxDistance,
                    _enemyLayer))
            {
                OnHit(hit);
            }
        }
        else if (_raycastType == RaycastType.Sphere)
        {
            if (Physics.SphereCast(
                    new Ray(_camera.transform.position, _camera.transform.forward),
                    _sphereRadius,
                    out var hit,
                    _maxDistance,
                    _enemyLayer))
            {
                OnHit(hit);
            }
        } else Debug.Assert(false);
    }

    private void OnHit(RaycastHit hit)
    {
        if (hit.transform.gameObject.TryGetComponent<EntityHealth>(out var health))
        {
            health.Health -= 1;
        }
    }
}