using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
namespace VREasy
{
    [CustomEditor(typeof(VRGUIButton))]
    [CanEditMultipleObjects]
    public class VRGUIButtonEditor : Editor
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

            VRGUIButton button = (VRGUIButton)target;

            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Selectable colours", EditorStyles.boldLabel);
            ColorBlock block = button.UnitySelectable.colors;
            block.normalColor = EditorGUILayout.ColorField("Normal", button.UnitySelectable.colors.normalColor);
            block.highlightedColor = EditorGUILayout.ColorField("Highlight", button.UnitySelectable.colors.highlightedColor);
            block.pressedColor = EditorGUILayout.ColorField("Pressed", button.UnitySelectable.colors.pressedColor);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(VRGUIButton bt in targets)
                {
                    Undo.RecordObject(bt.UnitySelectable, "Changed colours in selectable");
                    bt.UnitySelectable.colors = block;
                }
                

            }
            EditorGUILayout.LabelField("See more in the Selectable component inspector", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();

            VRSelectable selectable = button;
            VRSelectableEditor.DisplayCommon(ref selectable,targets);

            
        }
    }
}