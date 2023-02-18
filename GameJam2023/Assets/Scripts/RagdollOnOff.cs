using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnOff : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Rigidbody _mainRb;
    [SerializeField]
    private GameObject _playerRig;
    [SerializeField]
    private Collider _mainCollider;

    private Collider[] _ragdollColliders;
    private Rigidbody[] _ragdollRigidbodies;

    private void Awake()
    {
        GetRagdollComponents();
        RagDollModeOff();
    }

    private void GetRagdollComponents()
    {
        _ragdollColliders = _playerRig.GetComponentsInChildren<Collider>();
        _ragdollRigidbodies = _playerRig.GetComponentsInChildren<Rigidbody>();
    }

    private void RagDollModeOn()
    {

        foreach(Collider col in _ragdollColliders)
        {
            col.enabled = true;
        }

        foreach(Rigidbody rb in _ragdollRigidbodies)
        {
            rb.isKinematic= false;
        }

        _animator.enabled = false;
        _mainCollider.enabled= false;
        _mainRb.isKinematic = true;
    }

    private void RagDollModeOff()
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
        _mainRb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("flyingBall"))
        {
            RagDollModeOn();
        }
    }
}
