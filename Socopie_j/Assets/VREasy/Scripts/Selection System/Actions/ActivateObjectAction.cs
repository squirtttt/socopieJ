using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class ActivateObjectAction : VRAction
    {
        public GameObject[] targets = new GameObject[0];
        public bool[] array_toggle = new bool[0];
        public bool[] array_activate = new bool[0];

        public bool toggle = true;
        public bool activate = false;



        public ACTIVATION_OPTION[] optionfield = new ACTIVATION_OPTION[0];

        public override void Trigger()
        {
            // 

            if ((targets.Length > 0) && (optionfield.Length == 0))
            {
                optionfield = new ACTIVATION_OPTION[targets.Length];
                for (int i = 0; i < optionfield.Length; i++)
                {
                    if (toggle == true)
                    {
                        optionfield[i] = ACTIVATION_OPTION.Toggle;
                    }
                    else if (activate == true)
                    {
                        optionfield[i] = ACTIVATION_OPTION.Enable;
                    }
                    else
                    {
                        optionfield[i] = ACTIVATION_OPTION.Disable;
                    }
                }

            }

            for (int i = 0; i < targets.Length; i++)
            {
                GameObject target = targets[i];
                if (optionfield[i] == ACTIVATION_OPTION.Toggle)
                {
                    Debug.Log("Triggered : " + targets[i].name + " toggled");
                    target.SetActive(!target.activeInHierarchy);
                }
                else if (optionfield[i] == ACTIVATION_OPTION.Enable)
                {
                    Debug.Log("Triggered : " + targets[i].name + " activated");
                    target.SetActive(true);
                }
                else
                {
                    Debug.Log("Triggered : " + targets[i].name + " de-activated");
                    target.SetActive(false);
                }
            }
        }
    }

}
