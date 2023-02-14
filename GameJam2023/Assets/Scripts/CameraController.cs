using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    private float _horizontalInput;
    private float _verticalInput;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        _camera.transform.rotation = Quaternion.Euler(_horizontalInput, _verticalInput, 0);
    }

    public void Look(InputAction.CallbackContext context)
    {
        _horizontalInput = context.ReadValue<Vector2>().x;
        _verticalInput = context.ReadValue<Vector2>().y;
        Debug.Log("Look called");
    }
}
