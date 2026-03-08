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
    [SerializeField] private LayerMask _ignoreLayers;
    [SerializeField] private float _maxDistance = 50.0f;
    [SerializeField] private RaycastType _raycastType;
    [SerializeField] private float _sphereRadius = 1;
    [SerializeField] private BarUI _cooldownOptional;
    [SerializeField] private float _cooldownSeconds = 1;
    private float _secondsSinceShoot = 0;

    private void Awake()
    {
        _shootInput.action.Enable();
        _ignoreLayers.value |= 1 << gameObject.layer;
        _secondsSinceShoot = _cooldownSeconds;
    }

    private void Update()
    {
        _secondsSinceShoot += Time.deltaTime;
        if (_cooldownOptional)
            _cooldownOptional.SetProgress(Mathf.Min(_cooldownSeconds == 0 ? 1 : _secondsSinceShoot / _cooldownSeconds,
                1));

        if (_secondsSinceShoot < _cooldownSeconds) return;
        if (!_shootInput.action.WasCompletedThisFrame()) return;
        _secondsSinceShoot = 0;

        if (_raycastType == RaycastType.Ray)
        {
            if (Physics.Raycast(
                    _camera.transform.position,
                    _camera.transform.forward,
                    out var hit,
                    _maxDistance,
                    ~_ignoreLayers.value
                ))
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
                    ~_ignoreLayers.value
                ))
            {
                OnHit(hit);
            }
        }
        else Debug.Assert(false);
    }

    private void OnHit(RaycastHit hit)
    {
        if (((1 << hit.transform.gameObject.layer) & _enemyLayer.value) == 0)
        {
            return; // not correct layer
        }

        if (hit.transform.gameObject.TryGetComponent<EntityHealth>(out var health))
        {
            health.Health -= 1;
        }
    }
}