using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(ActivateVRElements))]
    public class ActivateVRelementEditor : Editor
    {

        private bool remove_element = false;
        bool handleRepaintErrors = false;
        public void OnEnable()
        {
            ActivateVRElements elements = (ActivateVRElements)target;
            var serializedObject = new SerializedObject(target);
            serializedObject.Update();
            // Conversion from old system
            if (elements.targets.Length != elements.options.Length)
            {
                elements.options = new ACTIVATION_OPTION[elements.targets.Length];
                for (int i = 0; i < elements.options.Length; i++)
                {
                    if (elements.toggle)
                    {
                        elements.options[i] = ACTIVATION_OPTION.Toggle;

                    }
                    else
                    {
                        if (elements.activate)
                        {
                            elements.options[i] = ACTIVATION_OPTION.Enable;
                        }
                        else
                        {
                            elements.options[i] = ACTIVATION_OPTION.Disable;
                        }
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }

            ActivateVRElements elements = (ActivateVRElements)target;

            EditorGUILayout.Separator();

            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty("targets");
            var serialized_option_array = serializedObject.FindProperty("options");
            serializedObject.Update();

            for (int i = 0; i < property.arraySize; i++)
            {
                //--------------------------------------
                // Start of GUI display
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                EditorGUIUtility.labelWidth = 70f;

                EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), true);
                EditorGUILayout.PropertyField(serialized_option_array.GetArrayElementAtIndex(i), GUIContent.none);

                if (GUILayout.Button("-", GUILayout.Width(20))) // Button that removes the selected target
                {
                    remove_element = true;
                }

                GUILayout.EndHorizontal();

                // End of GUI display
                // ------------------------------------

                if (remove_element)
                {
                    // shifts entire array up
                    if (i < property.arraySize - 1)
                    {
                        elements.targets[i] = elements.targets[i + 1];
                        elements.options[i] = elements.options[i + 1];
                    }
                    else
                    {
                        VRElement[] target_array_tmp = new VRElement[elements.targets.Length - 1];
                        for (int j = 0; j < elements.targets.Length - 1; j++)
                        {
                            target_array_tmp[j] = elements.targets[j];
                        }
                        elements.targets = target_array_tmp;

                        ACTIVATION_OPTION[] option_array_tmp = new ACTIVATION_OPTION[elements.targets.Length];
                        for (int j = 0; j < elements.targets.Length; j++)
                        {
                            option_array_tmp[j] = elements.options[j];
                        }
                        elements.options = option_array_tmp;
                    }
                }
            }

            if (GUILayout.Button("Add Target"))
            {
                if (elements.targets.Length == 0)
                {
                    elements.targets = new VRElement[1];
                    elements.options = new ACTIVATION_OPTION[1];
                }
                else
                {
                    VRElement[] target_array_tmp = new VRElement[elements.targets.Length + 1];
                    elements.targets.CopyTo(target_array_tmp, 0);
                    elements.targets = target_array_tmp;

                    ACTIVATION_OPTION[] option_array_tmp = new ACTIVATION_OPTION[elements.targets.Length];
                    elements.options.CopyTo(option_array_tmp, 0);
                    elements.options = option_array_tmp;

                }
            }

            remove_element = false;

            //



            serializedObject.ApplyModifiedProperties();
        }
        
    }
}