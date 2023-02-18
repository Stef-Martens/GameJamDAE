using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private class BoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }

    private enum PlayerState
    {
        Moving,
        Ragdoll,
        StandingUp,
        ResettingBones
    }

    [SerializeField]
    private Transform _camTarget;
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _jumpingPower;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private GameObject _playerRig;
    [SerializeField]
    private Collider _mainCollider;
    [SerializeField]
    private CameraRotation _cameraRotation;
    [SerializeField]
    private string _standUpStateName;
    [SerializeField]
    private string _standUpClipName;
    [SerializeField]
    private float _timeToResetBones;

    private Collider[] _ragdollColliders;
    private Rigidbody[] _ragdollRigidbodies;
    private PlayerController _playerController;

    private float _horizontalInput;
    private float _verticalInput;
    private float _movementForce = 1f;
    private float _distToGround;
    private float _timeToWakeUp;
    private float _elapsedResetBonesTime;

    private Rigidbody _rb;

    private InputActionAsset _inputAsset;
    private InputActionMap _player;
    private InputAction _move;

    private Animator _animator;
    private Transform _hipsBone;
    private Transform[] _bones;
    private BoneTransform[] _standUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransforms;

    private Vector3 _forceDirection = Vector3.zero;
    private PlayerState _currentState = PlayerState.Moving;

    public LayerMask groundLayer;
    public bool canMove = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputAsset = GetComponentInParent<PlayerInput>().actions;
        _player = _inputAsset.FindActionMap("Player");
        _animator= GetComponent<Animator>();
        _distToGround = _collider.bounds.extents.y;

        GetRagdollComponents();
        PopulateAnimationStartBoneTransforms(_standUpClipName, _standUpBoneTransforms);
        DisableRagDoll();
    }

    private void OnEnable()
    {
        _player.FindAction("Jump").started += Jump;
        _move = _player.FindAction("Move");
        _move.started += Move;
        _player.Enable();
    }

    private void OnDisable()
    {
        _player.FindAction("Jump").started -= Jump;
        _player.FindAction("Move").started -= Move;
        _player.Disable();
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case PlayerState.Moving:
                MovingBehaviour();
                break;
            case PlayerState.Ragdoll:
                RagDollBehaviour();
                break;
            case PlayerState.StandingUp:
                StandingUpBehaviour();
                break;
            case PlayerState.ResettingBones:
                ResettingBonesBehaviour();
                break;
        }
    }
    
    public void Join(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontalInput = context.ReadValue<Vector2>().x;
        _verticalInput = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            _animator.SetBool("IsJumping", true);
            _forceDirection += Vector3.up * _jumpingPower;
        }
        else
        {
            _animator.SetBool("IsJumping", false);
        }
    }

    private void LookAt()
    {
        Vector3 direction = _rb.velocity;
        direction.y = 0f;

        if (_move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            _rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            _rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.1f, groundLayer);
    }

    private void GetRagdollComponents()
    {
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _ragdollColliders = _playerRig.GetComponentsInChildren<Collider>();
        _ragdollRigidbodies = _playerRig.GetComponentsInChildren<Rigidbody>();
        _bones = _hipsBone.GetComponentsInChildren<Transform>();

        _standUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollBoneTransforms = new BoneTransform[_bones.Length];

        for(int boneIndex = 0; boneIndex< _bones.Length; boneIndex++)
        {
            _standUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }
    }

    private void EnableRagDoll()
    {

        foreach (Collider col in _ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        _animator.enabled = false;
        _mainCollider.enabled = false;
        _rb.isKinematic = true;
        canMove = false;
        _cameraRotation.canRotateCam = false;
        _timeToWakeUp = 3f;
    }

    private void DisableRagDoll()
    {
        _animator.enabled = true;

        foreach (Collider col in _ragdollColliders)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }

        _animator.enabled = true;
        _mainCollider.enabled = true;
        _rb.isKinematic = false;
        canMove = true;
        _cameraRotation.canRotateCam = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("flyingBall"))
        {
            TriggerRagdoll();
        }
    }

    private void TriggerRagdoll()
    {
        EnableRagDoll();
        _currentState = PlayerState.Ragdoll;
    }

    private void MovingBehaviour()
    {
        _animator.SetFloat("speed", _rb.velocity.magnitude / _maxSpeed);

        _forceDirection += _horizontalInput * GetCameraRight(_camera) * _movementForce;
        _forceDirection += _verticalInput * GetCameraForward(_camera) * _movementForce;

        _rb.AddForce(_forceDirection, ForceMode.Impulse);
        _forceDirection = Vector3.zero;

        if (_rb.velocity.y < 0f)
            _rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > _maxSpeed * _maxSpeed)
            _rb.velocity = horizontalVelocity.normalized * _maxSpeed + Vector3.up * _rb.velocity.y;

        LookAt();
    }

    private void RagDollBehaviour()
    {
        _timeToWakeUp -= Time.deltaTime;

        if(_timeToWakeUp <= 0)
        {
            RealignPositionToHips();
            PopulateBoneTransforms(_ragdollBoneTransforms);

            _currentState = PlayerState.StandingUp;
            DisableRagDoll();

            _animator.Play(_standUpStateName);
        }
    }

    private void StandingUpBehaviour()
    {
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName(_standUpStateName) == false)
        {
            _currentState = PlayerState.Moving;
        }
    }

    private void ResettingBonesBehaviour()
    {

    }

    private void RealignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        transform.position = _hipsBone.position;

        //kijken of de geraakte speler nog steeds grounded is
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        _hipsBone.position = originalHipsPosition;
    }

    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        //takes a snapshot of the current local position and rotation of the bones
        for(int boneIndex = 0; boneIndex < boneTransforms.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }

    private void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach(AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransforms(_standUpBoneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

}

