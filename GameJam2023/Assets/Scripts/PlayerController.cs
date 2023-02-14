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

    private float _horizontalInput;
    private Rigidbody _rb;

    public LayerMask groundLayer;
    private float _verticalInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpingPower);
        }

        if (context.canceled && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
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
        Debug.DrawRay(groundCheck.position, Vector3.down, Color.red, 1);

        if (Physics.Raycast(groundCheck.position, Vector3.down, _radius, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
