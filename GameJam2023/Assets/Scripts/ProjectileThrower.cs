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
    private Rigidbody _ballRb;
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
    private float _throwStrength = 10f;
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
    private float _rbMassCached;

    private InputActionAsset _inputAsset;
    private InputActionMap _player;
    private int _ballCounter;

    private void Awake()
    {
        _inputAsset = GetComponentInParent<PlayerInput>().actions;
        _player = _inputAsset.FindActionMap("Player");

        InitialParent = _ballRb.transform.parent;
        InitialRotation = _ballRb.transform.localRotation;
        InitialLocalPosition = _ballRb.transform.localPosition;
        _ballRb.freezeRotation = true;
        _rbMassCached = _ballRb.mass;
        _ballRb.gameObject.SetActive(false);

        int ballLayer = _ballRb.gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(ballLayer, i))
            {
                BallCollisionMask |= 1 << i; // magic
            }
        }
    }

    private void OnEnable()
    {
        _player.FindAction("Aim").started += Aim;
        _player.FindAction("Throw").started += Throw;
        _player.Enable();
    }

    private void OnDisable()
    {
        _player.FindAction("Aim").started -= Aim;
        _player.FindAction("Throw").started -= Throw;
        _player.Disable();
    }

    private void Update()
    {
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
            }
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if(context.action.IsInProgress())
        {
            _isAiming= true;
        }
        else
        {
            _isAiming = false;
        }
    }

    public void Throw(InputAction.CallbackContext context)
    {
        if(context.action.IsPressed())
        {
            _isThrowing = true;
        }
        else
        {
            _isThrowing = false;
        }
    }

    private void DrawProjection()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
        Vector3 startPosition = _releasePosition.position;
        Vector3 startVelocity = _throwStrength * _camera.transform.forward / _rbMassCached;
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
            _ballRb.gameObject.SetActive(false);
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
            _ballCounter = 0;
        }

        IsBallThrowAvailable = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("ball"))
        {
            return;
        }

        _ballCounter++;
        _pickedUpBall = true;
        if(_ballCounter < 2)
        {
            Destroy(collision.gameObject);
            _ballRb.gameObject.SetActive(true);
        }
    }

}
