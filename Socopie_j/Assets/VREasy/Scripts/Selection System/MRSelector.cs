using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif
#else
using UnityEngine.VR;
#endif

// Specific controller designed for MR experiences (mostly AR, but can be used in VR as well)
// It combines the functionality of other selectors
//      * Selection through sight (SightSelector)
//      * Selection activated by a trigger (PointerSelector)
//      * Grabbing based on hand location

// needs to check
//  scripting runtime .net 4.6

namespace VREasy
{
    public class MRSelector : SightSelector
    {

        public static MRSelector CreateMRSelector(ref GameObject _ref)
        {
            MRSelector sel = _ref.AddComponent<MRSelector>();
            return sel;
        }

        public VRGrabTrigger selectionTrigger;
        public VRGrabTrigger grabTrigger;
#if UNITY_2017_2_OR_NEWER
        public XRNode controllerNode = XRNode.RightHand;
#else
        public VRNode controllerNode = VRNode.RightHand;
#endif
        public Transform bodyAnchor;
        public MR_CONTROLLER controller = MR_CONTROLLER.MOTION_CONTROLLER;

        private VRGrabbable _currentGrabbedObject = null;
        private VRSelectable _currentSelectedObject = null;
        private bool lookingAtGrabbable = false;
        private bool lookingAtSelectable = false;
        private Transform xrNodeWrapper;

        protected override void InitialiseSelector()
        {
            base.InitialiseSelector();
            // set transform wrapper for XRNode
            // used in VRGrabbable (SLIDE AND ROTATE)
            GameObject go = new GameObject("XRNode");
            xrNodeWrapper = go.transform;
            // Add Rigidbody, required for VRGrabbable (DRAG)
            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            if (bodyAnchor == null)
            {
                if(Camera.main == null)
                {
                    Debug.Log("[MRSelector]: Body anchor not set and main camera could not be found. Current MRSelector will be used as world reference.");
                    bodyAnchor = transform;
                } else
                {
                    if(Camera.main.transform.parent == null)
                    {
                        Debug.Log("[MRSelector]: Body anchor not set and main camera has no parent. Controller's position will be set with respect to the main camera.");
                        bodyAnchor = transform;
                    } else
                    {
                        bodyAnchor = Camera.main.transform.parent;
                    }
                }
            }
            xrNodeWrapper.parent = bodyAnchor;

            
        }

        // select an object if the trigger is active and we are looking at a VRSelectable
        protected override VRSelectable GetSelectable()
        {
            _currentSelectedObject = GetElement<VRSelectable>();
            if (_currentSelectedObject != null && !_currentSelectedObject.CanSelectWithSight())
            {
                _currentSelectedObject = null;
            }
            lookingAtSelectable = _currentSelectedObject != null;

            // if trigger is not active, object is not selectable
            if (selectionTrigger == null || !selectionTrigger.Triggered())
            {
                _currentSelectedObject = null;
            }

            return _currentSelectedObject;
        }

        // grab an object if looking at it and activating the trigger
        // keep grabbing until trigger is released
        protected override VRGrabbable GetGrabbable()
        {
            if(grabTrigger == null)
            {
                return null;
            }
            
            if (!grabTrigger.Triggered())
            {
                lookingAtGrabbable = GetElement<VRGrabbable>() != null;
                _currentGrabbedObject = null;
            } else
            {
                if (_currentGrabbedObject == null)
                {
                    _currentGrabbedObject = GetElement<VRGrabbable>();
                }
                lookingAtGrabbable = _currentGrabbedObject != null;
            }
            
            return _currentGrabbedObject;
        }

        protected override void ChildUpdate()
        {
            base.ChildUpdate();
            SetCrosshairState(lookingAtGrabbable || lookingAtSelectable);
            xrNodeWrapper.localPosition = GetEndPointPosition();
            xrNodeWrapper.localRotation = GetEndPointRotation();
        }

        // calculate changes in position based on a third object
        // Usually hand or controller
        public override Vector3 GetEndPointPosition()
        {
            switch(controller)
            {
                case MR_CONTROLLER.MOTION_CONTROLLER:
                    {
#if UNITY_2019_2_OR_NEWER
                        System.Collections.Generic.List<XRNodeState> nodeStates = new System.Collections.Generic.List<XRNodeState>();
                        InputTracking.GetNodeStates(nodeStates);
                        XRNodeState nodeState = new XRNodeState();

                        foreach (var state in nodeStates)
                        {
                            if (state.nodeType == controllerNode)
                            {
                                nodeState = state;
                            }
                        }


                        Vector3 nodePosition = new Vector3();
                        nodeState.TryGetPosition(out nodePosition);

                        return nodePosition;
#else
                        return InputTracking.GetLocalPosition(controllerNode);
#endif
                    }
                case MR_CONTROLLER.HOLOLENS_HAND:
                    {
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
                        var interactionSourceStates = InteractionManager.GetCurrentReading();
                        foreach (var interactionSourceState in interactionSourceStates)
                        {
                            if (interactionSourceState.source.kind == InteractionSourceKind.Hand)
                            {
                                // can use handedness interactionSourceState.source.handedness;
                                var sourcePose = interactionSourceState.sourcePose;
                                Vector3 sourceGripPosition;
                                if (sourcePose.TryGetPosition(out sourceGripPosition, InteractionSourceNode.Grip))
                                {
                                    return sourceGripPosition;
                                }
                            }
                        }
#endif
#endif
                        return xrNodeWrapper.localPosition;
                    }
            }
            return xrNodeWrapper.localPosition;
            
        }

        // calculate changes in rotation based on a third object
        // Usually hand or controller
        public override Quaternion GetEndPointRotation()
        {
            switch (controller)
            {
                case MR_CONTROLLER.MOTION_CONTROLLER:
                    {
#if UNITY_2019_2_OR_NEWER
                        System.Collections.Generic.List<XRNodeState> nodeStates = new System.Collections.Generic.List<XRNodeState>();
                        InputTracking.GetNodeStates(nodeStates);
                        XRNodeState nodeState = new XRNodeState();

                        foreach (var state in nodeStates)
                        {
                            if (state.nodeType == controllerNode)
                            {
                                nodeState = state;
                            }
                        }


                        Quaternion nodeRotation = new Quaternion();
                        nodeState.TryGetRotation(out nodeRotation);
                        return nodeRotation;
#else
                        return InputTracking.GetLocalRotation(controllerNode);
#endif
                    }
                case MR_CONTROLLER.HOLOLENS_HAND:
                    {
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
                        var interactionSourceStates = InteractionManager.GetCurrentReading();
                        foreach (var interactionSourceState in interactionSourceStates)
                        {
                            if (interactionSourceState.source.kind == InteractionSourceKind.Hand)
                            {
                                var sourcePose = interactionSourceState.sourcePose;
                                Quaternion sourceGripRotation;
                                if (sourcePose.TryGetRotation(out sourceGripRotation, InteractionSourceNode.Grip))
                                {
                                    return sourceGripRotation;
                                }
                            }
                        }
#endif
#endif
                        return xrNodeWrapper.localRotation;
                    }
            }
            return xrNodeWrapper.localRotation;
        }

        // return transform around XRNode attached
        public override Transform GetSelectorTransform()
        {
            return xrNodeWrapper;
        }
    }
}