using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRToggleGroup))]
    public class VRToggleGroupEditor : Editor
    {

        //SerializedProperty grabColour;

        private void OnEnable()
        {
            //grabColour = serializedObject.FindProperty("");
        }

        [MenuItem("VREasy/Components/VRToggleGroup")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<VRToggleGroup>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add a VR Toggle Group you must select a game object in the hierarchy first", "OK");
            }
        }

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

            VRToggleGroup toggleGroup = (VRToggleGroup)target;

            DrawDefaultInspector();
            //serializedObject.Update();

            //EditorGUILayout.PropertyField(grabColour);

            //serializedObject.ApplyModifiedProperties();
        }
    }
}