using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileThrower : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private GameObject _ballPrefab;
    [SerializeField]
    private Transform _ballParent;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private Transform _releasePosition;
    [Header("Ball Controls")]
    [SerializeField]
    [Range(1, 100)]
    private float _maxThrowStrength = 30f;
    [SerializeField]
    [Range(1, 20)]
    private float _minThrowStrength = 10f;
    [SerializeField]
    [Range(0.01f, 1f)]
    private float _throwStrengthIncrease = 0.1f;

    private float _throwStrength;
    [SerializeField]
    [Range(1, 10)]
    private float _explosionDelay = 5f;
    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int _linePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float _timeBetweenPoints = 0.1f;

    private Transform InitialParent;
    private Vector3 InitialLocalPosition;
    private Quaternion InitialRotation;

    private bool IsBallThrowAvailable = true;
    public LayerMask BallCollisionMask;
    private bool _isAiming;
    private bool _isThrowing;
    private bool _pickedUpBall;

    private void Awake()
    {
        InitialParent = _rb.transform.parent;
        InitialRotation = _rb.transform.localRotation;
        InitialLocalPosition = _rb.transform.localPosition;
        _rb.freezeRotation = true;
        _rb.gameObject.SetActive(false);

        int ballLayer = _rb.gameObject.layer;
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
        if (_isAiming & !_isThrowing)
        {
            ChargeShot();
        }

        if (Application.isFocused && _isAiming)
        {
            _animator.transform.rotation = Quaternion.Euler(
                _animator.transform.eulerAngles.x,
                _camera.transform.rotation.eulerAngles.y,
                _animator.transform.eulerAngles.z
            );

            DrawProjection();

            if (_isThrowing && IsBallThrowAvailable)
            {
                IsBallThrowAvailable = false;
                _animator.SetTrigger("throw");
                _isThrowing = false;
            }
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if(context.action.IsPressed())
        {
            _isAiming= true;
        }
        else
        {
            _isAiming = false;
            _throwStrength = _minThrowStrength;
        }
    }
    private void ChargeShot()
    {
        if (_throwStrength < _maxThrowStrength)
            _throwStrength += _throwStrengthIncrease;
    }

    public void Throw(InputAction.CallbackContext context)
    {
        if(context.action.WasReleasedThisFrame())
        {
            _isThrowing = true;
        }
    }

    private void DrawProjection()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
        Vector3 startPosition = _releasePosition.position;
        Vector3 startVelocity = _throwStrength * _camera.transform.forward / _rb.mass;
        int i = 0;
        _lineRenderer.SetPosition(i, startPosition);
        for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            _lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = _lineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude,
                BallCollisionMask))
            {
                _lineRenderer.SetPosition(i, hit.point);
                _lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }

    public void ReleaseBall()
    {
        if(_pickedUpBall)
        {
            _rb.gameObject.SetActive(false);
            GameObject ball = Instantiate(_ballPrefab, _releasePosition.position, Quaternion.identity);
            ball.tag = "ball";
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.transform.SetParent(null, true);
            rb.AddForce(_camera.transform.forward * _throwStrength, ForceMode.Impulse);
            _pickedUpBall = false;
            _throwStrength = _minThrowStrength;
        }

        IsBallThrowAvailable = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("ball"))
        {
            return;
        }

        _pickedUpBall = true;
        Destroy(collision.gameObject);
        _rb.gameObject.SetActive(true);
    }

}
