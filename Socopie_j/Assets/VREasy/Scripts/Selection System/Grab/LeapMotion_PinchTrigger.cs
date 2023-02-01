using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class LeapMotion_PinchTrigger : VRGrabTrigger
    {
#if VREASY_LEAPMOTION_SDK
        public Leap.Unity.PinchDetector Pinch
        {
            get
            {
                if(_pinch == null)
                {
                    _pinch = gameObject.AddComponent<Leap.Unity.PinchDetector>();
                }
                return _pinch;
            }
        }

        private Leap.Unity.PinchDetector _pinch;
        
#endif

        public override bool Triggered()
        {
#if VREASY_LEAPMOTION_SDK
            try
            {
                return Pinch.IsPinching;
#pragma warning disable 0168
            } catch(System.Exception e)
#pragma warning restore 0168
            {
                return false;
            }
#else
            return false;
#endif
        }
    }
}