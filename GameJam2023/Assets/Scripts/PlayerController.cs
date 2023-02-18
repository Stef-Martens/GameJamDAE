using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform _camTarget;
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _jumpingPower;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Collider _collider;

    private float _horizontalInput;
    private float _verticalInput;
    private float _movementForce = 1f;
    private float _distToGround;

    private Rigidbody _rb;

    private InputActionAsset _inputAsset;
    private InputActionMap _player;
    private InputAction _move;

    private Animator _animator;

    private Vector3 _forceDirection = Vector3.zero;

    public LayerMask groundLayer;
    public bool canMove = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputAsset = GetComponentInParent<PlayerInput>().actions;
        _player = _inputAsset.FindActionMap("Player");
        _animator= GetComponent<Animator>();
        _distToGround = _collider.bounds.extents.y;
    }

    private void OnEnable()
    {
        _player.FindAction("Jump").started += Jump;
        _move = _player.FindAction("Move");
        _move.started += Move;
        _player.Enable();
    }

    private void OnDisable()
    {
        _player.FindAction("Jump").started -= Jump;
        _player.FindAction("Move").started -= Move;
        _player.Disable();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            _animator.SetFloat("speed", _rb.velocity.magnitude / _maxSpeed);

            _forceDirection += _horizontalInput * GetCameraRight(_camera) * _movementForce;
            _forceDirection += _verticalInput * GetCameraForward(_camera) * _movementForce;

            _rb.AddForce(_forceDirection, ForceMode.Impulse);
            _forceDirection = Vector3.zero;

            if (_rb.velocity.y < 0f)
                _rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

            Vector3 horizontalVelocity = _rb.velocity;
            horizontalVelocity.y = 0;
            if (horizontalVelocity.sqrMagnitude > _maxSpeed * _maxSpeed)
                _rb.velocity = horizontalVelocity.normalized * _maxSpeed + Vector3.up * _rb.velocity.y;

            LookAt();
        }
    }

    private void Update()
    {
    }
    public void Join(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontalInput = context.ReadValue<Vector2>().x;
        _verticalInput = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            _animator.SetBool("IsJumping", true);
            _forceDirection += Vector3.up * _jumpingPower;
        }
        else
        {
            _animator.SetBool("IsJumping", false);
        }
    }

    public void StunPlayer()
    {
        canMove = false;
        Invoke("StartAgain", 2f);
    }

    private void MoveAgain()
    {
        canMove = true;
    }

    private void LookAt()
    {
        Vector3 direction = _rb.velocity;
        direction.y = 0f;

        if (_move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            _rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            _rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.1f, groundLayer);
    }




}

