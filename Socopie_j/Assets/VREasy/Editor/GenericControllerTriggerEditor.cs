using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GenericControllerTrigger))]
    public class GenericControllerTriggerEditor : Editor
    {
        private OCULUS_CONTROLLER_INPUT_TYPE oculus_input_type = OCULUS_CONTROLLER_INPUT_TYPE.BUTTON;

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

            GenericControllerTrigger genericController = (GenericControllerTrigger)target;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Settings for your VR controller", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();

            //GENERIC_VR_BUTTON vr_button = (GENERIC_VR_BUTTON)EditorGUILayout.EnumPopup("Selected button", genericController.vr_button);
            GENERIC_CONTROLLER_TYPE type = (GENERIC_CONTROLLER_TYPE)EditorGUILayout.EnumPopup("Controller type", genericController.type);
            GENERIC_VR_BUTTON vr_button = genericController.vr_button;

            if (EditorGUI.EndChangeCheck())
            {
                foreach(GenericControllerTrigger g in targets)
                {
                    Undo.RecordObject(g, "Change in generic controller");
                    //g.vr_button = vr_button;
                    g.type = type;
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Select input", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current input: " + genericController.vr_button);
            EditorGUI.BeginChangeCheck();
            switch (genericController.type)
            {
                case GENERIC_CONTROLLER_TYPE.OCULUS_TOUCH:
                    {
                        OculusControllerTriggerEditor.CheckWarningOculusSDKOrder();
                        EditorGUILayout.Separator();
                        vr_button = drawAndSelectOculusInputSelector(vr_button);
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.OCULUS_REMOTE:
                    {
                        OculusControllerTriggerEditor.CheckWarningOculusSDKOrder();
                        EditorGUILayout.Separator();
                        vr_button = drawAndSelectOculusRemoteInputSelector(vr_button);
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.STEAM_VR:
                    {
                        vr_button = drawAndSelectSteamVRInputSelector(vr_button);
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.WINDOWS_MIXED_REALITY:
                    {
#if UNITY_2017_2_OR_NEWER
                        vr_button = drawAndSelectWMRInputSelector(vr_button);
#else
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("Windows Mixed Reality controllers were only introduced in Unity 2017.2. Please upgrade to use them", MessageType.Warning);
#endif
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.KNUCKLES:
                    {
#if UNITY_2017_2_OR_NEWER
                        vr_button = drawAndSelectKnucklesInputSelector(vr_button);
#else
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("Vive Knuckles controllers were only introduced in Unity 2017.2. Please upgrade to use them", MessageType.Warning);
#endif
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.DAYDREAM:
                    {
#if UNITY_2017_4_OR_NEWER
                        vr_button = drawAndSelectDaydreamInputSelector(vr_button);
#else
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("Daydream controllers were only introduced in Unity 2017.4. Please upgrade to use them or use the external GoogleVR SDK.", MessageType.Warning);
#endif
                    }
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (GenericControllerTrigger g in targets)
                {
                    Undo.RecordObject(g, "Change in generic controller");
                    g.vr_button = vr_button;
                }
            }
        }

        private GENERIC_VR_BUTTON drawAndSelectOculusInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            oculus_input_type = (OCULUS_CONTROLLER_INPUT_TYPE)EditorGUILayout.EnumPopup("Input type", oculus_input_type);

            Texture2D img = null;
            switch(oculus_input_type)
            {
                case OCULUS_CONTROLLER_INPUT_TYPE.BUTTON:
                    {
                        img = Resources.Load<Texture2D>("Oculus_Touch_Generic_Button");
                    }
                    break;
                case OCULUS_CONTROLLER_INPUT_TYPE.TOUCH:
                    {
                        img = Resources.Load<Texture2D>("Oculus_Touch_Generic_Touch");
                    }
                    break;
            }
            
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            switch (oculus_input_type)
            {
                case OCULUS_CONTROLLER_INPUT_TYPE.BUTTON:
                    {
                        // Position all buttons
                        if (VREasy_utils.CreateOverlayGUIButton(135 / referenceSize * baseWidth, 150 / referenceSize * baseHeight, ref GraphicRect, "Primary thumbstick"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMB_STICK_PRESS;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(45 / referenceSize * baseWidth, 240 / referenceSize * baseHeight, ref GraphicRect, "X"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_THREE_PRESS;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(265 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumbstick"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMB_STICK_PRESS;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(315 / referenceSize * baseWidth, 90 / referenceSize * baseHeight, ref GraphicRect, "A"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_ONE_PRESS;
                        }

                        // two
                        if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 45 / referenceSize * baseHeight, ref GraphicRect, "B"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_TWO_PRESS;
                        }
                        // four
                        if (VREasy_utils.CreateOverlayGUIButton(225 / referenceSize * baseWidth, 300 / referenceSize * baseHeight, ref GraphicRect, "Y"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_FOUR_PRESS;
                        }
                        // start
                        if (VREasy_utils.CreateOverlayGUIButton(225 / referenceSize * baseWidth, 250 / referenceSize * baseHeight, ref GraphicRect, "Start"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_START;
                        }
                    }
                    break;
                case OCULUS_CONTROLLER_INPUT_TYPE.TOUCH:
                    {
                        // Position all buttons
                        if (VREasy_utils.CreateOverlayGUIButton(135 / referenceSize * baseWidth, 150 / referenceSize * baseHeight, ref GraphicRect, "Primary thumbstick"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMB_STICK_TOUCH;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(230 / referenceSize * baseWidth, 365 / referenceSize * baseHeight, ref GraphicRect, "Primary index trigger"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_PRIMARY_INDEX_TRIGGER;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(260 / referenceSize * baseWidth, 25 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumbstick"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMB_STICK_TOUCH;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(165 / referenceSize * baseWidth, 85 / referenceSize * baseHeight, ref GraphicRect, "Secondary index trigger"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_SECONDARY_INDEX_TRIGGER;
                        }
                        // primary and secondary thumb rest
                        if (VREasy_utils.CreateOverlayGUIButton(80 / referenceSize * baseWidth, 350 / referenceSize * baseHeight, ref GraphicRect, "Primary thumb rest"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMBREST;
                        }
                        if (VREasy_utils.CreateOverlayGUIButton(310 / referenceSize * baseWidth, 210 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumb rest"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMBREST;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(45 / referenceSize * baseWidth, 240 / referenceSize * baseHeight, ref GraphicRect, "X"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_THREE_TOUCH;
                        }

                        if (VREasy_utils.CreateOverlayGUIButton(355 / referenceSize * baseWidth, 110 / referenceSize * baseHeight, ref GraphicRect, "A"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_ONE_TOUCH;
                        }

                        // two
                        if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 45 / referenceSize * baseHeight, ref GraphicRect, "B"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_TWO_TOUCH;
                        }
                        // four
                        if (VREasy_utils.CreateOverlayGUIButton(225 / referenceSize * baseWidth, 300 / referenceSize * baseHeight, ref GraphicRect, "Y"))
                        {
                            defaultButton = GENERIC_VR_BUTTON.OCULUS_FOUR_TOUCH;
                        }
                    }
                    break;
            }

            

            return defaultButton;
        }

        private GENERIC_VR_BUTTON drawAndSelectOculusRemoteInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Oculus_Remote_Generic");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(130 / referenceSize * baseWidth, 40 / referenceSize * baseHeight, ref GraphicRect, "One press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_REMOTE_ONE;
            }

            if (VREasy_utils.CreateOverlayGUIButton(290 / referenceSize * baseWidth, 310 / referenceSize * baseHeight, ref GraphicRect, "Two press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_REMOTE_TWO;
            }

            return defaultButton;
        }

        private GENERIC_VR_BUTTON drawAndSelectKnucklesInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Vive_Knuckles_generic");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            // left and right versions of the following
            // inner face button
            // outer face button
            // trackpad press
            // trackpad touch
            // trigger touch
            if (VREasy_utils.CreateOverlayGUIButton(240 / referenceSize * baseWidth, 260 / referenceSize * baseHeight, ref GraphicRect, "Left inner face"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_LEFT_INNER_FACE;
            }
            if (VREasy_utils.CreateOverlayGUIButton(150 / referenceSize * baseWidth, 95 / referenceSize * baseHeight, ref GraphicRect, "Right inner face"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_RIGHT_INNER_FACE;
            }
            if (VREasy_utils.CreateOverlayGUIButton(155 / referenceSize * baseWidth, 200 / referenceSize * baseHeight, ref GraphicRect, "Left outer face"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_LEFT_OUTER_FACE;
            }
            if (VREasy_utils.CreateOverlayGUIButton(235 / referenceSize * baseWidth, 30 / referenceSize * baseHeight, ref GraphicRect, "Right outer face"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_RIGHT_OUTER_FACE;
            }
            if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 190 / referenceSize * baseHeight, ref GraphicRect, "Left trackpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_LEFT_TRACKPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(235 / referenceSize * baseWidth, 210 / referenceSize * baseHeight, ref GraphicRect, "Left trackpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_LEFT_TRACKPAD_TOUCH;
            }
            if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 25 / referenceSize * baseHeight, ref GraphicRect, "Right trackpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_RIGHT_TRACKPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(150 / referenceSize * baseWidth, 50 / referenceSize * baseHeight, ref GraphicRect, "Right trackpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_RIGHT_TRACKPAD_TOUCH;
            }
            if (VREasy_utils.CreateOverlayGUIButton(205 / referenceSize * baseWidth, 305 / referenceSize * baseHeight, ref GraphicRect, "Left trigger touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_LEFT_TRIGGER_TOUCH;
            }
            if (VREasy_utils.CreateOverlayGUIButton(185 / referenceSize * baseWidth, 140 / referenceSize * baseHeight, ref GraphicRect, "Right trigger touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.KNUCKLES_RIGHT_TRIGGER_TOUCH;
            }
            return defaultButton;
        }

        private GENERIC_VR_BUTTON drawAndSelectSteamVRInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Vive_Controller_generic");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(165/ referenceSize * baseWidth, 120/ referenceSize * baseHeight, ref GraphicRect, "Right trigger touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRIGGER_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(365/ referenceSize*baseWidth, 80/ referenceSize*baseHeight, ref GraphicRect, "Right trackpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRACKPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(320/ referenceSize*baseWidth, 20/ referenceSize*baseHeight, ref GraphicRect, "Right trackpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRACKPAD_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(220/ referenceSize*baseWidth, 305/ referenceSize*baseHeight, ref GraphicRect, "Left trigger touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_LEFT_TRIGGER_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(25 / referenceSize * baseWidth, 210 / referenceSize * baseHeight, ref GraphicRect, "Left trackpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_LEFT_TRACKPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(70 / referenceSize * baseWidth, 155 / referenceSize * baseHeight, ref GraphicRect, "Left trackpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_LEFT_TRACKPAD_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(125 / referenceSize * baseWidth, 140 / referenceSize * baseHeight, ref GraphicRect, "Left menu press"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_LEFT_CONTROLLER_MENU;
            }
            if (VREasy_utils.CreateOverlayGUIButton(270 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Right menu press"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_RIGHT_CONTROLLER_MENU;
            }
            return defaultButton;


        }

        private GENERIC_VR_BUTTON drawAndSelectWMRInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("MR_controller");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(70 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_TOUCHPAD_TOUCH;
            }
            if (VREasy_utils.CreateOverlayGUIButton(320 / referenceSize * baseWidth, 135 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_TOUCHPAD_TOUCH;
            }
            if (VREasy_utils.CreateOverlayGUIButton(180 / referenceSize * baseWidth, 130 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_TOUCHPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(200 / referenceSize * baseWidth, 250 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_TOUCHPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(170 / referenceSize * baseWidth, 65 / referenceSize * baseHeight, ref GraphicRect, "Left thumbstick press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_THUMBSTICK_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(220 / referenceSize * baseWidth, 180 / referenceSize * baseHeight, ref GraphicRect, "Right thumbstick press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_THUMBSTICK_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(40 / referenceSize * baseWidth, 150 / referenceSize * baseHeight, ref GraphicRect, "Left select trigger press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_SELECT_TRIGGER_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(340 / referenceSize * baseWidth, 270 / referenceSize * baseHeight, ref GraphicRect, "Right select trigger press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_SELECT_TRIGGER_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(170 / referenceSize * baseWidth, 190 / referenceSize * baseHeight, ref GraphicRect, "Left grip press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_GRIP_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(210 / referenceSize * baseWidth, 310 / referenceSize * baseHeight, ref GraphicRect, "Right grip press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_GRIP_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(65 / referenceSize * baseWidth, 185 / referenceSize * baseHeight, ref GraphicRect, "Left menu button press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_MENU_BUTTON_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(320 / referenceSize * baseWidth, 310 / referenceSize * baseHeight, ref GraphicRect, "Right menu button press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_MENU_BUTTON_PRESS;
            }

            return defaultButton;


        }

        private GENERIC_VR_BUTTON drawAndSelectDaydreamInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Daydream_Controller");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            
            if (VREasy_utils.CreateOverlayGUIButton(200 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.DAYDREAM_RIGHT_TOUCHPAD_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(280 / referenceSize * baseWidth, 50 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.DAYDREAM_RIGHT_TOUCHPAD_CLICK;
            }

            if (VREasy_utils.CreateOverlayGUIButton(240 / referenceSize * baseWidth, 165 / referenceSize * baseHeight, ref GraphicRect, "Right App button"))
            {
                defaultButton = GENERIC_VR_BUTTON.DAYDREAM_RIGHT_APP_BUTTON;
            }

            if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 195 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.DAYDREAM_LEFT_TOUCHPAD_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(110 / referenceSize * baseWidth, 220 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.DAYDREAM_LEFT_TOUCHPAD_CLICK;
            }

            if (VREasy_utils.CreateOverlayGUIButton(155 / referenceSize * baseWidth, 355 / referenceSize * baseHeight, ref GraphicRect, "Left App button"))
            {
                defaultButton = GENERIC_VR_BUTTON.DAYDREAM_LEFT_APP_BUTTON;
            }

            return defaultButton;
        }
    }
}