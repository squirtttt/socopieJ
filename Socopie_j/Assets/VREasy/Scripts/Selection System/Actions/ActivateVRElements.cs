using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class ActivateVRElements : VRAction
    {
        public VRElement[] targets = new VRElement[0];
        public ACTIVATION_OPTION[] options = new ACTIVATION_OPTION[0];

        public bool toggle = true;
        public bool activate = false;

        private int index_counter = 0;

        public override void Trigger()
        {
            // Conversion from old system
            if (targets.Length != options.Length)
            {
                options = new ACTIVATION_OPTION[targets.Length];
                for (int i = 0; i < options.Length; i++)
                {
                    if (toggle)
                    {
                        options[i] = ACTIVATION_OPTION.Toggle;

                    }
                    else
                    {
                        if (activate)
                        {
                            options[i] = ACTIVATION_OPTION.Enable;
                        }
                        else
                        {
                            options[i] = ACTIVATION_OPTION.Disable;
                        }
                    }
                }
            }


            index_counter = 0;
            foreach (VRElement t in targets)
            {
                if (options[index_counter] == ACTIVATION_OPTION.Toggle)
                {
                    if (t.active) t.DeactivateElement();
                    else t.ReactivateElement();
                }
                else
                {
                    if (options[index_counter] == ACTIVATION_OPTION.Enable)
                    {
                        t.ReactivateElement();
                    }
                    else
                    {
                        t.DeactivateElement();
                    }
                }

                index_counter++;

            }
        }
    }

}