using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace VREasy
{
    [CustomEditor(typeof(ObservableTriggerActionList))]
    [CanEditMultipleObjects]
    public class ObservableTriggerActionListEditor : Editor
    {
        [MenuItem("VREasy/Components/Observed trigger actions")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<ObservableTriggerActionList>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add an observed trigger action you must select a game object in the hierarchy first", "OK");
            }
        }

        private static GameObject targetObject;

        private static List<Component> components_list = new List<Component>();
        private static List<string> componentNames_list = new List<string>();
        private static List<string> properties = new List<string>();

        private static List<string> props_within = new List<string>();
        private static int propwithinIndex = 0;

        private static int componentIndex = 0;
        private static int propertyIndex = 0;

        bool handleRepaintErrors = false;
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

            ObservableTriggerActionList observable = (ObservableTriggerActionList)target;

            ConfigureObservableTriggerActionList(ref observable, targets);
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Trigger conditions", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Actions will be triggered once the observed value enters the following range", EditorStyles.wordWrappedLabel);
            //EVENT_ACTION_COMPARATOR comparator = (EVENT_ACTION_COMPARATOR)EditorGUILayout.EnumPopup("Comparator", observable.comparator);
            float minimumTargetRange = EditorGUILayout.FloatField("Minimum", observable.targetValueRange.x);
            float maximumTargetRange = EditorGUILayout.FloatField("Maximum", observable.targetValueRange.y);
            Vector2 targetValueRange = new Vector2(minimumTargetRange,maximumTargetRange);
            bool fireOnce = EditorGUILayout.Toggle("Fire once", observable.fireOnce);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Observing a property can be computationally intensive, it is recommended not to do it every frame (the higher the skip value, the better the performance", EditorStyles.wordWrappedLabel);
            int skipFrames = EditorGUILayout.IntField("Skipped frames", observable.skipFrames);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (ObservableTriggerActionList ob in targets)
                {
                    Undo.RecordObject(ob, "");
                    ob.fireOnce = fireOnce;
                    ob.targetValueRange = targetValueRange;
                    //ob.comparator = comparator;
                    ob.skipFrames = skipFrames;
                }
                
            }

            

            if(targets.Length == 1)
            {
                EditorGUILayout.Separator();
                VRSelectableEditor.DisplayActionList(observable.actionList, new Object[] { observable });
            }

        }

        public static void ConfigureObservableTriggerActionList(ref ObservableTriggerActionList observable, Object[] targets)
        {
            

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Observed target", EditorStyles.boldLabel);
            if (observable.targetComponent == null || string.IsNullOrEmpty(observable.targetProperty))
            {
                EditorGUILayout.LabelField("No observed target linked");
            }
            else
            {
                EditorGUILayout.LabelField("Target component: " + observable.targetComponent);
                EditorGUILayout.LabelField("Target property: " + observable.targetProperty);
                if (!string.IsNullOrEmpty(observable.targetField))
                    EditorGUILayout.LabelField("Target field: " + observable.targetField);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Modify observed target",EditorStyles.boldLabel);
            GameObject st = (GameObject)EditorGUILayout.ObjectField("Target object", targetObject, typeof(GameObject), true);

            if (st == null)
            {
                components_list.Clear();
                componentNames_list.Clear();
                properties.Clear();
                props_within.Clear();
                return;
            }

            if (targetObject != st)
            {
                targetObject = st;
                VREasy_utils.LoadComponents(targetObject, ref components_list, ref componentNames_list);
                componentIndex = -1;
            }

            EditorGUILayout.Separator();
            int ci = EditorGUILayout.Popup("Component", componentIndex > 0 ? componentIndex : 0, componentNames_list.ToArray());
            if (ci != componentIndex)
            {
                componentIndex = ci;
                VREasy_utils.LoadPropertiesFromComponent(components_list[componentIndex], ref properties);
                propertyIndex = -1;
                props_within.Clear();
            }
            if (properties.Count > 0)
            {
                int pi = EditorGUILayout.Popup("Target property", propertyIndex, properties.ToArray());
                if (pi != propertyIndex)
                {
                    propertyIndex = pi;
                    // load properties within
                    // handle complex properties such as Vectors (fields within properties)
                    props_within.Clear();
                    PropertyInfo prop = components_list[componentIndex].GetType().GetProperty(properties[propertyIndex]);
                    if (prop.PropertyType == typeof(Vector2) || prop.PropertyType != typeof(Vector3) || prop.PropertyType != typeof(Vector4) || prop.PropertyType != typeof(Color))
                    {
                        FieldInfo[] ps = prop.PropertyType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        foreach (FieldInfo p in ps)
                        {
                            if (p.Name == "x" || p.Name == "y" || p.Name == "z" || p.Name == "w" || p.Name == "r" || p.Name == "g" || p.Name == "b" || p.Name == "a") //p.FieldType == typeof(float) || p.FieldType == typeof(double))
                                props_within.Add(p.Name);
                        }
                    }

                }
                if (propertyIndex >= 0) // check if selected material property actually exists in the material
                {
                    try
                    {
                        components_list[componentIndex].GetType().GetProperty(properties[propertyIndex]).GetValue(components_list[componentIndex], null);
                    }
                    catch (TargetInvocationException e)
                    {
                        EditorGUILayout.HelpBox("The target object does not contain property " + properties[propertyIndex] + ". (not using default materials?). Use Custom Properties on VRMaterialExposer instead and set the custom property name.", MessageType.Error);
                        Debug.LogError("[VREasy] ObservableTriggerActionList: invalid property: " + e.ToString());
                        return;
                    }
                }

                if (props_within.Count > 0)
                {
                    propwithinIndex = EditorGUILayout.Popup("Sub property", propwithinIndex, props_within.ToArray());
                }

                // link target
                if (propertyIndex >= 0)
                {
                    Handles.BeginGUI();
                    if (GUILayout.Button("Observe property"))
                    {
                        foreach(ObservableTriggerActionList o in targets)
                        {
                            o.targetComponent = components_list[componentIndex];
                            o.targetProperty = properties[propertyIndex];
                            o.targetField = props_within.Count > 0 ? props_within[propwithinIndex] : "";
                        }
                        targetObject = null;
                    }
                    Handles.EndGUI();
                }
            }
            
        }
    }
}