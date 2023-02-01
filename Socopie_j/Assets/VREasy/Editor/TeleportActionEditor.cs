using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(TeleportAction))]
    public class TeleportActionEditor : Editor
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

            TeleportAction teleport = (TeleportAction)target;
            ConfigureTeleportAction(teleport,true);
        }

        public static void ConfigureTeleportAction(TeleportAction teleport, bool showCustomTarget = false)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Teleport settings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            float fader = EditorGUILayout.FloatField("Fading time", teleport.fadeTimer);
            Transform hmd = (Transform)EditorGUILayout.ObjectField("HMD parent object", teleport.HMD, typeof(Transform), true);
            if(hmd == null)
            {
                EditorGUILayout.HelpBox("HMD parent object needs to be set to have something to teleport!", MessageType.Error);
            }
            Transform targetPosition = null; 
            if (showCustomTarget)
            {
                targetPosition = (Transform)EditorGUILayout.ObjectField("Target position", teleport.targetPosition, typeof(Transform), true);
            }
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(teleport, "teleport changed");
                teleport.HMD = hmd;
                teleport.fadeTimer = fader;
                teleport.targetPosition = targetPosition;
            }
        }
    }
}