using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VREasy
{
    public class UnityEventAction : VRAction
    {
        public UnityEvent unityEvent;
        
        public override void Trigger()
        {
            unityEvent.Invoke();
        }

    }
}