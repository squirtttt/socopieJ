using UnityEngine;
using System.Collections;

namespace VREasy
{
    [RequireComponent(typeof(Collider))]
    public class VRTriggerArea : VRSelectable
    {
        public ActionList ExitActions
        {
            get
            {
                if(exitActions == null)
                {
                    GameObject child = new GameObject("ExitTriggerActions");
                    child.transform.parent = transform;
                    exitActions = child.AddComponent<ActionList>();
                }
                return exitActions;
            }
        }

        public ActionList EntryActions
        {
            get
            {
                if (entryActions == null)
                {
                    GameObject child = new GameObject("EntryTriggerActions");
                    child.transform.parent = transform;
                    entryActions = child.AddComponent<ActionList>();
                }
                return entryActions;
            }
        }

        public ActionList exitActions;
        public ActionList entryActions;

        protected override void Initialise()
        {
            coolDownTime = 0.0f;
        }

        public override bool CanSelectWithSight()
        {
            return false;
        }

        public override bool CanSelectWithPointer()
        {
            return false;
        }
        // callback when selectable exiting trigger area
        protected override void Unpressed()
        {
            ExitActions.Trigger();
        }

        // callback when selectable entering trigger area
        protected override void Pressed(VRSelector selector)
        {
            if(!isPressed) EntryActions.Trigger();
        }

        protected override void Trigger()
        {
            // nothing -actions will be triggered when entering / exiting interaction
        }
    }
}