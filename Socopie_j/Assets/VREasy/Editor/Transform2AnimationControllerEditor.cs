using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(Transform2AnimationController))]
    [CanEditMultipleObjects]
    public class Transform2AnimationControllerEditor : Editor
    {

        [MenuItem("VREasy/Transform-based controllers/Transform2Animation controller")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<Transform2AnimationController>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add a Transform to Animation controller you must select a game object in the hierarchy first", "OK");
            }
        }

        SerializedProperty originAxis;
        SerializedProperty originElement;
        SerializedProperty origin;
        SerializedProperty animationType;
        SerializedProperty controlType;
        SerializedProperty animationComponent;
        SerializedProperty selectedAnimation;
        SerializedProperty animatorComponent;
        SerializedProperty animatorState;
        SerializedProperty animatorLayer;
        SerializedProperty targetParameter;

        private void OnEnable()
        {
            originAxis = serializedObject.FindProperty("originAxis");
            originElement = serializedObject.FindProperty("originElement");
            origin = serializedObject.FindProperty("origin");
            animationType = serializedObject.FindProperty("animationType");
            controlType = serializedObject.FindProperty("controlType");
            animationComponent = serializedObject.FindProperty("animationComponent");
            selectedAnimation = serializedObject.FindProperty("selectedAnimation");
            animatorComponent = serializedObject.FindProperty("animatorComponent");
            animatorState = serializedObject.FindProperty("animatorState");
            animatorLayer = serializedObject.FindProperty("animatorLayer");
            targetParameter = serializedObject.FindProperty("targetParameter");
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
            Transform2AnimationController anim = (Transform2AnimationController)target;

            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("This component allows for an animation to be controlled based on the position / rotation of a game object (origin) in the scene", MessageType.Info);


            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Origin (observed)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(origin);
            EditorGUILayout.PropertyField(originElement);
            EditorGUILayout.PropertyField(originAxis);

            EditorGUILayout.Separator();
            if(anim.origin == null)
            {
                EditorGUILayout.HelpBox("Select an origin transform to control the animation", MessageType.Warning);
            } else
            {
                float prevWidth = EditorGUIUtility.labelWidth;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Control range", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Current observed value: " + anim.GetCurrentOriginValue() + " (" + anim.originElement + " " + anim.originAxis + ")");
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                //EditorGUIUtility.labelWidth = 50;
                float minimumTargetRange = EditorGUILayout.FloatField("Start", anim.rangeValues.x, GUILayout.ExpandWidth(false));
                if(GUILayout.Button("Same as current"))
                {
                    minimumTargetRange = anim.GetCurrentOriginValue();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                float maximumTargetRange = EditorGUILayout.FloatField("End", anim.rangeValues.y, GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Same as current"))
                {
                    maximumTargetRange = anim.GetCurrentOriginValue();
                }
                EditorGUILayout.EndHorizontal();
                Vector2 targetValueRange = new Vector2(minimumTargetRange, maximumTargetRange);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (Transform2AnimationController a in targets)
                    {
                        Undo.RecordObject(a, "");
                        a.rangeValues = targetValueRange;
                    }
                }
                EditorGUIUtility.labelWidth = prevWidth;
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Start and End values determine when the animation control will start and end.", MessageType.Info);

            }


            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Animation controlled", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(controlType);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(animationType);

            switch (anim.animationType)
            {
                case ANIMATION_TYPE.ANIMATOR:
                    {
                        EditorGUILayout.PropertyField(animatorComponent);
                        if (anim.controlType == ANIMATION_TARGET.NUMERIC_PARAMETER)
                        {
                            EditorGUILayout.PropertyField(targetParameter);
                            EditorGUILayout.HelpBox("Only Float parameters are supported. The parameter will be set within the [0-1] range", MessageType.Info);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(animatorLayer);
                            EditorGUILayout.PropertyField(animatorState);

                        }
                    }
                    break;
                case ANIMATION_TYPE.LEGACY:
                    {
                        if(anim.controlType == ANIMATION_TARGET.NUMERIC_PARAMETER)
                        {
                            EditorGUILayout.HelpBox("To control a numeric parameter you must have ANIMATOR animation type", MessageType.Warning);
                        } else
                        {
                            EditorGUILayout.PropertyField(animationComponent);
                            EditorGUILayout.PropertyField(selectedAnimation);
                        }
                        
                    }
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static bool containsParam(Animator _Anim, string _ParamName)
        {
            foreach (AnimatorControllerParameter param in _Anim.parameters)
            {
                if (param.name == _ParamName) return true;
            }
            return false;
        }
    }


}