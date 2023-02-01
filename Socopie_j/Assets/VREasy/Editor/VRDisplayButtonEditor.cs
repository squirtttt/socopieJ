using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRDisplayButton))]
    [CanEditMultipleObjects]
    public class VRDisplayButtonEditor : Editor
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
            VRDisplayButton displayButton = (VRDisplayButton)target;

            ConfigureDisplayButton(ref displayButton,targets);
        }

        public static void ConfigureDisplayButton(ref VRDisplayButton displayButton, Object[] targets)
        {
            VR2DButton vrButton = displayButton;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            float timeToHide = EditorGUILayout.FloatField("Time to hide buttons", displayButton.timeToHide);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(displayButton, "changed display button settings");
                displayButton.timeToHide = timeToHide;
            }
            VRSelectable selectable = displayButton;
            VRSelectableEditor.DisplayStateOptions(selectable, targets);
            VRSelectableEditor.DisplayTooltip(selectable, targets);

            VR2DButtonEditor.displayGraphicalRepresentation(ref vrButton,targets,true,true,false);
            EditorGUILayout.Separator();

            VR2DButtonEditor.displayTypeAndFaceDirection(ref vrButton,targets);
            EditorGUILayout.Separator();

            // display representations for each switch object
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Swappable - Icons", EditorStyles.boldLabel);
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField("Each swappable object in the Switch action is represented with a different icon");

            EditorGUILayout.Separator();

            displayButton.SetRepresentationLength(displayButton.Action.swapObjects.Count);
            if (displayButton.Action.swapObjects.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                if (displayButton.Action.Target == null)
                    EditorGUILayout.LabelField("Target: " + displayButton.Action.Target);
                else
                    EditorGUILayout.LabelField("Target: " + displayButton.Action.Target.name);
                EditorGUILayout.LabelField("Icon");
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Add swappable slots via the SwitchAction script", MessageType.Info);
            }

            if (targets.Length > 1)
            {
                EditorGUILayout.HelpBox("Multiobject editing not supported to edit swappable icons", MessageType.Warning);
            } else
            {
                for (int ii = 0; ii < displayButton.Action.swapObjects.Count; ii++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("" + (ii + 1));

                    // Swappables
                    if (displayButton.Action.swapObjects[ii] == null)
                    {
                        EditorGUILayout.LabelField("Not assigned");
                    }
                    else
                    {
                        EditorGUILayout.LabelField(displayButton.Action.swapObjects[ii].name);
                    }

                    // Representation
                    displayButton.representations[ii] = (Sprite)EditorGUILayout.ObjectField(displayButton.representations[ii], typeof(Sprite), true);
                    EditorGUILayout.EndHorizontal();
                }
            }
            

            
            
        }
    }
}