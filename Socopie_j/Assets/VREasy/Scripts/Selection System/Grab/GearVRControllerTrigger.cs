using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{
    public class GearVRControllerTrigger : VRGrabTrigger
    {
        // Mapping done according to https://developer.oculus.com/documentation/unity/latest/concepts/unity-ovrinput/
#if VREASY_OCULUS_UTILITIES_SDK
        public OVRInput.Controller controller = OVRInput.Controller.Active;
        public GEARVR_CONTROLLER_INPUT button;
#endif

        public override bool Triggered()
        {
#if VREASY_OCULUS_UTILITIES_SDK
            switch(button)
            {
                case GEARVR_CONTROLLER_INPUT.TOUCHPAD_PRESS:
                    return OVRInput.Get(OVRInput.Button.PrimaryTouchpad, controller);
                case GEARVR_CONTROLLER_INPUT.TOUCHPAD_TOUCH:
                    return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad, controller);
                case GEARVR_CONTROLLER_INPUT.BACK_BUTTON:
                    return OVRInput.Get(OVRInput.Button.Back, controller);
                case GEARVR_CONTROLLER_INPUT.INDEX_TRIGGER:
                    return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller);
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