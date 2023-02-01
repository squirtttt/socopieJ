using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace VREasy
{
    [CustomEditor(typeof(VRSlider))]
    public class VRSliderEditor : Editor
    {

        private static GameObject sliderTarget;

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
            VRSlider slider = (VRSlider)target;

            ConfigureSlider(ref slider);
        }

        public static void ConfigureSlider(ref VRSlider slider)
        {
            float min = EditorGUILayout.FloatField("Min value", slider.min);
            float max = EditorGUILayout.FloatField("Max value", slider.max);
            bool valid = min <= max;
            slider.min = valid ? min : slider.min;
            slider.max = valid ? max : slider.max;

            float value = EditorGUILayout.Slider("Slider value (%)", slider.value, 0, 1);
            slider.SetValue(value);
            EditorGUILayout.LabelField("Actual value: " + slider.GetRealValue());

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Broadcast target", EditorStyles.boldLabel);
            if (slider.targetComponent == null || string.IsNullOrEmpty(slider.targetProperty))
            {
                EditorGUILayout.LabelField("No broadcast target linked");
            }
            else
            {
                EditorGUILayout.LabelField("Target component: " + slider.targetComponent);
                EditorGUILayout.LabelField("Target property: " + slider.targetProperty);
                if(!string.IsNullOrEmpty(slider.targetField))
                    EditorGUILayout.LabelField("Target field: " + slider.targetField);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Modify linked target", EditorStyles.boldLabel);
            GameObject st = (GameObject)EditorGUILayout.ObjectField("Target object", sliderTarget, typeof(GameObject), true);

            if (st == null)
            {
                components_list.Clear();
                componentNames_list.Clear();
                properties.Clear();
                props_within.Clear();
                return;
            }

            if (sliderTarget != st)
            {
                sliderTarget = st;
                VREasy_utils.LoadComponents(sliderTarget, ref components_list, ref componentNames_list);
                componentIndex = -1;
            }

            EditorGUILayout.Separator();
            // add VRMaterialExposer and select it
            Handles.BeginGUI();
            if (GUILayout.Button("Add VRMaterialExposer to target"))
            {
                addAndSelectComponent<VRMaterialPropertyExposer>();
            }
            Handles.EndGUI();

            // add VRAnimatorExposer and select it
            Handles.BeginGUI();
            if (GUILayout.Button("Add VRAnimatorExposer to target"))
            {
                addAndSelectComponent<VRAnimatorExposer>();
            }
            Handles.EndGUI();

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
                if(pi != propertyIndex)
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
                        EditorGUILayout.HelpBox("The target material does not contain property " + properties[propertyIndex] + ". (not using default materials?). Use Custom Properties on VRMaterialExposer instead and set the custom property name.", MessageType.Error);
                        Debug.Log("[VREasy] VRSliderEditor: invalid material property: " + e.ToString());
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
                    if (GUILayout.Button("Link property"))
                    {
                        slider.targetComponent = components_list[componentIndex];
                        slider.targetProperty = properties[propertyIndex];
                        slider.targetField = props_within.Count > 0 ? props_within[propwithinIndex] : "";
                        sliderTarget = null;
                    }
                    Handles.EndGUI();
                }
            }
            /*if (properties.Count > 0)
            {
                propertyIndex = EditorGUILayout.Popup("Target property", propertyIndex, properties.ToArray());
                // handle complex properties such as Vectors (fields within properties)
                FieldInfo[] props_within = components_list[componentIndex].GetType().GetProperty(properties[propertyIndex]).PropertyType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach(FieldInfo p in props_within)
                {
                    if (p.FieldType == typeof(float) || p.FieldType == typeof(double))
                        Debug.Log(p.Name);
                }
                // link target
                Handles.BeginGUI();
                if (GUILayout.Button("Link target"))
                {
                    slider.targetComponent = components_list[componentIndex];
                    slider.targetProperty = properties[propertyIndex];
                    sliderTarget = null;
                }
                Handles.EndGUI();
            }*/
        }

        private static void addAndSelectComponent<T>() where T : Component
        {
            T exp = sliderTarget.AddComponent<T>();
            VREasy_utils.LoadComponents(sliderTarget, ref components_list, ref componentNames_list);
            componentIndex = -1;
            for (int ii = 0; ii < components_list.Count; ii++)
            {
                if (components_list[ii].GetType() == typeof(T) && (T)components_list[ii] == exp)
                {
                    componentIndex = ii;
                    break;
                }
            }
            VREasy_utils.LoadPropertiesFromComponent(components_list[componentIndex], ref properties);
            propertyIndex = -1;
            props_within.Clear();
        }



    }
}