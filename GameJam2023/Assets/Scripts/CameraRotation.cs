using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    private GameObject _orbitY;
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    [Range(0.1f, 3)]
    private float _speed;

    private float _xRotation = 0;
    private float _yRotation = 10;

    private float _xIncrease = 0;
    private float _yIncrease = 0;

    public bool canRotateCam;

    private void LateUpdate()
    {
        TrackPlayer();

        if(canRotateCam)
        {
            RotationY();
            RotationX();
        }
    }

    private void TrackPlayer()
    {
        transform.position = _player.transform.position;
    }

    private void RotationY()
    {
        _yRotation += _yIncrease;
        _yRotation = Mathf.Clamp(_yRotation, 1, 60);
        _orbitY.transform.localEulerAngles = new Vector3(_yRotation, _orbitY.transform.localEulerAngles.y, _orbitY.transform.localEulerAngles.z);
    }

    private void RotationX()
    {
        _xRotation += _xIncrease;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, _xRotation, transform.localEulerAngles.z);
    }

    public void Look(InputAction.CallbackContext context)
    {
        _xIncrease = context.ReadValue<Vector2>().x * _speed;
        _yIncrease = context.ReadValue<Vector2>().y * -_speed;
    }

}
