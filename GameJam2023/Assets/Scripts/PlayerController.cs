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

    private InputActionAsset _inputAsset;
    private InputActionMap _player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputAsset = GetComponentInParent<PlayerInput>().actions;
        _player = _inputAsset.FindActionMap("Player");
    }

    private void OnEnable()
    {
        _player.FindAction("Jump").started += Jump;
        _player.FindAction("Move").started += Move;
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

    private void Update()
    {
        _rb.velocity = new Vector3(_horizontalInput * _speed, _rb.velocity.y, _verticalInput * _speed);
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
            if(_rb.velocity.y == 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpingPower);
            }
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {

    }

    public void Throw(InputAction.CallbackContext context)
    {

    }



    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, _radius, groundLayer);
    }
}
