using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform _groundCheck;
    [SerializeField]
    private Transform _camTarget;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _jumpingPower;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private Camera _camera;

    private float _horizontalInput;
    private float _verticalInput;
    private float _movementForce = 1f;
    private float _maxSpeed = 5f;

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

    private void FixedUpdate()
    {
        if (canMove)
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
            if (_rb.velocity.y == 0)
            {
                _forceDirection += Vector3.up * _jumpingPower;
                _animator.SetTrigger("jump");
            }
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {

    }

    public void Throw(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _animator.SetTrigger("throw");
        }
    }

    public void Stunned()
    {
        canMove = false;
        Invoke("StartAgain", 2f);
    }

    void StartAgain()
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
        return Physics.CheckSphere(_groundCheck.position, _radius, groundLayer);
    }



}

