using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(UnityEventAction))]
    public class UnityEventActionEditor : Editor
    {
        SerializedProperty unityEvent;

        private void OnEnable()
        {
            unityEvent = serializedObject.FindProperty("unityEvent");

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

            UnityEventAction teleport = (UnityEventAction)target;

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("When this action is triggered, VREasy will call all registered callback functions attached to this Unity Event", MessageType.Info);
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(unityEvent);

            serializedObject.ApplyModifiedProperties();


        }
    }

}