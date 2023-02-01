using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class KeyboardGrab : VRGrabTrigger
    {
        public KeyCode key;

        public override bool Triggered()
        {
            return Input.GetKey(key);
        }
    }
}