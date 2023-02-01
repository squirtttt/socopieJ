using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRToggle))]
    [CanEditMultipleObjects]
    public class VRToggleEditor : Editor
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
            VRToggle toggle = (VRToggle)target;

            ConfigureToggle(ref toggle,targets);
        }

        public static void ConfigureToggle(ref VRToggle toggle, Object[] targets)
        {
            VR2DButton button = toggle;
            VR2DButtonEditor.displayGraphicalRepresentation(ref button, targets, true, true, false);
            VR2DButtonEditor.displayTypeAndFaceDirection(ref button, targets);

            VRSelectable selectable = toggle;

            EditorGUILayout.LabelField("ON STATE (Idle)", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            VRSelectableEditor.DisplayActionList(toggle.IdleActions,targets);
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("OFF STATE (Select)", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            VRSelectableEditor.DisplayActionList(toggle.SelectActions,targets);
            EditorGUI.indentLevel--;

            VRSelectableEditor.DisplayStateOptions(selectable, targets);
            VRSelectableEditor.DisplayTooltip(selectable, targets);
            VRSelectableEditor.DisplayTimingOptions(selectable, targets);
            VRSelectableEditor.DisplayAudioOptions(selectable, targets);

        }
    }
}