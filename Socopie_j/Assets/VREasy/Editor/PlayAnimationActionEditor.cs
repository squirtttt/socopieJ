using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(PlayAnimationAction))]
    [CanEditMultipleObjects]
    public class PlayAnimationActionEditor : Editor
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
            PlayAnimationAction playAnim = (PlayAnimationAction)target;

            ANIMATION_TYPE type = (ANIMATION_TYPE)EditorGUILayout.EnumPopup("Animation type", playAnim.type);
            foreach(PlayAnimationAction p in targets)
            {
                p.type = type;
            }
            EditorGUILayout.Separator();
            switch (playAnim.type)
            {
                case ANIMATION_TYPE.ANIMATOR:
                    drawAnimatorPanel(playAnim,targets);
                    break;
                case ANIMATION_TYPE.LEGACY:
                    drawLegacyPanel(playAnim,targets);
                    break;
            }
        }

        private void drawAnimatorPanel(PlayAnimationAction playAnim, Object[] targets)
        {
            EditorGUI.BeginChangeCheck();
            Animator animator = (Animator)EditorGUILayout.ObjectField("Animator", playAnim.animator, typeof(Animator), true);
            string targetParameter = EditorGUILayout.TextField("Target parameter", playAnim.targetParameter);
            ANIMATOR_PARAMETER_TYPE parameterType = (ANIMATOR_PARAMETER_TYPE)EditorGUILayout.EnumPopup("Parameter type", playAnim.parameterType);
            bool parameterValue_b = false;
            float parameterValue_f = 0.0f;
            switch (parameterType)
            {
                case ANIMATOR_PARAMETER_TYPE.BOOL:
                case ANIMATOR_PARAMETER_TYPE.TRIGGER:
                    parameterValue_b = EditorGUILayout.Toggle("Parameter value", playAnim.parameterValue_b);
                    break;
                case ANIMATOR_PARAMETER_TYPE.FLOAT:
                    parameterValue_f = EditorGUILayout.FloatField("Float value", playAnim.parameterValue_f);
                    break;
                case ANIMATOR_PARAMETER_TYPE.INT:
                    parameterValue_f = EditorGUILayout.IntField("Integer value", (int)playAnim.parameterValue_f);
                    break;
            }
            if(EditorGUI.EndChangeCheck())
            {
                foreach(PlayAnimationAction p in targets)
                {
                    Undo.RecordObject(p, "Changed animator parameters");
                    p.animator = animator;
                    p.targetParameter = targetParameter;
                    p.parameterType = parameterType;
                    p.parameterValue_b = parameterValue_b;
                    p.parameterValue_f = parameterValue_f;
                }
                
            }

        }

        private void drawLegacyPanel(PlayAnimationAction playAnim, Object[] targets)
        {
            EditorGUI.BeginChangeCheck();
            Animation animation = (Animation)EditorGUILayout.ObjectField("Animation", playAnim.animation, typeof(Animation), true);
            string clip = EditorGUILayout.TextField("Clip", playAnim.clip);
            PLAY_ACTION action = (PLAY_ACTION)EditorGUILayout.EnumPopup("Animation action", playAnim.playType);
            //ANIMATION_ACTION action = (ANIMATION_ACTION)EditorGUILayout.EnumPopup("Animation action", playAnim.action);
            float speed = EditorGUILayout.FloatField("Speed (negative for reverse)", playAnim.speed);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(PlayAnimationAction p in targets)
                {
                    Undo.RecordObject(p, "Changed animation parameters");
                    p.animation = animation;
                    p.clip = clip;
                    p.playType = action;
                    p.speed = speed;
                }
                
            }
        }
    }
}