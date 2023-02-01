using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace VREasy
{
    [System.Serializable]
    public enum BooleanOperator { AND, OR };

    [ExecuteInEditMode]
    public class CombinationTrigger : VRGrabTrigger
    {
        public VRGrabTrigger[] TriggerList;
        public BooleanOperator booleanOperator =  BooleanOperator.OR;
        public VRGrabTrigger empty;

        private bool TriggeredAND()
        {
            bool triggered = true;
            foreach (VRGrabTrigger trigger in TriggerList)
            {
                triggered = triggered && trigger.Triggered();
            }

            return triggered;
        }

        private bool TriggeredOR()
        {
            bool triggered = false;
            foreach (VRGrabTrigger trigger in TriggerList)
            {
                triggered = triggered || trigger.Triggered();
            }

            return triggered;
        }

        public override bool Triggered()
        {
            switch (booleanOperator)
            {
                case BooleanOperator.AND:
                    return TriggeredAND();
                    
                case BooleanOperator.OR:
                    return TriggeredOR();
                default:
                    return false;
            }
        }
    }
}
