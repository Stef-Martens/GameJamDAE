using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed;
    private float _horizontalRotation;
    private float _verticalRotation;

    void LateUpdate()
    {
        transform.Rotate(Vector3.up, _horizontalRotation * Time.deltaTime * _rotationSpeed, Space.World);
        transform.Rotate(Vector3.right, _verticalRotation* Time.deltaTime * _rotationSpeed, Space.Self);
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        _horizontalRotation = context.ReadValue<Vector2>().x;
        _verticalRotation = context.ReadValue<Vector2>().y;
    }


}
