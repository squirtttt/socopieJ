using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public abstract class LOSSelector : VRSelector
    {
        public float selectionDistance = 10.0f;
        public LayerMask layerMask = -1; // all layers by default

        protected T GetElement<T>() where T : VRElement
        {
            T obj = null;
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, transform.forward, out _hit, selectionDistance,layerMask))
            {
                obj = _hit.collider.gameObject.GetComponent<T>();
            }

            return obj;
        }

        public override Vector3 GetEndPointPosition()
        {
            // if grabbing object, end pointer position ends where it meets the object
            float maxDistance = selectionDistance;
            if (_previouslyGrabbedObject != null)
            {
                maxDistance = Vector3.Distance(transform.position, _previouslyGrabbedObject.transform.position);
                maxDistance = Mathf.Clamp(maxDistance,0.0f, selectionDistance);
            }
            return transform.position + (transform.forward * maxDistance);
            
        }
    }
}