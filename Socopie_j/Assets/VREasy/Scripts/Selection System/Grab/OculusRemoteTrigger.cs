using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class OculusRemoteTrigger : VRGrabTrigger
    {
        // for button names mapping visit https://docs.unity3d.com/Manual/OculusControllers.html
#if VREASY_OCULUS_UTILITIES_SDK
        private OVRInput.Controller controller = OVRInput.Controller.All;
        public OVRInput.Button button;
#endif
        public override bool Triggered()
        {
#if VREASY_OCULUS_UTILITIES_SDK
            return OVRInput.Get(button, controller);
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