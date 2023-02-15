using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileThrower : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField]
    private Animator Animator;
    [SerializeField]
    private Camera Camera;
    [SerializeField]
    private Rigidbody Ball;
    [SerializeField]
    private Transform BallParent;
    [SerializeField]
    private LineRenderer LineRenderer;
    [SerializeField]
    private Transform ReleasePosition;
    [Header("Ball Controls")]
    [SerializeField]
    [Range(1, 100)]
    private float ThrowStrength = 10f;
    [SerializeField]
    [Range(1, 10)]
    private float ExplosionDelay = 5f;
    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;

    private Transform InitialParent;
    private Vector3 InitialLocalPosition;
    private Quaternion InitialRotation;

    private bool IsBallThrowAvailable = true;
    public LayerMask BallCollisionMask;

    private void Awake()
    {
        InitialParent = Ball.transform.parent;
        InitialRotation = Ball.transform.localRotation;
        InitialLocalPosition = Ball.transform.localPosition;
        Ball.freezeRotation = true;
        Ball.gameObject.SetActive(false);

        int ballLayer = Ball.gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(ballLayer, i))
            {
                BallCollisionMask |= 1 << i; // magic
            }
        }
    }

    private void Update()
    {
        if (Application.isFocused && Mouse.current.rightButton.isPressed)
        {
            Animator.transform.rotation = Quaternion.Euler(
                Animator.transform.eulerAngles.x,
                Camera.transform.rotation.eulerAngles.y,
                Animator.transform.eulerAngles.z
            );

            DrawProjection();

            if (Mouse.current.leftButton.wasReleasedThisFrame && IsBallThrowAvailable)
            {
                IsBallThrowAvailable = false;
                Animator.SetTrigger("throw");
                ReleaseBall();
            }
        }
        else
        {
            LineRenderer.enabled = false;
        }
    }

    private void DrawProjection()
    {
        LineRenderer.enabled = true;
        LineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;
        Vector3 startPosition = ReleasePosition.position;
        Vector3 startVelocity = ThrowStrength * Camera.transform.forward / Ball.mass;
        int i = 0;
        LineRenderer.SetPosition(i, startPosition);
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            LineRenderer.SetPosition(i, point);

            Vector3 lastPosition = LineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude,
                BallCollisionMask))
            {
                LineRenderer.SetPosition(i, hit.point);
                LineRenderer.positionCount = i + 1;
                return;
            }
        }
    }

    private void ReleaseBall()
    {
        //maak clone en instantiate
        //Ball.velocity = Vector3.zero;
        //Ball.angularVelocity = Vector3.zero;
        //Ball.isKinematic = false;
        //Ball.freezeRotation = false;
        //Ball.constraints = RigidbodyConstraints.None;
        //Ball.transform.SetParent(null, true);
        //Ball.AddForce(Camera.transform.forward * ThrowStrength, ForceMode.Impulse);
        Ball.gameObject.SetActive(false);
    }

    private void PickUpBall()
    {
        Ball.constraints = RigidbodyConstraints.FreezePosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("ball"))
        {
            return;
        }

        Destroy(collision.gameObject);
        Ball.gameObject.SetActive(true);
    }

}
