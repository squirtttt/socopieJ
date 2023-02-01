using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class ResetTransformAction : VRAction
    {
        public Transform target;
        public bool snap = false;
        public float interpolationSpeed = 3.0f;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private bool reset = false;

        private const float EPSILON = 0.01f;

        void Start()
        {
            _originalPosition = target.position;
            _originalRotation = target.rotation;
        }

        public override void Trigger()
        {
            if(target == null)
            {
                Debug.LogWarning("ResetTransformAction: target not set");
            } else
            {
                reset = true;
                
            }
        }

        void Update()
        {
            if (!reset)
                return;

            if(snap)
            {
                target.position = _originalPosition;
                target.rotation = _originalRotation;
            } else
            {
                target.transform.position = Vector3.Lerp(target.position,_originalPosition,Time.deltaTime * interpolationSpeed);
                target.transform.rotation = Quaternion.Lerp(target.rotation,_originalRotation,Time.deltaTime * interpolationSpeed);
            }

            if (Vector3.Distance(target.position, _originalPosition) <= EPSILON && Quaternion.Angle(target.rotation, _originalRotation) <= EPSILON)
                reset = false;
            
        }
    }
}