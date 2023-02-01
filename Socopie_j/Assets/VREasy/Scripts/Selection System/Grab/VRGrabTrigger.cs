using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    public abstract class VRGrabTrigger : MonoBehaviour
    {
        public abstract bool Triggered();

#if UNITY_EDITOR

        public static List<string> assemblyNames = new List<string>();
        public static List<string> names = new List<string>();

        private static int grabIndex = 0;

        public static void DisplayGrabTriggerSelector(ref VRGrabTrigger grabTrigger, ref GameObject obj)
        {
            if (grabTrigger == null)
            {
                // add grab trigger
                EditorGUILayout.HelpBox("Trigger not configured. Link a trigger to be able to interact with VRGrabbable and VR elements", MessageType.Error);
                EditorGUI.BeginChangeCheck();
                grabTrigger = (VRGrabTrigger)EditorGUILayout.ObjectField("Trigger", grabTrigger, typeof(VRGrabTrigger), true);
                EditorGUILayout.BeginHorizontal();
                VREasy_utils.LoadClassesFromAssembly(typeof(VRGrabTrigger), ref assemblyNames, ref names);
                grabIndex = EditorGUILayout.Popup("Add", grabIndex, names.ToArray());

                Handles.BeginGUI();
                VRGrabTrigger trigger = null;
                bool linkTrigger = (GUILayout.Button("Link trigger"));
                Handles.EndGUI();
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(obj, "Linked trigger");
                    if (linkTrigger)
                    {
                        trigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(obj, assemblyNames[grabIndex]);
                        grabTrigger = trigger;
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                grabTrigger = (VRGrabTrigger)EditorGUILayout.ObjectField("Associated trigger", grabTrigger, typeof(VRGrabTrigger), true);
                Handles.BeginGUI();
                bool destroy = GUILayout.Button("Remove trigger");
                Handles.EndGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(grabTrigger, "Removed trigger");
                    if (destroy)
                    {
                        DestroyImmediate(grabTrigger);
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }
#endif
    }
}