using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SteamControllerGrab))]
    public class SteamControllerGrabEditor : Editor
    {
        bool handleRepaintErrors = false;
        public override void OnInspectorGUI()
        {
            //SteamControllerGrab steamGrab = (SteamControllerGrab)target;
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }

#if VREASY_STEAM_SDK

            SteamControllerGrab steamController = (SteamControllerGrab)target;
            DrawDefaultInspector();
            /*EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("If only one of the controllers is active during game play, it will respond to both LEFT and RIGHT versions of the button selected", MessageType.Info);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Select input", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current input: " + steamController.button + "(" + steamController.controllerSide + ") " + steamController.type);
            EditorGUI.BeginChangeCheck();

            STEAM_VR_CONTROLLER_INPUT_TYPE type = steamController.type; 
            Valve.VR.EVRButtonId button = steamController.button;
            STEAM_VR_CONTROLLER_SIDE side = steamController.controllerSide;
            drawAndSelectViveInputSelector(ref button, ref side, ref type);

            if(EditorGUI.EndChangeCheck())
            {
                foreach(SteamControllerGrab sg in targets)
                {
                    Undo.RecordObject(sg,"change of button");
                    sg.button = button;
                    sg.type = type;
                    sg.controllerSide = side;
                }
            }*/
#else
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.HelpBox("Steam SDK not found or not activated. Please make sure the Steam SDK is imported and you have activated it via the VREasy/SDK Selector GUI", MessageType.Warning);
#endif
        }

#if VREASY_STEAM_SDK
        private void drawAndSelectViveInputSelector(ref Valve.VR.EVRButtonId defaultButton, ref STEAM_VR_CONTROLLER_SIDE side, ref STEAM_VR_CONTROLLER_INPUT_TYPE type)
        {
            Texture2D img = Resources.Load<Texture2D>("Vive_Controller");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;
            float baseX = GraphicRect.x;
            float baseY = GraphicRect.y;
            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(320 / referenceSize * baseWidth, 15 / referenceSize * baseHeight, ref GraphicRect, "Right Menu button"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
                side = STEAM_VR_CONTROLLER_SIDE.RIGHT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(170 / referenceSize * baseWidth, 130 / referenceSize * baseHeight, ref GraphicRect, "Right trigger press"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
                side = STEAM_VR_CONTROLLER_SIDE.RIGHT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(175 / referenceSize * baseWidth, 90 / referenceSize * baseHeight, ref GraphicRect, "Right trigger touch"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
                side = STEAM_VR_CONTROLLER_SIDE.RIGHT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(350 / referenceSize * baseWidth, 90 / referenceSize * baseHeight, ref GraphicRect, "Right touchpad press"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
                side = STEAM_VR_CONTROLLER_SIDE.RIGHT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(350 / referenceSize * baseWidth, 50 / referenceSize * baseHeight, ref GraphicRect, "Right touchpad touch"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
                side = STEAM_VR_CONTROLLER_SIDE.RIGHT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(215 / referenceSize * baseWidth, 150 / referenceSize * baseHeight, ref GraphicRect, "Right Grip"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_Grip;
                side = STEAM_VR_CONTROLLER_SIDE.RIGHT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(80 / referenceSize * baseWidth, 145 / referenceSize * baseHeight, ref GraphicRect, "Left Menu button"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
                side = STEAM_VR_CONTROLLER_SIDE.LEFT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 295 / referenceSize * baseHeight, ref GraphicRect, "Left trigger press"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
                side = STEAM_VR_CONTROLLER_SIDE.LEFT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(220 / referenceSize * baseWidth, 250 / referenceSize * baseHeight, ref GraphicRect, "Left trigger touch"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
                side = STEAM_VR_CONTROLLER_SIDE.LEFT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(35 / referenceSize * baseWidth, 235 / referenceSize * baseHeight, ref GraphicRect, "Left touchpad press"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
                side = STEAM_VR_CONTROLLER_SIDE.LEFT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(35 / referenceSize * baseWidth, 185 / referenceSize * baseHeight, ref GraphicRect, "Left touchpad touch"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
                side = STEAM_VR_CONTROLLER_SIDE.LEFT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(155 / referenceSize * baseWidth, 330 / referenceSize * baseHeight, ref GraphicRect, "Left Grip"))
            {
                defaultButton = Valve.VR.EVRButtonId.k_EButton_Grip;
                side = STEAM_VR_CONTROLLER_SIDE.LEFT;
                type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;
            }

            

        }
#endif
    }
}