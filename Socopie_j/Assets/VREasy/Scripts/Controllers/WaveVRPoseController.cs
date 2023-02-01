using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VREASY_WAVEVR_SDK
using wvr;
#endif

namespace VREasy
{
    // controls the pose of a controller (device) wen using WaveVR SDK
    public class WaveVRPoseController : MonoBehaviour
    {
#if VREASY_WAVEVR_SDK
        public WaveVR_Controller.EDeviceType device = WaveVR_Controller.EDeviceType.Dominant;
#endif

        // Update is called once per frame
        void Update()
        {
#if VREASY_WAVEVR_SDK
            if(!WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).connected) return;

            WaveVR_Utils.RigidTransform waveTransform = WaveVR_Controller.Input(device).transform;
            transform.position = waveTransform.pos;
            transform.rotation = waveTransform.rot;
#endif
        }
    }
}
