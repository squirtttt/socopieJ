using UnityEngine;
using System.Collections;
using System;
#if VREASY_WAVEVR_SDK
using wvr;
#endif

namespace VREasy
{
    public class WaveVRControllerTrigger : VRGrabTrigger
    {
#if VREASY_WAVEVR_SDK
        public WaveVR_Controller.EDeviceType controller = WaveVR_Controller.EDeviceType.Dominant;
        public WVR_InputId button;
#endif
        public override bool Triggered()
        {
#if VREASY_WAVEVR_SDK
            // this controls if the dominant controller is connected (can be left, if in left-handed mode)
            if (!WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).connected) return false;
            return WaveVR_Controller.Input(controller).GetPress(button);

#else
            return false;
#endif
        }

    }
}