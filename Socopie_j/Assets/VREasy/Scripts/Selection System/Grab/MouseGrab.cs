using UnityEngine;
using System.Collections;
using System;

namespace VREasy {
    public class MouseGrab : VRGrabTrigger
    {
        public int mouseButton = 0;
        public override bool Triggered()
        {
            return Input.GetMouseButton(mouseButton);
        }
    }
}