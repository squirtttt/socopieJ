using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class OculusGoControllerTrigger : VRGrabTrigger
    {
        // for button names mapping visit https://docs.unity3d.com/Manual/OculusControllers.html
#if VREASY_OCULUS_UTILITIES_SDK
        private OVRInput.Controller controller = OVRInput.Controller.All;
        public OVRInput.Button button;
        public OVRInput.Touch touch;
#endif
        public OCULUS_CONTROLLER_INPUT_TYPE input_type = OCULUS_CONTROLLER_INPUT_TYPE.BUTTON;

        public override bool Triggered()
        {
#if VREASY_OCULUS_UTILITIES_SDK
            switch (input_type)
            {
                case OCULUS_CONTROLLER_INPUT_TYPE.BUTTON:
                    return OVRInput.Get(button, controller);
                case OCULUS_CONTROLLER_INPUT_TYPE.TOUCH:
                    return OVRInput.Get(touch, controller);
                default:
                    return false;
            }

#else
            return false;
#endif
        }

#if VREASY_OCULUS_UTILITIES_SDK
        private void Update()
        {
            OVRInput.Update();
        }

        private void FixedUpdate()
        {
            OVRInput.FixedUpdate();
        }
#endif
    }
}