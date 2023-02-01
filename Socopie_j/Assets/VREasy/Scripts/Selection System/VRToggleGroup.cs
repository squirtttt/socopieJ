using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{
    public class VRToggleGroup : MonoBehaviour
    {
        public List<VRToggle> toggles;

        // Use this for initialization
        void OnEnable()
        {
            foreach(VRToggle toggle in toggles)
            {
                toggle.activationEvent += onToggleActivated;
                toggle.ActivateIdle();
            }
        }

        // Update is called once per frame
        void OnDisable()
        {
            foreach (VRToggle toggle in toggles)
            {
                toggle.activationEvent -= onToggleActivated;
            }
        }

        public void onToggleActivated(VRToggle activatedToggle)
        {
            foreach (VRToggle toggle in toggles)
            {
                // search for all other toggles and deactivate them
                if (activatedToggle != toggle)
                {
                    toggle.ActivateIdle();
                } 
            }
        }


    }
}