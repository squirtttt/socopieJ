using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRAnimatorExposer))]
    public class VRAnimatorExposerEditor : Editor
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
            VRAnimatorExposer anim = (VRAnimatorExposer)target;

            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            ANIMATION_TYPE type = (ANIMATION_TYPE)EditorGUILayout.EnumPopup("Animation type", anim.type);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(anim, "changed type");
                anim.type = type;
            }
            switch(anim.type)
            {
                case ANIMATION_TYPE.ANIMATOR:
                    if (anim.animator == null)
                    {
                        EditorGUILayout.HelpBox("Missing required Animator component. Please add and configure one to the game object", MessageType.Error);
                    } else
                    {
                        if(anim.animator.runtimeAnimatorController == null)
                        {
                            EditorGUILayout.HelpBox("AnimatorController not assigned to Animator component. Please fix the Animator", MessageType.Error);
                        } else
                        {
                            EditorGUILayout.LabelField("The currently active state in the Animator Controller is now controllable via a VRSlider",EditorStyles.wordWrappedLabel);
                        }
                    }
                    break;
                case ANIMATION_TYPE.LEGACY:
                    if (anim.animation == null)
                    {
                        EditorGUILayout.HelpBox("Missing required Animation component. Please add and configure one to the game object", MessageType.Error);
                    }
                    else
                    {
                        if (anim.animation.GetClipCount() == 0)
                        {
                            EditorGUILayout.HelpBox("Animation does not have animations assigned (size of Animations array should be > 1. Please fix the Animation", MessageType.Error);
                        } else
                        {
                            if(!anim.animation.clip.legacy)
                            {
                                EditorGUILayout.LabelField("All animations should be set to legacy in the import settings");
                                if(GUILayout.Button("Set legacy"))
                                {
                                    anim.animation.clip.legacy = true;
                                }
                            } else
                            {
                                EditorGUILayout.LabelField("Default animation clip is now controllable",EditorStyles.wordWrappedLabel);
                            }
                            
                        }
                    }
                    break;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("Animation does not have animations assigned (size of Animations array should be > 1. Please fix the Animation", MessageType.Info);
        }
    }
}