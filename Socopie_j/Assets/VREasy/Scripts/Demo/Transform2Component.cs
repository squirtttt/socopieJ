using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{
    public class Transform2Component : MonoBehaviour
    {

        public TRANSFORM_TARGET originElement = TRANSFORM_TARGET.POSITION;
        public TRANSFORM_TARGET_ELEMENT originAxis = TRANSFORM_TARGET_ELEMENT.X;
        public Transform origin;

        protected Vector3 referenceOriginValues;
        private float valueChange;

        void Awake()
        {
            if (origin == null)
            {
                Debug.LogWarning("[VREasy] TransformController (" + gameObject.name +"): Origin transform not set. Disabling script.");
                enabled = false;
                return;
            }
            switch (originElement)
            {
                case TRANSFORM_TARGET.POSITION:
                    referenceOriginValues = origin.position;
                    break;
                case TRANSFORM_TARGET.ROTATION:
                    referenceOriginValues = origin.rotation.eulerAngles;
                    break;
            }
        }

        private void Update()
        {
            switch (originElement)
            {
                case TRANSFORM_TARGET.POSITION:
                    valueChange = getVectorFieldValueChange(origin.position);
                    break;
                case TRANSFORM_TARGET.ROTATION:
                    {
                        float rotation = getVectorFieldValueChange(origin.eulerAngles);
                        valueChange = getPosNegAngle(rotation);
                    }

                    break;
            }
            SetDestinationChange(valueChange);
        }

        public float GetCurrentOriginValue()
        {
            switch (originElement)
            {
                case TRANSFORM_TARGET.POSITION:
                    {
                        switch (originAxis)
                        {
                            case TRANSFORM_TARGET_ELEMENT.X:
                                return origin.transform.localPosition.x;
                            case TRANSFORM_TARGET_ELEMENT.Y:
                                return origin.transform.localPosition.y;
                            case TRANSFORM_TARGET_ELEMENT.Z:
                                return origin.transform.localPosition.z;
                        }
                    }
                    break;
                case TRANSFORM_TARGET.ROTATION:
                    {
                        switch (originAxis)
                        {
                            case TRANSFORM_TARGET_ELEMENT.X:
                                return getPosNegAngle(origin.transform.localEulerAngles.x);
                            case TRANSFORM_TARGET_ELEMENT.Y:
                                return getPosNegAngle(origin.transform.localEulerAngles.y);
                            case TRANSFORM_TARGET_ELEMENT.Z:
                                return getPosNegAngle(origin.transform.localEulerAngles.z);
                        }
                    }
                    break;                    
            }
            return valueChange;
        }

        private float getPosNegAngle(float angle)
        {
            if (angle + 180 > 360) angle -= 360;
            else if (angle - 180 < -360) angle += 360;
            return angle;
        }

        private float getVectorFieldValueChange(Vector3 vector)
        {
            switch (originAxis)
            {
                case TRANSFORM_TARGET_ELEMENT.X:
                    return vector.x - referenceOriginValues.x;
                case TRANSFORM_TARGET_ELEMENT.Y:
                    return vector.y - referenceOriginValues.y;
                case TRANSFORM_TARGET_ELEMENT.Z:
                    return vector.z - referenceOriginValues.z;
            }
            return 0;
        }

        protected virtual void SetDestinationChange(float valueChange) { }

    }
}