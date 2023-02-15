using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 _cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    public bool lookAtPlayer = false;
    public bool rotateAroundPlayer = true;
    public float rotationSpeed = 5.0f;
    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        _cameraOffset = transform.position - playerTransform.position;
    }

    private void LateUpdate()
    {
        if(rotateAroundPlayer)
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(_xRotation * rotationSpeed, Vector3.up);

            _cameraOffset = camTurnAngle* _cameraOffset;
        }

        Vector3 newPos = playerTransform.position + _cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (lookAtPlayer || rotateAroundPlayer)
            transform.LookAt(playerTransform);
                        
    }

    public void Look(InputAction.CallbackContext context)
    {
        _xRotation = context.ReadValue<Vector2>().x;
        _yRotation = context.ReadValue<Vector2>().y;
    }
}
