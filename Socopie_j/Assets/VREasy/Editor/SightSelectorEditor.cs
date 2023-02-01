using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(SightSelector))]
    public class SightSelectorEditor : Editor
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
            SightSelector selector = (SightSelector)target;

            ConfigureSightSelector(selector);
        }

        public static void ConfigureSightSelector(SightSelector selector)
        {
            LOSSelector los = selector;
            LOSSelectorEditor.ConfigureLOSSelector(ref los);

            VRSelector sel = selector;
            VRSelectorEditor.ConfigureSelector(ref sel);

            EditorGUI.BeginChangeCheck();
            float selectionDistance = EditorGUILayout.DelayedFloatField("Selection distance", selector.selectionDistance);
            bool useCrosshair = EditorGUILayout.Toggle("Use crosshair", selector.useCrosshair);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selector, "Changed sight selector properties distance");
                selector.selectionDistance = selectionDistance;
                selector.useCrosshair = useCrosshair;
            }
            selector.reconfigureCrosshair();
            if (selector.useCrosshair)
            {
                // display different options based on the crosshair type
                EditorGUI.BeginChangeCheck();
                CROSSHAIR_TYPE type = (CROSSHAIR_TYPE)EditorGUILayout.EnumPopup("Type", selector.crosshairType);
                if(EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(selector, "Crosshair type");
                    selector.crosshairType = type;
                }
                Sprite crosshairSprite = selector.CrosshairSprite;
                // common for static sprites
                if (selector.crosshairType == CROSSHAIR_TYPE.SINGLE_SPRITE || selector.crosshairType == CROSSHAIR_TYPE.DUAL_SPRITE)
                {
                    EditorGUI.BeginChangeCheck();
                    crosshairSprite = (Sprite)EditorGUILayout.ObjectField("Active sprite", selector.CrosshairSprite, typeof(Sprite), true);
                    float crosshairSize = selector.CrosshairSize;
                    float resizeMultiplier = selector.resizeMultiplier;
                    float resizeSpeed = selector.resizeSpeed;
                    bool dynamicResize = selector.dynamicSize;
                    if (crosshairSprite)
                    {
                        crosshairSize = EditorGUILayout.Slider("Crosshair size", selector.CrosshairSize, 0, 1);
                        dynamicResize = EditorGUILayout.Toggle("Dynamic resizing", selector.dynamicSize);
                        if (dynamicResize)
                        {
                            EditorGUI.indentLevel++;
                            resizeMultiplier = EditorGUILayout.FloatField("Resize multiplier %", selector.resizeMultiplier);
                            resizeSpeed = EditorGUILayout.FloatField("Resize speed", selector.resizeSpeed);
                            EditorGUI.indentLevel--;
                        }

                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(selector, "Changed crosshair properties");
                        selector.CrosshairSprite = crosshairSprite;
                        selector.CrosshairSize = crosshairSize;
                        selector.resizeSpeed = resizeSpeed;
                        selector.resizeMultiplier = resizeMultiplier;
                        selector.dynamicSize = dynamicResize;
                    }
                }
                
                EditorGUILayout.Separator();
                EditorGUI.BeginChangeCheck();
                // specific
                switch (selector.crosshairType)
                {
                    case CROSSHAIR_TYPE.SINGLE_SPRITE:
                        {
                            Color crosshairActiveColour = selector.CrosshairActiveColour;
                            Color crosshairIdleColour = selector.CrosshairIdleColour;
                            if (crosshairSprite)
                            {
                                crosshairActiveColour = EditorGUILayout.ColorField("Active colour", selector.CrosshairActiveColour);
                                crosshairIdleColour = EditorGUILayout.ColorField("Idle colour", selector.CrosshairIdleColour);
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(selector, "Changed crosshair properties");
                                selector.CrosshairActiveColour = crosshairActiveColour;
                                selector.CrosshairIdleColour = crosshairIdleColour;
                            }
                        }
                        break;
                    case CROSSHAIR_TYPE.DUAL_SPRITE:
                        {
                            Sprite idleSprite = (Sprite)EditorGUILayout.ObjectField("Idle sprite", selector.idleSprite, typeof(Sprite), true);

                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(selector, "Changed crosshair properties");
                                selector.idleSprite = idleSprite;
                            }
                        }
                        break;
                    case CROSSHAIR_TYPE.ANIMATED_SPRITE:
                        {
                            RuntimeAnimatorController animatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator controller", selector.Animator.runtimeAnimatorController, typeof(RuntimeAnimatorController), true);
                            string idleState = EditorGUILayout.TextField("Idle state", selector.idleAnimatorState);
                            string activeState = EditorGUILayout.TextField("Active state", selector.activeAnimatorState);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(selector, "Changed crosshair properties");
                                selector.Animator.runtimeAnimatorController = animatorController;
                                selector.idleAnimatorState = idleState;
                                selector.activeAnimatorState = activeState;
                            }
                            if(selector.Animator.runtimeAnimatorController == null)
                            {
                                EditorGUILayout.HelpBox("An animator controller must be linked with the SightSelector to play animated Sprites", MessageType.Error);
                            } else
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.LabelField("The animation states must exist within [" + selector.Animator.runtimeAnimatorController.name + "]", EditorStyles.wordWrappedLabel);
                                EditorGUI.indentLevel--;
                                EditorGUILayout.HelpBox("The SightSelector state (idle or active) will determine the animator state that will play in controller [" + selector.Animator.runtimeAnimatorController.name + "]", MessageType.Info);
                            }
                            
                        }
                        break;
                }
                
            }

            EditorGUILayout.Separator();
            if(GUILayout.Button("Remove component"))
            {
                if(EditorUtility.DisplayDialog("VREasy: Remove Sight Selector","You are about to remove the Sight Selector. Would you like to remove the crosshair attached to it?","Yes","No"))
                {
                    selector.removeCrosshair();
                }
                DestroyImmediate(selector);
            }
        }
    }
}