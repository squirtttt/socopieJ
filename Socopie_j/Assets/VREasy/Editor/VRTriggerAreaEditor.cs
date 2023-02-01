using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRTriggerArea))]
    [CanEditMultipleObjects]
    public class VRTriggerAreaEditor : Editor
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
            VRTriggerArea trigger = (VRTriggerArea)target;

            ConfigureAreaTrigger(ref trigger,targets);

            EditorGUILayout.Separator();
            TouchSelector touch = FindObjectOfType<TouchSelector>();
            if(touch == null)
            {
                EditorStyles.label.wordWrap = true;
                EditorGUILayout.HelpBox("Current scene does not contain an instance of TouchSelector. This object is needed to activate trigger areas", MessageType.Warning);
            }
        }

        public static void ConfigureAreaTrigger(ref VRTriggerArea trigger, Object[] targets)
        {
            VRSelectable selectable = trigger;
            VRSelectableEditor.DisplayStateOptions(selectable, targets);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("[On enter]", EditorStyles.boldLabel);
            VRSelectableEditor.DisplayActionList(trigger.EntryActions,targets);

            // display exit trigger actionlist
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("[On Exit]", EditorStyles.boldLabel);
            VRSelectableEditor.DisplayActionList(trigger.ExitActions,targets);
            EditorGUILayout.Separator();

            VRSelectableEditor.DisplayAudioOptions(selectable, targets);
        }
    }
}