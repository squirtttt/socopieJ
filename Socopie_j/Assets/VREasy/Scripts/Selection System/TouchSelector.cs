using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class TouchSelector : VRSelector
    {
        public static TouchSelector CreateTouchSelector(ref GameObject _ref)
        {
            Rigidbody rb = _ref.GetComponent<Rigidbody>();
            bool hadRb = (rb != null);
            Collider cc = _ref.GetComponent<Collider>();
            if (cc == null)
            {
                BoxCollider col = _ref.AddComponent<BoxCollider>();
                col.size = new Vector3(0.1f, 0.1f, 0.2f);
                col.center = Vector3.forward * 0.1f;
                col.isTrigger = true;
            }
            else
            {
                if (!cc.isTrigger)
                {
                    Debug.LogWarning("The current selector object contains a collider that is not set to be a trigger. TouchSelector requires your collider to be a trigger. If this is an issue, please add a second (non-trigger) collider to your selector object.");
                }
                cc.isTrigger = true;
            }
            TouchSelector sel = _ref.AddComponent<TouchSelector>();
            if (!hadRb) sel.ConfigureRigidbody();

            return sel;
        }

        public VRGrabTrigger grabTrigger;

        private VRSelectable _selectObject = null;
        private VRGrabbable _grabObject = null;
        
        protected override VRSelectable GetSelectable()
        {
            return _selectObject;
        }

        protected override VRGrabbable GetGrabbable()
        {
            return _grabObject;
        }

        public void ConfigureRigidbody()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        void OnTriggerStay(Collider col)
        {
            getTouchSelectable(col);
            getTouchGrabbable(col);
        }

        void OnCollisionStay(Collision col)
        {
            getTouchSelectable(col.collider);
            getTouchGrabbable(col.collider);
        }

        void OnTriggerExit(Collider col)
        {
            _selectObject = null;
        }

        void OnCollisionExit(Collision col)
        {
            _selectObject = null;
        }

        private void getTouchSelectable(Collider col)
        {
            if (_selectObject != null) return;
            _selectObject = col.gameObject.GetComponent<VRSelectable>();
            if (_selectObject != null && !_selectObject.CanSelectWithTouch())
            {
                _selectObject = null;
            }
        }

        private void getTouchGrabbable(Collider col)
        {
            if (_grabObject != null) return;
            if (grabTrigger != null)
            {
                if (grabTrigger.Triggered())
                {
                    // if object already grabbed, ignore anything else
                    if (_grabObject == null)
                    {
                        _grabObject = col.gameObject.GetComponent<VRGrabbable>();
                    }
                    return;
                }
            }
            _grabObject = null;
        }

        protected override void ChildUpdate()
        {
            // if object is grabbed but grabbable goes out of collider / trigger, clean up if no longer triggered
            if(_grabObject != null && grabTrigger != null)
            {
                if(!grabTrigger.Triggered())
                {
                    _grabObject.StopGrab(this);
                    _grabObject = null;
                }
            }
        }
    }
}