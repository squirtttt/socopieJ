using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(ResetTransformAction))]
    public class ResetTransformActionEditor : Editor
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
            ResetTransformAction reset = (ResetTransformAction)target;

            EditorGUI.BeginChangeCheck();
            Transform t = (Transform)EditorGUILayout.ObjectField("Target object", reset.target, typeof(Transform), true);
            bool snap = EditorGUILayout.Toggle("Snap to position", reset.snap);
            float speed = reset.interpolationSpeed;
            if (!snap)
            {
                speed = EditorGUILayout.FloatField("Interpolation speed", reset.interpolationSpeed);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(reset.target, "Changed options on reset action");
                reset.target = t;
                reset.snap = snap;
                reset.interpolationSpeed = speed;
            }
            
        }
    }
}