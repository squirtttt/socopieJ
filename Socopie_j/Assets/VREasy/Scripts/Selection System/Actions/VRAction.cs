using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace VREasy
{
    [System.Serializable]
    public abstract class VRAction : MonoBehaviour
    {
        public float delay = 0.0f;
        public abstract void Trigger(); // must be overriden, determines the action to be triggered by the VRSelectable
        
    }
}
