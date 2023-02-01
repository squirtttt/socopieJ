using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{
    public class Transform2TransformController : Transform2Component
    {
        public TRANSFORM_TARGET destinationElement = TRANSFORM_TARGET.POSITION;
        public TRANSFORM_TARGET_ELEMENT destinationAxis = TRANSFORM_TARGET_ELEMENT.X;
        public Transform destination;
        public float mappingScale = 1f;

        // Use this for initialization
        void Start()
        {
            if (destination == null) destination = transform;
        }

        // Update is called once per frame
        protected override void SetDestinationChange(float value)
        {
            if (destination == null) return;
            switch (destinationElement)
            {
                case TRANSFORM_TARGET.POSITION:
                    {
                        Vector3 currentPosition = destination.position;
                        vectorChange(ref currentPosition, value * mappingScale, destinationAxis);
                        destination.position = currentPosition;
                    }
                    break;
                case TRANSFORM_TARGET.ROTATION:
                    {
                        Vector3 currentRotation = destination.eulerAngles;
                        vectorChange(ref currentRotation, value * mappingScale, destinationAxis);
                        destination.eulerAngles = currentRotation;
                    }
                    break;
            }
            vectorChange(ref referenceOriginValues, value, originAxis);
        }

        private void vectorChange(ref Vector3 vector, float value, TRANSFORM_TARGET_ELEMENT element)
        {
            switch (element)
            {
                case TRANSFORM_TARGET_ELEMENT.X:
                    vector.x += value;
                    break;
                case TRANSFORM_TARGET_ELEMENT.Y:
                    vector.y += value;
                    break;
                case TRANSFORM_TARGET_ELEMENT.Z:
                    vector.z += value;
                    break;
            }
        }
    }
}