using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    private Animator _animator;

    private Transform _comedianTransform;
    [SerializeField] private Transform _rightHand;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightFoot;
    [SerializeField] private Transform _leftFoot;

    private float _weight = 1;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _comedianTransform = FindObjectOfType<JokeMaker>().transform;
        if (Vector3.Distance(_comedianTransform.position, transform.position) < 2)
        {
            _comedianTransform = null;
        }
    }

    public void SetWeight(float newWeight)
    {
        _weight = newWeight;
    }

    public void SetIKParts(Transform rHand, Transform lHand, Transform rFoot, Transform lFoot)
    {
        _rightHand = rHand;
        _leftHand = lHand;
        _rightFoot = rFoot;
        _leftFoot = lFoot;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!_animator)
        {
            return;
        }

        if (_comedianTransform)
        {
            _animator.SetLookAtWeight(_weight);
            _animator.SetLookAtPosition(_comedianTransform.position);
        }

        if (_rightHand)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _weight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _weight);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHand.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHand.rotation);
        }

        if (_leftHand)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _weight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _weight);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHand.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHand.rotation);
        }

        if (_rightFoot)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _weight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _weight);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, _rightFoot.position);
            _animator.SetIKRotation(AvatarIKGoal.RightFoot, _rightFoot.rotation);
        }

        if (_leftFoot)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _weight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _weight);
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFoot.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, _leftFoot.rotation);
        }
    }
}
