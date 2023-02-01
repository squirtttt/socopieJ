using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace VREasy
{
    [CustomEditor(typeof(TriggeredActions))]
    public class TriggeredActionsEditor : Editor
    {

        [MenuItem("VREasy/Components/Triggered Actions")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<TriggeredActions>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add Triggered Actions you must select a game object in the hierarchy first", "OK");
            }
        }

        // Use this for initialization
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

            TriggeredActions triggeredAction = (TriggeredActions)target;

            ConfigureTriggeredAction(triggeredAction);
        }

        public static void ConfigureTriggeredAction(TriggeredActions triggeredAction)
        {

            EditorGUILayout.HelpBox("Create a trigger condition, that will activate a list of actions when it is triggered.", MessageType.Info);

            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();

            bool fireOnce = EditorGUILayout.Toggle("Fire once", triggeredAction.fireOnce);

            if(EditorGUI.EndChangeCheck())
            {
                triggeredAction.fireOnce = fireOnce;
            }

            EditorGUILayout.Separator();

            // Draw Action Trigger
            GameObject obj = triggeredAction.gameObject;
            VRGrabTrigger.DisplayGrabTriggerSelector(ref triggeredAction.grabTrigger, ref obj);

            // Draw Action List
            EditorGUILayout.Separator();
            VRSelectableEditor.DisplayActionList(triggeredAction.actionList, new Object[] { triggeredAction });

        }

       
    }
}
