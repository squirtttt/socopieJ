using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class GearVR_MobileTapTrigger : VRGrabTrigger
    {
        public override bool Triggered()
        {
            return Input.GetMouseButton(0);
        }
    }
}