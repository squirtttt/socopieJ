using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class MoveObjectAction : VRAction
    {
        public Transform target;

        public Transform[] pivots;
        
        public bool updatePosition = true;
        public float positionInterpolationSpeed = 5f;
        public bool updateRotation = true;
        public float rotationInterpolationSpeed = 5f;


        private int pivotIndex = -1;
        private bool _transitioningPosition = false;
        private bool _transitioningRotation = false;
        private float threshold = 0.01f;

        public override void Trigger()
        {
            pivotIndex++;
            if (pivotIndex >= pivots.Length)
                pivotIndex = 0;

            if (updatePosition)
                _transitioningPosition = true;
            if (updateRotation)
                _transitioningRotation = true;
            
        }

        void Update()
        {
            if (!target)
            {
                Debug.Log("MoveObjectAction: target object not set");
                return;
            }
            if(pivotIndex > pivots.Length)
            {
                Debug.Log("MoveObjectAction: not enough pivots set");
                return;
            }
            if (_transitioningPosition)
            {
                target.position = Vector3.Lerp(target.position, pivots[pivotIndex].position, Time.deltaTime * positionInterpolationSpeed);
                if (Vector3.Distance(target.position, pivots[pivotIndex].position) < threshold)
                {
                    _transitioningPosition = false;
                }
            }
            if (_transitioningRotation)
            {
                target.rotation = Quaternion.Slerp(target.rotation, pivots[pivotIndex].rotation, Time.deltaTime * rotationInterpolationSpeed);
                if (Quaternion.Angle(target.rotation, pivots[pivotIndex].rotation) < threshold)
                {
                    _transitioningRotation = false;
                }
            }
        }
    }
}