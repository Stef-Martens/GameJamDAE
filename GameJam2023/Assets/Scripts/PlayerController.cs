using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _jumpingPower;
    [SerializeField]
    private float _throwDistance;
    [SerializeField]
    private float _throwSpeed;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private Camera _camera;

    private float _horizontalInput;
    private Rigidbody _rb;

    public LayerMask groundLayer;
    private float _verticalInput;
    private PlayerInput _playerInput;
    private float _horizontalRotation;
    private float _verticalRotation;
    private float _maxSpeed = 5f;

    private InputActionAsset _inputAsset;
    private InputActionMap _player;
    private InputAction _move;
    private Vector3 _forceDirection = Vector3.zero;
    private float _movementForce = 1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputAsset = GetComponentInParent<PlayerInput>().actions;
        _player = _inputAsset.FindActionMap("Player");
    }

    private void OnEnable()
    {
        _player.FindAction("Jump").started += Jump;
        _move = _player.FindAction("Move");
        _move.started += Move;
        _player.FindAction("Aim").started += Aim;
        _player.FindAction("Throw").started += Throw;
        _player.Enable();
    }

    private void OnDisable()
    {
        _player.FindAction("Jump").started -= Jump;
        _player.FindAction("Move").started -= Move;
        _player.FindAction("Aim").started -= Aim;
        _player.FindAction("Throw").started -= Throw;
        _player.Disable();
    }

    //private void Update()
    //{
    //    _rb.velocity = new Vector3(_horizontalInput * _speed, _rb.velocity.y, _verticalInput * _speed);
    //}

    private void FixedUpdate()
    {
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

    public void Join(InputAction.CallbackContext context)
    {
        if(context.performed)
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
            if (_rb.velocity.y == 0)
            {
                _forceDirection += Vector3.up * _jumpingPower;
            }
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {

    }

    public void Throw(InputAction.CallbackContext context)
    {

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
        return Physics.CheckSphere(groundCheck.position, _radius, groundLayer);
    }



}
    
