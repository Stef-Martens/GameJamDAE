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

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Join(InputAction.CallbackContext context)
    {
        if(context.performed)
        {

        }
    }


    public void Move(InputAction.CallbackContext context)
    {
        if(context.performed && IsGrounded())
        {

        }
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

        Debug.Log(IsGrounded());
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
