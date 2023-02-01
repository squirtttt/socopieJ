using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class VRToggle : VR2DButton
    {
        public delegate void onActivation(VRToggle toggle);
        public event onActivation activationEvent;

        public bool toggle_state = false;

        public ActionList IdleActions
        {
            get
            {
                if (idleActions == null)
                {
                    Transform ct = transform.Find("IdleToggleActions");
                    if(ct == null)
                    {
                        GameObject child = new GameObject("IdleToggleActions");
                        child.transform.parent = transform;
                        idleActions = child.AddComponent<ActionList>();
                    } else
                    {
                        idleActions = ct.GetComponent<ActionList>();
                    }
                    
                }
                return idleActions;
            }
        }
        public ActionList SelectActions
        {
            get
            {
                if (selectActions == null)
                {
                    Transform ct = transform.Find("SelectToggleActions");
                    if (ct == null)
                    {
                        GameObject child = new GameObject("SelectToggleActions");
                        child.transform.parent = transform;
                        selectActions = child.AddComponent<ActionList>();
                    }
                    else
                    {
                        selectActions = ct.GetComponent<ActionList>();
                    }
                }
                return selectActions;
            }
        }
        public ActionList idleActions;
        public ActionList selectActions;

        protected override void Trigger()
        {
            // trigger one action list depending on the state
            if (toggle_state) ActivateIdle();
            else ActivateSelect();
        }

        protected override void SetState()
        {
            // switch between states
            if (toggle_state)
            {
                setSprite(selectIcon);
            } else
            {
                setSprite(idleIcon);
            }
            
            
        }

        public void ActivateIdle()
        {
            idleActions.Trigger();
            toggle_state = false;
        }

        public void ActivateSelect()
        {
            selectActions.Trigger();
            toggle_state = true;
            if (activationEvent != null) activationEvent(this);
        }
    }
}