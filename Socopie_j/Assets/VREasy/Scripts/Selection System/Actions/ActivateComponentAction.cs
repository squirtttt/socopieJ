using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace VREasy
{
    [System.Serializable]
    public class ActivateComponentAction : VRAction
    {
        public Component[] component_target = new Component[0];
        public GameObject[] object_target;
        public int[] component_index;
        public ACTIVATION_OPTION[] optionfield;

        // component is the old version, its kept here to be used to convert older versions to the new component_target array version
        public Component component;

        public override void Trigger()
        {
            // Used to convert old system into new system
            if (component != null)
            {
                component_target = new Component[1];
                object_target = new GameObject[1];
                component_index = new int[1];
                optionfield = new ACTIVATION_OPTION[1];

                component_target[0] = component;
                optionfield[0] = ACTIVATION_OPTION.Toggle;
                object_target[0] = component.gameObject;


                List<Component> components_list = new List<Component>();
                List<string> componentNames_list = new List<string>();
                VREasy_utils.LoadComponents(object_target[0], ref components_list, ref componentNames_list);

                component_index[0] = componentNames_list.FindIndex(component_target[0].ToString().StartsWith);
            }

            if ((component_target.Length > 0) && (optionfield.Length == 0))
            {
                optionfield = new ACTIVATION_OPTION[component_target.Length];
                for (int i = 0; i < optionfield.Length; i++)
                {
                    optionfield[i] = ACTIVATION_OPTION.Toggle;
                    component_index[i] = 0;
                }

            }
            // End of conversion



            for (int i = 0; i < component_target.Length; i++)
            {
                Component target = component_target[i];
                if (target != null)
                {
                    if (optionfield[i] == ACTIVATION_OPTION.Toggle)
                    {
                        Debug.Log("Toggled : " + target.name);
                        bool val = (bool)target.GetType().GetProperty("enabled").GetValue(target, null);
                        target.GetType().GetProperty("enabled").SetValue(target, !val, null);
                    }
                    else if (optionfield[i] == ACTIVATION_OPTION.Enable)
                    {
                        Debug.Log("Activated : " + target.name);
                        target.GetType().GetProperty("enabled").SetValue(target, true, null);
                    }
                    else
                    {
                        Debug.Log("De-activated : " + target.name);
                        target.GetType().GetProperty("enabled").SetValue(target, false, null);
                    }
                }

            }
        }
        
    }
}