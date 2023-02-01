using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{

    public class TriggeredActions : MonoBehaviour
    {

        // Public Trigger
        public VRGrabTrigger grabTrigger;

        public bool fireOnce = true;

        private bool hasFired = false;

        // Public TriggerList
        public ActionList actionList
        {
            get
            {
                if (_actionList == null)
                {
                    _actionList = GetComponent<ActionList>();
                }
                if (_actionList == null)
                {
                    _actionList = gameObject.AddComponent<ActionList>();
                }
                return _actionList;
            }

        }
        private ActionList _actionList;

        private void Start()
        {
            grabTrigger = GetComponent<VRGrabTrigger>();
        }


        // Update is called once per frame
        void Update()
        {
            if(fireOnce)
            {
                // Trigger Action List
                if (!hasFired && grabTrigger.Triggered())
                {
                    actionList.Trigger();
                    hasFired = true;
                }
                if (!grabTrigger.Triggered()) hasFired = false;
            } else
            {
                // Trigger Action List
                if (grabTrigger.Triggered())
                {
                    actionList.Trigger();
                }
            }
            


        }

    }

}
