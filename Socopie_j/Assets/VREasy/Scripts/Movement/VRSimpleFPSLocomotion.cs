using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if VREASY_STEAM_SDK
using Valve.VR;
#endif
#if VREASY_WAVEVR_SDK
using wvr;
#endif

namespace VREasy
{
    public class VRSimpleFPSLocomotion : MonoBehaviour
    {
        public float speed = 2.0f;
        public Transform head;
        public VRLOCOMOTION_INPUT input = VRLOCOMOTION_INPUT.UNITY_INPUT;
        public float forwardAngle = 30f;
#if VREASY_STEAM_SDK
        public SteamVR_Action_Vector2 directionAction;
        public SteamVR_Input_Sources inputSource;
        //public SteamVR_TrackedObject trackedObject;
        //private SteamVR_Controller.Device device;
#endif
        public VRGrabTrigger trigger;

        public bool fixedHeight = true;
        public bool fixedForward = false;
        public float fixedMovement = 2f;
        public X_AXIS_TYPE xAxisType = X_AXIS_TYPE.TRANSLATE;
        public bool useLeftController = true;
        public bool useRightController = false;
        public bool inverseZaxis = false;
        [Range(0.0f,100.0f)]
        public float deadZone = 0.0f;
        public float stepRotationDelay = 0.5f;
        [Range(0.0f,359.0f)]
        public float stepRotationAngle = 45.0f;

        private float lastStepRotation = 0.0f;

        private bool mobileMoving = false;

        void Awake()
        {

            if (transform == head)
            {
                Debug.LogWarning("VRSimpleLocomotion should not be placed in the HEAD game object but in a parent transform. Automatically fixed.");
                GameObject loc = new GameObject("[VREASY]LocomotionParent");
                //loc.transform.position = Vector3.zero;
                transform.parent = loc.transform;
                VRSimpleFPSLocomotion dest = loc.AddComponent<VRSimpleFPSLocomotion>();
                dest.speed = speed;
                dest.head = head;
                dest.input = input;
                dest.trigger = trigger;
                dest.fixedHeight = fixedHeight;
                dest.fixedForward = fixedForward;
                dest.fixedMovement = fixedMovement;
                dest.xAxisType = xAxisType;
#if VREASY_STEAM_SDK
                //dest.trackedObject = trackedObject;
#endif
                Destroy(this);
            }

#if VREASY_GOOGLEVR_SDK
            if(input == VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER)
            {
                if(FindObjectOfType<GvrControllerInput>() == null)
                {
                    Debug.LogWarning("[VREasy] GvrControllerInput instance not found, adding one for VRSimpleFPSLocomotion");
                    GameObject n = new GameObject("[VREasy] GvrControllerInput");
                    n.AddComponent<GvrControllerInput>();
                }
            }
#endif
        }

        private void OnEnable()
        {
#if VREASY_STEAM_SDK
            SteamVR.Initialize();
#endif
        }

        void Update()
        {
#if VREASY_OCULUS_UTILITIES_SDK
            OVRInput.Update();
#endif
            if (head == null)
                return;
            Vector3 move = Vector3.zero;
            switch (input)
            {
                case VRLOCOMOTION_INPUT.UNITY_INPUT:
                    move = Vector3.right * Input.GetAxis("Horizontal") - Vector3.forward * Input.GetAxis("Vertical");
                    break;
                case VRLOCOMOTION_INPUT.GENERIC_VR_CONTROLLER:
#if UNITY_2019_1_OR_NEWER
                    {
                        var devices = new List<UnityEngine.XR.InputDevice>();
                        if(useLeftController)
                        {
                            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand,
                                                                         devices);
                            Vector2 axisValue;
                            if (devices.Count > 0 && devices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis,
                                                          out axisValue))
                            {
                                move.x += axisValue.x;
                                move.z += axisValue.y;
                            }
                        }
                        devices.Clear();
                        if (useRightController)
                        {
                            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand,
                                                                         devices);
                            Vector2 axisValue;
                            if (devices.Count > 0 && devices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis,
                                                          out axisValue))
                            {
                                move.x += axisValue.x;
                                move.z += axisValue.y;
                            }
                        }
                    }
#else
                    move = Vector3.right * Input.GetAxis("Horizontal") - Vector3.forward * Input.GetAxis("Vertical");
