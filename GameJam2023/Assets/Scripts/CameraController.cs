using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject virtualPlayerCam;

    void Start()
    {
        PlayerController[] playerAmount = FindObjectsOfType<PlayerController>();
        int layer = playerAmount.Length + 7;

        virtualPlayerCam.layer = layer;

        var bitMask = (1 << layer)
            | (1 << 0)
            | (1 << 1)
            | (1 << 2)
            | (1 << 4)
            | (1 << 5)
            | (1 << 8);

        cam.cullingMask = bitMask;
        cam.gameObject.layer = layer;
    }
}
