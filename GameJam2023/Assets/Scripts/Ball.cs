using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public LayerMask groundLayer;

    private void FixedUpdate()
    {
        if(!IsGrounded())
        {
            gameObject.tag = "flyingBall";
        }
        else
        {
            gameObject.tag = "ball";
        }
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.5f, groundLayer);
    }

}