#endif

                    break;
                case VRLOCOMOTION_INPUT.STEAM_CONTROLLER:
                    {
#if VREASY_STEAM_SDK
                        try
                        {
                            //device = SteamVR_Controller.Input((int)trackedObject.index);
                            //Vector2 inp = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
                            
                            Vector2 inp = directionAction.GetAxis(inputSource);
                            move.x = inp.x;
                            move.z = inp.y;
                        }
#pragma warning disable 0168
                        catch (System.Exception _) { }
#pragma warning restore 0168
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.MOBILE_TILT:
                    {
                        if(head.eulerAngles.x >= forwardAngle && head.eulerAngles.x < 90f)
                        {
                            mobileMoving = true;
                        }
                        if(head.eulerAngles.x >= 360f-forwardAngle)
                        {
                            mobileMoving = false;
                        }
                        if(mobileMoving)
                        {
                            move = Vector3.forward;
                        }
                    }
                    break;
                case VRLOCOMOTION_INPUT.TRIGGER:
                    {
                        if(trigger != null)
                        {
                            mobileMoving = (trigger.Triggered());
                            if(mobileMoving)
                            {
                                move = Vector3.forward;
                            }
                        }
                    }
                    break;
                case VRLOCOMOTION_INPUT.OCULUS_CONTROLLER:
                    {
#if VREASY_OCULUS_UTILITIES_SDK
                        Vector2 inp = Vector2.zero;
                        if(useLeftController)
                        {
                            inp += OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                        }
                        if(useRightController)
                        {
                            inp += OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
                        }
                        move.x = inp.x;
                        move.z = inp.y;
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.GEAR_VR_CONTROLLER:
                    {
#if VREASY_OCULUS_UTILITIES_SDK
                        Vector2 inp = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                        move.x = inp.x;
                        move.z = inp.y;
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER:
                    {
#if VREASY_GOOGLEVR_SDK
                        Vector2 inp = Vector2.zero;
                        if(useLeftController)
                        {
                            inp += GvrControllerInput.GetDevice(GvrControllerHand.Left).TouchPos;
                        }
                        if(useRightController)
                        {
                            inp += GvrControllerInput.GetDevice(GvrControllerHand.Right).TouchPos;
                        }
                        //Vector2 inp = GvrControllerInput.IsTouching ? GvrControllerInput.TouchPosCentered : Vector2.zero;
                        move.x = inp.x;
                        move.z = inp.y;
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.WAVEVR:
                    {
#if VREASY_WAVEVR_SDK
                        // this controls if the dominant controller is connected (can be left, if in left-handed mode)
                        if (WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).connected) {
                            Vector2 inp = Vector2.zero;
                            if(useLeftController)
                            {
                                inp += WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Left).GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);
                            }
                            if (useRightController)
                            {
                                inp += WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);
                            }
                            move.x = inp.x;
                            move.z = inp.y;
                        }
#endif
                    }
                    break;
            }

            if (Mathf.Abs(move.x) < deadZone / 100.0) move.x = 0.0f;
            if (Mathf.Abs(move.z) < deadZone / 100.0) move.z = 0.0f;

            switch (xAxisType)
            {
                case X_AXIS_TYPE.STEPPED_ROTATE:
                    {
                        if(lastStepRotation + stepRotationDelay < Time.time) {
                            float rotate = Mathf.Abs(move.x) > 0 ? Mathf.Sign(move.x) * stepRotationAngle : 0;
                            transform.RotateAround(head.transform.position, Vector3.up, rotate);
                            lastStepRotation = Time.time;
                        }
                        if (Mathf.Abs(move.x) < Mathf.Epsilon) lastStepRotation = 0;
                        move.x = 0;

                    }
                    break;
                case X_AXIS_TYPE.ROTATE:
                    {
                        float rotate = move.x;
                        move.x = 0;
                        transform.RotateAround(head.transform.position,Vector3.up,rotate * speed);
                    }
                    break;
            }

            if (fixedForward) move.z = fixedMovement;
            else move *= speed;
            if (inverseZaxis) move.z = -move.z;
            move = head.TransformDirection(move);
            if (fixedHeight)
                move.y = 0;
            transform.position += move * Time.deltaTime;
        }
    }
}