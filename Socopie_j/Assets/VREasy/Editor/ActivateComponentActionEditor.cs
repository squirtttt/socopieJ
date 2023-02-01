using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace VREasy
{
    [CustomEditor(typeof(ActivateComponentAction))]
    public class ActivateComponentActionEditor : Editor
    {
        private List<Component> components_list = new List<Component>();
        private List<string> componentNames_list = new List<string>();
        private int componentIndex = 0;
        private bool remove_element = false;

        bool handleRepaintErrors = false;

        void OnEnable()
        {
            // When editor is viewed
            ActivateComponentAction action = (ActivateComponentAction)target;

            var serializedObject = new SerializedObject(target);
            var serialized_object_array = serializedObject.FindProperty("object_target");
            var serialized_target_array = serializedObject.FindProperty("component_target");
            var serialized_option_array = serializedObject.FindProperty("optionfield");
            var serialized_index_array = serializedObject.FindProperty("component_index");
            serializedObject.Update();

            if (action.component != null)
            {
                action.component_target = new Component[1];
                action.object_target = new GameObject[1];
                action.component_index = new int[1];
                action.optionfield = new ACTIVATION_OPTION[1];

                action.component_target[0] = action.component;
                action.optionfield[0] = ACTIVATION_OPTION.Toggle;
                action.object_target[0] = action.component.gameObject;


                List<Component> components_list = new List<Component>();
                List<string> componentNames_list = new List<string>();
                VREasy_utils.LoadComponents(action.object_target[0], ref components_list, ref componentNames_list);

                action.component_index[0] = componentNames_list.FindIndex(action.component_target[0].ToString().StartsWith);

                action.component = null;
            }

            // Makes sure the toggle/activate/deactivate system is updated to current version
            if ((serialized_target_array.arraySize > 0) && ((serialized_option_array.arraySize != serialized_target_array.arraySize)
                || (serialized_object_array.arraySize != serialized_target_array.arraySize) || (serialized_index_array.arraySize != serialized_target_array.arraySize)))
            {

                action.optionfield = new ACTIVATION_OPTION[serialized_target_array.arraySize];
                action.object_target = new GameObject[serialized_target_array.arraySize];
                action.component_index = new int[serialized_target_array.arraySize];

                var updated_serialized_option_array = serializedObject.FindProperty("optionfield");

                for (int i = 0; i < updated_serialized_option_array.arraySize; i++)
                {
                    action.optionfield[i] = ACTIVATION_OPTION.Toggle;
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

            ActivateComponentAction action = (ActivateComponentAction)target;


            var serializedObject = new SerializedObject(action);
            var serialized_component_array = serializedObject.FindProperty("component_target");
            var serialized_object_array = serializedObject.FindProperty("object_target");
            var serialized_option_array = serializedObject.FindProperty("optionfield");
            var serialized_index_array = serializedObject.FindProperty("component_index");

            serializedObject.Update();


            if (action.component_target.Length > 0)
            {
                for (int i = 0; i < serialized_component_array.arraySize; i++)
                {
                    GameObject current_obj = action.object_target[i];

                    VREasy_utils.LoadComponents(current_obj, ref components_list, ref componentNames_list);

                    if (action.component_target[i] != null)
                    {
                        List<string> find_names_list = new List<string>();
                        for (int j = 0; j < components_list.ToArray().Length; j++)
                        {
                            find_names_list.Add(components_list[j].ToString());
                        }
                        componentIndex = find_names_list.FindIndex(action.component_target[i].ToString().StartsWith);

                        if (componentIndex < 0)
                        {
                            componentIndex = 0;
                        }
                        serialized_index_array.GetArrayElementAtIndex(i).intValue = componentIndex;
                    }
                    else
                    {
                        componentIndex = 0;
                    }

                    // Drawing the GUI
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                    EditorGUIUtility.labelWidth = 70f;
                    EditorGUILayout.PropertyField(serialized_object_array.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUILayout.PropertyField(serialized_option_array.GetArrayElementAtIndex(i), GUIContent.none);
                    componentIndex = EditorGUILayout.Popup("Component", serialized_index_array.GetArrayElementAtIndex(i).intValue, componentNames_list.ToArray());
                    EditorGUIUtility.labelWidth = 0f;

                    if (GUILayout.Button("-", GUILayout.Width(20))) // Button that removes the selected target
                    {
                        remove_element = true;
                    }
                    GUILayout.EndHorizontal();

                    if (action.object_target[i] != null)
                    {
                        action.component_target[i] = components_list[componentIndex];
                    }

                    // removing a target
                    if (remove_element)
                    {
                        // shifts entire array up
                        if (i < serialized_component_array.arraySize - 1)
                        {
                            action.component_target[i] = action.component_target[i + 1];
                            action.optionfield[i] = action.optionfield[i + 1];
                            action.object_target[i] = action.object_target[i + 1];
                        }
                        else
                        {
                            Component[] com_array = new Component[action.component_target.Length - 1];

                            for (int j = 0; j < action.component_target.Length - 1; j++)
                            {
                                com_array[j] = action.component_target[j];
                            }

                            action.component_target = com_array;

                            ACTIVATION_OPTION[] option_array = new ACTIVATION_OPTION[action.component_target.Length];

                            for (int j = 0; j < action.component_target.Length; j++)
                            {
                                option_array[j] = action.optionfield[j];

                            }

                            action.optionfield = option_array;

                            GameObject[] obj_array = new GameObject[action.component_target.Length];

                            for (int j = 0; j < action.component_target.Length; j++)
                            {
                                obj_array[j] = action.object_target[j];

                            }

                            action.object_target = obj_array;

                            int[] index_array = new int[action.component_target.Length];

                            for (int j = 0; j < action.component_target.Length; j++)
                            {
                                index_array[j] = action.component_index[j];

                            }

                            action.component_index = index_array;
                        }
                    }
                }
            }

            // Adding a target
            if (GUILayout.Button("Add Target"))
            {
                if (action.component_target.Length == 0)
                {
                    action.component_target = new Component[1];
                    action.object_target = new GameObject[1];
                    action.component_index = new int[1];
                    action.optionfield = new ACTIVATION_OPTION[1];
                }
                else
                {
                    Component[] com_array = new Component[action.component_target.Length + 1];
                    action.component_target.CopyTo(com_array, 0);
                    action.component_target = com_array;

                }

                ACTIVATION_OPTION[] option_array = new ACTIVATION_OPTION[action.component_target.Length];
                action.optionfield.CopyTo(option_array, 0);
                action.optionfield = option_array;

                GameObject[] object_array = new GameObject[action.component_target.Length];
                action.object_target.CopyTo(object_array, 0);
                action.object_target = object_array;

                int[] index_array = new int[action.component_target.Length];
                action.component_index.CopyTo(index_array, 0);
                action.component_index = index_array;
            }

            remove_element = false;

            serializedObject.ApplyModifiedProperties();
            EditorGUI.BeginChangeCheck();
        }
    }
}