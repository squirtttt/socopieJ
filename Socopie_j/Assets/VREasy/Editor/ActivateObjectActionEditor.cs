using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace VREasy
{

    [CustomEditor(typeof(ActivateObjectAction))]
    public class ActivateObjectActionEditor : Editor
    {
        bool handleRepaintErrors = false;
        int[] selected;
        bool remove_element = false;

        void OnEnable()
        {
            // When editor is viewed
            ActivateObjectAction elements = (ActivateObjectAction)target;

            var serializedObject = new SerializedObject(target);
            var serialized_target_array = serializedObject.FindProperty("targets");
            var serialized_option_array = serializedObject.FindProperty("optionfield");

            // Makes sure the toggle/activate/deactivate system is updated to current version
            if ((serialized_target_array.arraySize > 0) && (serialized_option_array.arraySize != serialized_target_array.arraySize))
            {

                elements.optionfield = new ACTIVATION_OPTION[serialized_target_array.arraySize];
                serializedObject.Update();
                var updated_serialized_option_array = serializedObject.FindProperty("optionfield");

                for (int i = 0; i < updated_serialized_option_array.arraySize; i++)
                {
                    if (elements.toggle == true)
                    {
                        elements.optionfield[i] = ACTIVATION_OPTION.Toggle;
                    }
                    else if (elements.activate == true)
                    {
                        elements.optionfield[i] = ACTIVATION_OPTION.Enable;
                    }
                    else
                    {
                        elements.optionfield[i] = ACTIVATION_OPTION.Disable;
                    }
                }

            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.BeginChangeCheck();

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

            ActivateObjectAction elements = (ActivateObjectAction)target;

            EditorGUILayout.Separator();

            var serializedObject = new SerializedObject(target);
            var serialized_target_array = serializedObject.FindProperty("targets");
            var serialized_option_array = serializedObject.FindProperty("optionfield");

            serializedObject.Update();



            if (elements.targets.Length > 0)
            {
                for (int i = 0; i < serialized_target_array.arraySize; i++)
                {

                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));



                    EditorGUIUtility.labelWidth = 70f;
                    EditorGUILayout.PropertyField(serialized_target_array.GetArrayElementAtIndex(i), GUILayout.Width(200));
                    EditorGUILayout.PropertyField(serialized_option_array.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUIUtility.labelWidth = 0f;

                    if (GUILayout.Button("-", GUILayout.Width(20))) // Button that removes the selected target
                    {
                        remove_element = true;
                    }

                    GUILayout.EndHorizontal();

                    if (remove_element)
                    {
                        //shifts entire array up
                        if (i < serialized_target_array.arraySize - 1)
                        {
                            elements.targets[i] = elements.targets[i + 1];
                            elements.optionfield[i] = elements.optionfield[i + 1];
                        }
                        else
                        {
                            GameObject[] object_array = new GameObject[elements.targets.Length - 1];

                            for (int j = 0; j < elements.targets.Length - 1; j++)
                            {
                                object_array[j] = elements.targets[j];
                            }

                            elements.targets = object_array;
                            ACTIVATION_OPTION[] option_array = new ACTIVATION_OPTION[elements.targets.Length];

                            for (int j = 0; j < elements.targets.Length; j++)
                            {
                                option_array[j] = elements.optionfield[j];

                            }

                            elements.optionfield = option_array;
                        }
                    }
                }
            }

            if (GUILayout.Button("Add Target"))
            {
                if (elements.targets.Length == 0)
                {
                    elements.targets = new GameObject[1];
                }
                else
                {
                    GameObject[] object_array = new GameObject[elements.targets.Length + 1];
                    elements.targets.CopyTo(object_array, 0);
                    elements.targets = object_array;

                }

                ACTIVATION_OPTION[] option_array = new ACTIVATION_OPTION[elements.targets.Length];
                elements.optionfield.CopyTo(option_array, 0);
                elements.optionfield = option_array;
            }

            remove_element = false;
            serializedObject.ApplyModifiedProperties();
            EditorGUI.BeginChangeCheck();
        }

    }
}
