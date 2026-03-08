using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference _movementInput;
    [SerializeField] private InputActionReference _lookInput;
    [SerializeField] private InputActionReference _jumpInput;
    [SerializeField] private Transform _camera;
    [SerializeField] private Collider _collider;
    [SerializeField] private LayerMask _ground;
    [SerializeField] private float _acceleration = 0.75f;
    [SerializeField] private float _maxSpeed = 5.0f;
    [SerializeField] private float _speedDecay = 0.5f;
    [SerializeField] private float _sensitivity = 100.0f;
    [SerializeField] private float _jumpVelocity = 100.0f;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Vector2 _angle = Vector2.zero;
    private bool _isOnGround;
    private int _framesSinceJump = 0;

    private void Awake()
    {
        _movementInput.action.Enable();
        _lookInput.action.Enable();
        _jumpInput.action.Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CheckOnGround();
        HandleLook();
        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        _angle += _sensitivity * _lookInput.action.ReadValue<Vector2>();
        _angle = new Vector2(_angle.x % 360, _angle.y % 360);
        _camera.transform.rotation = Quaternion.Euler(-_angle.y, _angle.x, 0);
    }

    private void HandleMovement()
    {
        Vector2 input = _movementInput.action.ReadValue<Vector2>() * Time.fixedDeltaTime;
        Vector3 forward = _camera.forward;
        Vector3 right = _camera.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 delta = forward * input.y + right * input.x;

        Vector3 velocity = _rigidBody.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0, velocity.z);

        if (input.magnitude > 0)
        {
            // moving
            Vector3 newVelocity = horizontal + _acceleration * delta;
            float newSpeed = Mathf.Min(_maxSpeed, newVelocity.magnitude);
            Vector3 result = Vector3.Normalize(newVelocity) * newSpeed;
            _rigidBody.linearVelocity = new Vector3(result.x, velocity.y, result.z);
        }
        else
        {
            // decay
            horizontal -= _speedDecay * Time.fixedDeltaTime * horizontal;
            _rigidBody.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
        }
    }

    private void HandleJump()
    {
        if (_isOnGround && _framesSinceJump > 3 && _jumpInput.action.WasPerformedThisFrame())
        {
            Vector3 velocity = _rigidBody.linearVelocity;
            velocity.y = _jumpVelocity;
            _rigidBody.linearVelocity = velocity;
            _framesSinceJump = 0;
        }

        _framesSinceJump++;
    }

    private void CheckOnGround()
    {
        _isOnGround = Physics.Raycast(
            transform.position,
            Vector3.down,
            out _,
            _collider.bounds.extents.y + 0.01f,
            _ground
        );
    }
}