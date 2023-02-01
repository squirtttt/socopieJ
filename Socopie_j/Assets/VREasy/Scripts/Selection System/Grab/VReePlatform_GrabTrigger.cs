using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class VReePlatform_GrabTrigger : VRGrabTrigger
    {
#if VREASY_VREE_PLATFORM_SDK
        public HandBehaviour HandBehaviour
        {
            get
            {
                if (handBehaviour == null)
                {
                    handBehaviour = gameObject.GetComponent<HandBehaviour>();
                }
                return handBehaviour;
            }
        }

        private HandBehaviour handBehaviour;
        
#endif

        public override bool Triggered()
        {
#if VREASY_VREE_PLATFORM_SDK
            try
            {
                return HandBehaviour.IsHandClosed();
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