using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if VREASY_VREE_PLATFORM_SDK
using VREasy.Networking;
using VREasy.Networking.Commands;
#endif
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    [RequireComponent(typeof(Collider))]
#if VREASY_VREE_PLATFORM_SDK
    public class VRGrabbable : VRElement, IVRGrabbableNetworkEvents
#else
    public class VRGrabbable : VRElement
#endif
    {
        public ActionList EndGrabActions
        {
            get
            {
                if (endGrabActions == null)
                {
                    Transform child = transform.Find("EndGrabActions");
                    if(child == null)
                    {
                        GameObject c = new GameObject("EndGrabActions");
                        c.transform.parent = transform;
                        child = c.transform;
                    }
                    
                    endGrabActions = child.gameObject.GetComponent<ActionList>();
                    if(endGrabActions == null)
                    {
                        endGrabActions = child.gameObject.AddComponent<ActionList>();
                    }
                }
                return endGrabActions;
            }
        }

        public ActionList StartGrabActions
        {
            get
            {
                if (startGrabActions == null)
                {
                    Transform child = transform.Find("StartGrabActions");
                    if (child == null)
                    {
                        GameObject c = new GameObject("StartGrabActions");
                        c.transform.parent = transform;
                        child = c.transform;
                    }

                    startGrabActions = child.gameObject.GetComponent<ActionList>();
                    if (startGrabActions == null)
                    {
                        startGrabActions = child.gameObject.AddComponent<ActionList>();
                    }
                }
                return startGrabActions;
            }
        }
        private Transform targetTransform {
            get
            {
                if (_targetTransform == null) return transform;
                return _targetTransform;
            }
            set
            {
                _targetTransform = value;
            }
        }
        private Transform _targetTransform = null;

        public ActionList endGrabActions;
        public ActionList startGrabActions;

        public GRAB_TYPE type = GRAB_TYPE.DRAG;
        public JOINT_TYPE joint_type = JOINT_TYPE.FIXED;

        public bool xAxis = true;
        public bool yAxis = true;
        public bool zAxis = true;
        
        public Color grabColour = Color.yellow;
        public bool alignWithPivot = false;
        public bool snapToPosition = false;
        public bool snapToOrigin = false;
        public bool snapToClosest = false;
        public float snapRadius = 0.25f;
        public List<Transform> snapPositions = new List<Transform>();
        public float minRotation = 0f;
        public float maxRotation = 90f;
        public Vector3 minMovement = Vector3.zero;
        public Vector3 maxMovement = Vector3.zero;
        public bool constraintMovement = false;
        public float throwingForce = 25f;
        public float translationScale = 1000.0f;
        public bool usePosition = false;
        public bool useGrabColour = true;

        // COMMON JOINT PROPERTIES
        public float breakForce = Mathf.Infinity;
        public float breakTorque = Mathf.Infinity;

        // SPRING JOINT PROPERTIES
        public Vector2 jointDistance = new Vector2(0,0.2f);
        public float damper = 0f;
        public float spring = 50f;

        public Transform pivot;

        // spring back on release options
        public bool springToOrigin = false;
        public float springSpeed = 10f;

        private Vector3 rotateAround;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private bool isGrabbed = false;
        private Transform selectorTransform;
        private Quaternion selectorOffset_rotation;
        private Vector3 selectorOffset_position;
        private Vector3 selectorTransformOffset_position;
        private VRSelector currentSelector;
        private bool wasKinematic;
        private bool dragKinematic = false;
        private List<Color> colourList = new List<Color>();

        private Vector3 lastPosition = Vector3.zero;
        private Vector3 currentVelocity = Vector3.zero;
        private Rigidbody ownRb = null;
        private Joint joint = null;
        private bool hadRb = false;

        public event Action OnStartGrab;
        public event Action OnStopGrab;

        //private Collider _collider = null;
        //private bool wasTrigger = false;



        protected void Awake()
        {            

            loadDefaultColours();
            //_collider = GetComponent<Collider>();
            //if(_collider.GetType() != typeof(MeshCollider))
            //    wasTrigger = _collider.isTrigger;
            targetTransform = pivot == null ? transform : pivot;
            SetOrigins();
            // prep rotation constraints (only if 1 axis selected)
            if (type == GRAB_TYPE.ROTATE && constraintMovement)
            {
                if (xAxis && !yAxis && !zAxis)
                {
                    rotateAround = Vector3.right;
                }
                if (!xAxis && yAxis && !zAxis)
                {
                    rotateAround = Vector3.up;
                }
                if (!xAxis && !yAxis && zAxis)
                {
                    rotateAround = Vector3.forward;
                }
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            // only draw if selected in the inspector
            if (Selection.activeGameObject != gameObject) return;
            Color oldColour = Gizmos.color;
            Gizmos.color = Color.red;
            if (snapToPosition)
            {
                if (snapToOrigin)
                {
                    Gizmos.DrawWireSphere(originalPosition, snapRadius);
                }

                foreach (Transform t in snapPositions)
                {
                    Gizmos.DrawWireSphere(t.position, snapRadius);
                }
                
            }
            Gizmos.color = oldColour;
#endif
        }
        
        public virtual void StartGrab(VRSelector selector)
        {            
            paintColours(false);
            //if (_collider.GetType() != typeof(MeshCollider))
            //    _collider.isTrigger = true;
            
            switch (type)
            {
                case GRAB_TYPE.DRAG:
                    {
                        if (isGrabbed) return;
                        currentSelector = selector;
                        if (alignWithPivot)
                        {
                            transform.rotation = selector.transform.rotation;
                            transform.position = selector.transform.position;
                        }
                        ownRb = GetComponent<Rigidbody>();
                        hadRb = ownRb != null;
                        if (ownRb == null)
                        {
                            ownRb = gameObject.AddComponent<Rigidbody>();
                            ownRb.useGravity = false;
                            ownRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        }
                        wasKinematic = ownRb.isKinematic;
                        MeshCollider _collider = GetComponent<MeshCollider>();
                        if(_collider != null && !_collider.convex)
                        {
                            ownRb.isKinematic = true;
                        }
                        if (wasKinematic || ownRb.isKinematic)
                        {
                            selectorOffset_position = selector.GetEndPointPosition() - transform.position;
                            dragKinematic = true;
                        }
                        switch(joint_type)
                        {
                            case JOINT_TYPE.FIXED:
                                {
                                    joint = gameObject.GetComponent<FixedJoint>();
                                    if (joint == null)
                                    {
                                        FixedJoint fj = gameObject.AddComponent<FixedJoint>();
                                        fj.breakForce = breakForce;
                                        fj.breakTorque = breakTorque;
                                        joint = fj;
                                    }
                                }
                                break;
                            /*case JOINT_TYPE.ATTACHED:
                                {
                                    joint = gameObject.GetComponent<ConfigurableJoint>();
                                    if (joint == null)
                                    {
                                        float massMultiplier = ownRb != null ? ownRb.mass : 1f;
                                        ConfigurableJoint cj = gameObject.AddComponent<ConfigurableJoint>();
                                        cj.xMotion = ConfigurableJointMotion.Locked;
                                        cj.yMotion = ConfigurableJointMotion.Locked;
                                        cj.zMotion = ConfigurableJointMotion.Locked;
                                        cj.angularXMotion = ConfigurableJointMotion.Locked;
                                        cj.angularYMotion = ConfigurableJointMotion.Locked;
                                        cj.angularZMotion = ConfigurableJointMotion.Locked;
                                        SoftJointLimit s = new SoftJointLimit();
                                        s.limit = 100f;
                                        s.bounciness = 0;
                                        s.contactDistance = 1f;
                                        cj.linearLimit = s;
                                        SoftJointLimitSpring sp = new SoftJointLimitSpring();
                                        sp.spring = 0;
                                        sp.damper = 0.0f;
                                        cj.linearLimitSpring = sp;
                                        JointDrive jx = new JointDrive();
                                        jx.positionSpring = 500 * massMultiplier;
                                        jx.positionDamper = 0.1f;
                                        jx.maximumForce = 300 * massMultiplier;
                                        cj.xDrive = jx;
                                        JointDrive jy = new JointDrive();
                                        jy.positionSpring = 500 * massMultiplier;//100;
                                        jy.positionDamper = 0.1f;
                                        jy.maximumForce = 300 * massMultiplier;//50;
                                        cj.yDrive = jy;
                                        JointDrive jz = new JointDrive();
                                        jz.positionSpring = 500 * massMultiplier;
                                        jz.positionDamper = 0.1f;
                                        jz.maximumForce = 300 * massMultiplier;
                                        cj.zDrive = jz;
                                        cj.rotationDriveMode = RotationDriveMode.XYAndZ;
                                        cj.projectionMode = JointProjectionMode.None;
                                        joint = cj;

                                    }
                                }
                                break;
                            */
                            case JOINT_TYPE.SPRING:
                                {
                                    joint = gameObject.GetComponent<SpringJoint>();
                                    if (joint == null)
                                    {
                                        SpringJoint sj = gameObject.AddComponent<SpringJoint>();
                                        sj.spring = spring;
                                        sj.damper = damper;
                                        sj.maxDistance = jointDistance.y;
                                        sj.minDistance = jointDistance.x;
                                        sj.breakForce = breakForce;
                                        sj.breakTorque = breakTorque;
                                        joint = sj;
                                    }
                                }
                                break;

                        }

                        if (joint != null)
                        {
                            joint.connectedBody = selector.GetSelectorTransform().GetComponent<Rigidbody>();//selector.GetComponent<Rigidbody>();
                            joint.enablePreprocessing = false;
                        }
                    }
                    break;
                case GRAB_TYPE.SLIDE:
                case GRAB_TYPE.ROTATE:
                    {
                        if (!isGrabbed)
                        {
                            selectorOffset_position = selector.GetEndPointPosition() - transform.position;

                        }
                        currentSelector = selector;
                        selectorTransform = selector.GetSelectorTransform();// selector.transform;
                        selectorOffset_rotation = selector.GetEndPointRotation();//selectorTransform.rotation;
                        selectorTransformOffset_position = selector.GetEndPointPosition();// selectorTransform.position;
                        if (isGrabbed) return;
                        Rigidbody rb = GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            wasKinematic = rb.isKinematic;
                            rb.isKinematic = true;
                        }
                    }
                    break;
            }
            if (!isGrabbed)
            {
                StartGrabActions.Trigger();
                if(OnStartGrab != null) OnStartGrab.Invoke();
            }
            isGrabbed = true;

        }

        public virtual void StopGrab(VRSelector selector)
        {
            if (isGrabbed)
            {
                EndGrabActions.Trigger();
                if (OnStopGrab != null) OnStopGrab.Invoke();
            }

            dragKinematic = false;
            isGrabbed = false;
            paintColours(true);
            //if (_collider.GetType() != typeof(MeshCollider))
            //    _collider.isTrigger = wasTrigger;

            switch(type)
            {
                case GRAB_TYPE.DRAG:
                    {
                        if (joint != null) Destroy(joint);
                        if (!hadRb) Destroy(ownRb);
                        else
                        {
                            ownRb.velocity = currentVelocity /  ownRb.mass;
                            ownRb.isKinematic = wasKinematic;
                        }
                    }
                    break;
                case GRAB_TYPE.SLIDE:
                case GRAB_TYPE.ROTATE:
                    {
                        currentSelector = null;
                        selectorTransform = null;
                        Rigidbody rb = GetComponent<Rigidbody>();
                        if (rb != null) rb.isKinematic = wasKinematic;
                    }
                    break;
            }
            // if snap to origin, place object in its original state (if close enough)
            if (snapToPosition)
            {
                snapObjectToPosition();
            }

            
        }

        public float GetCurrentAxisValue()
        {
            if(xAxis)
            {
                switch(type)
                {
                    case GRAB_TYPE.ROTATE:
                        return transform.localEulerAngles.x;
                    case GRAB_TYPE.DRAG:
                    case GRAB_TYPE.SLIDE:
                        return transform.localPosition.x;
                }
            } else if(yAxis)
            {
                switch (type)
                {
                    case GRAB_TYPE.ROTATE:
                        return transform.localEulerAngles.y;
                    case GRAB_TYPE.DRAG:
                    case GRAB_TYPE.SLIDE:
                        return transform.localPosition.y;
                }
            }
            else if(zAxis)
            {
                switch (type)
                {
                    case GRAB_TYPE.ROTATE:
                        return transform.localEulerAngles.z;
                    case GRAB_TYPE.SLIDE:
                    case GRAB_TYPE.DRAG:
                        return transform.localPosition.z;
                }
            }
            return 0f;
        }

        void FixedUpdate()
        {
            if (isGrabbed)
            {
                currentVelocity = (transform.localPosition - lastPosition) * throwingForce;
                lastPosition = transform.localPosition;

                if (type == GRAB_TYPE.SLIDE && selectorTransform != null || dragKinematic)
                {
                    // POSITION
                    Vector3 controllerPos = currentSelector.GetEndPointPosition();
                    Vector3 currentOffset = controllerPos - selectorOffset_position;
                    Vector3 moved = currentOffset - transform.position;
                    float moveX = xAxis ? Vector3.Dot(moved, targetTransform.right) : 0;
                    float moveY = yAxis ? Vector3.Dot(moved, targetTransform.up) : 0;
                    float moveZ = zAxis ? Vector3.Dot(moved, targetTransform.forward) : 0;
                    Vector3 m = new Vector3(moveX, moveY, moveZ);
                    //Vector3 m = getControllerChange();
                    /*if (constraintMovement)
                    {
                        Vector3 futurePosition = transform.localPosition + transform.TransformDirection(m);
                        if (xAxis)
                        {
                            if (futurePosition.x > maxMovement.x || futurePosition.x < minMovement.x) m.x = 0.0f;
                        }
                        if (yAxis)
                        {
                            if (futurePosition.y > maxMovement.y || futurePosition.y < minMovement.y) m.y = 0.0f;
                        }
                        if (zAxis)
                        {
                            if (futurePosition.z > maxMovement.z || futurePosition.z < minMovement.z) m.z = 0.0f;
                        }
                    }*/
                    
                    ApplyMovement(m);
                    
                }
                else if (type == GRAB_TYPE.ROTATE && selectorTransform != null)
                {
                    // ROTATION
                    Quaternion finalRotation = Quaternion.identity;
                    if (!usePosition)
                    {
                        // rotation by controllers rotation
                        Quaternion diff = Quaternion.Inverse(selectorOffset_rotation) * currentSelector.GetEndPointRotation();//selectorTransform.rotation;
                        Vector3 controllerAxis = Vector3.zero;
                        float controllerAngle = 0.0f;
                        diff.ToAngleAxis(out controllerAngle, out controllerAxis);
                        //Vector3 objectAxis = transform.InverseTransformDirection(selectorTransform.TransformDirection(controllerAxis));
                        Vector3 objectAxis = transform.InverseTransformDirection(currentSelector.GetSelectorTransform().TransformDirection(controllerAxis));
                        finalRotation = Quaternion.AngleAxis(controllerAngle, objectAxis);

                    } else
                    {
                        // rotation by controllers translation
                        //Vector3 directionChange = selectorTransform.position - selectorTransformOffset_position;
                        //Vector3 rotationAxis = Vector3.Cross(directionChange, selectorTransform.up);
                        Vector3 directionChange = currentSelector.GetEndPointPosition() - selectorTransformOffset_position;

                        Vector3 rotationAxis;
                        if (pivot != null)
                        {
                            rotationAxis = Vector3.Cross(directionChange, currentSelector.GetSelectorTransform().position - pivot.position);

                        }
                        else
                        {
                            rotationAxis = Vector3.Cross(directionChange, currentSelector.GetSelectorTransform().position - targetTransform.position);
                        }

                        

                        float controllerAngle = -translationScale * directionChange.magnitude;
                        finalRotation = Quaternion.AngleAxis(controllerAngle, rotationAxis);

                    }

                    
                    finalRotation = Quaternion.Euler(
                            xAxis ? finalRotation.eulerAngles.x : 0,
                            yAxis ? finalRotation.eulerAngles.y : 0,
                            zAxis ? finalRotation.eulerAngles.z : 0);


                    //Vector3 rotation = getControllerChange();
                    // clamping rotation
                    if (constraintMovement)
                    {
                        Quaternion futureRotation = targetTransform.localRotation * finalRotation;


                        float angleDifference = -GetSignedAngle(futureRotation, originalRotation, targetTransform.TransformDirection(rotateAround));
                        if (angleDifference <= maxRotation && angleDifference >= minRotation)
                        {
                            // within range, allow rotation
                            targetTransform.rotation = targetTransform.rotation * finalRotation;
                        }
                        
                    } else
                    {
                        targetTransform.rotation = targetTransform.rotation * finalRotation;
                    }

                }
            } else {
                // not grabbed
                if(springToOrigin)
                {
                    targetTransform.position = Vector3.Lerp(targetTransform.position,originalPosition,Time.deltaTime * springSpeed);
                    targetTransform.rotation = Quaternion.Lerp(targetTransform.rotation,originalRotation,Time.deltaTime * springSpeed);
                }
            }
            
        }

        public float GetSignedAngle(Quaternion A, Quaternion B, Vector3 axis)
        {
            float angle = 0f;
            Vector3 angleAxis = Vector3.zero;
            (Quaternion.Inverse(A) * B).ToAngleAxis(out angle, out angleAxis);
            if (Vector3.Angle(axis, angleAxis) > 90f)
            {
                angle = -angle;
            }
            return Mathf.DeltaAngle(0f, angle);
        }

        protected virtual void ApplyMovement(Vector3 move)
        {
            targetTransform.Translate(move, Space.Self);
            if (constraintMovement)
            {
                Vector3 position = targetTransform.localPosition;
                if(xAxis) position.x = Mathf.Clamp(position.x, minMovement.x, maxMovement.x);
                if (yAxis) position.y = Mathf.Clamp(position.y, minMovement.y, maxMovement.y);
                if (zAxis) position.z = Mathf.Clamp(position.z, minMovement.z, maxMovement.z);
                targetTransform.localPosition = position;
            }
        }

        /*protected virtual Vector3 RestrictMovement(Vector3 move)
        {
            return move;
        }*/

        public void SetOrigins()
        {
            originalPosition = targetTransform.position;
            originalRotation = targetTransform.rotation;
        }

        private void snapObjectToPosition()
        {
            if(snapToClosest)
            {
                // snap to the closest snapping position
                float currentDistance = snapToOrigin ? Vector3.Distance(targetTransform.position, originalPosition) : int.MaxValue;
                int currentSnapPosition = snapToOrigin ? -1 : -2;
                for (int ii=0; ii < snapPositions.Count; ii++)
                {
                    float dist = Vector3.Distance(snapPositions[ii].position, targetTransform.position);
                    if(dist < currentDistance)
                    {
                        currentDistance = dist;
                        currentSnapPosition = ii;
                    }
                }
                if(currentSnapPosition == -1)
                {
                    targetTransform.position = originalPosition;
                    targetTransform.rotation = originalRotation;
                } else if(currentSnapPosition >= 0) 
                {
                    targetTransform.position = snapPositions[currentSnapPosition].position;
                    targetTransform.rotation = snapPositions[currentSnapPosition].rotation;
                }
            } else
            {
                // snap only if near a snapping position
                if (snapToOrigin && Vector3.Distance(targetTransform.position, originalPosition) <= snapRadius)
                {
                    targetTransform.position = originalPosition;
                    targetTransform.rotation = originalRotation;
                }

                foreach (Transform t in snapPositions)
                {
                    if (Vector3.Distance(targetTransform.position, t.position) <= snapRadius)
                    {
                        targetTransform.position = t.position;
                        targetTransform.rotation = t.rotation;
                    }
                }
            }
            
            
        }

        private void loadDefaultColours()
        {
            colourList.Clear();
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
            {
                if (r.material.HasProperty("_Color"))
                {
                    colourList.Add(r.material.color);

                }
            }
        }

        private void paintColours(bool defaultColour)
        {
            if (!useGrabColour) return;

            Renderer[] rends = GetComponentsInChildren<Renderer>();
            for (int ii=0; ii < rends.Length; ii++)
            {
                try
                {
                    if (rends[ii].material.HasProperty("_Color"))
                        rends[ii].material.color = defaultColour ? colourList[ii] : grabColour;
#pragma warning disable 0168
                }
                catch (System.Exception e)
#pragma warning restore 0168
                {
                    // ignore ghost old out of range
                }
            }
        }
        
    }
}