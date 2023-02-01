using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRMaterialPropertyExposer))]
    public class VRMaterialPropertyExposerEditor : Editor
    {

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

            VRMaterialPropertyExposer matExposer = (VRMaterialPropertyExposer)target;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            Material mat = (Material)EditorGUILayout.ObjectField("Target material",matExposer.Material,typeof(Material),true);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Custom properties", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("You can target any Color or Number based properties by name", EditorStyles.wordWrappedLabel);
            string customColour = EditorGUILayout.TextField("Custom Colour", matExposer.customColourShaderProperty);
            string customFloat = EditorGUILayout.TextField("Custom Colour", matExposer.customFloatShaderProperty);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(matExposer, "Changed material exposer values");
                matExposer.Material = mat;
                matExposer.customColourShaderProperty = customColour;
                matExposer.customFloatShaderProperty = customFloat;
            }


        }
    }
}