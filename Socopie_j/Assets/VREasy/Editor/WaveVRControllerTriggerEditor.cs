using UnityEngine;
using System.Collections;
using UnityEditor;
#if VREASY_WAVEVR_SDK
using wvr;
#endif

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(WaveVRControllerTrigger))]
    public class WaveVRControllerTriggerEditor : Editor
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

#if VREASY_WAVEVR_SDK
            WaveVRControllerTrigger waveController = (WaveVRControllerTrigger)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            WaveVR_Controller.EDeviceType controller = waveController.controller;
            WVR_InputId button = waveController.button;
            EditorGUILayout.LabelField("Button selected: " + button + " (" + waveController.controller + ")",EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("WaveVR works on a Dominant / Non-dominant scheme. Select buttons in the controller marked as R for input from the dominant controller and L for input from the non dominant controller.", MessageType.Info);
            EditorGUILayout.Separator();

            drawWaveButtonSelector(ref button, ref controller);
            
            EditorGUILayout.Separator();


            if (EditorGUI.EndChangeCheck())
            {
                foreach (WaveVRControllerTrigger o in targets)
                {
                    Undo.RecordObject(o, "");
                    o.button = button;
                    o.controller = controller;
                }
            }
            

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("More info on Vive Wave SDK site", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Go"))
            {
                Application.OpenURL("https://developer.vive.com/resources/mobile-vr/");
            }

#else
            EditorStyles.label.wordWrap = true;

            EditorGUILayout.HelpBox("Wave SDK not found or not activated. Please make sure the WaveVR SDK is imported and you have activated it via the VREasy/SDK Selector GUI", MessageType.Warning);
#endif
        }

#if VREASY_WAVEVR_SDK
        private void drawWaveButtonSelector(ref WVR_InputId defaultButton, ref WaveVR_Controller.EDeviceType controller)
        {
            Texture2D img = Resources.Load<Texture2D>("WaveVR_Controller");
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
            if (VREasy_utils.CreateOverlayGUIButton(170 / referenceSize * baseWidth, 25 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Touchpad;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(210 / referenceSize * baseWidth, 210 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Touchpad;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(230 / referenceSize * baseWidth, 135 / referenceSize * baseHeight, ref GraphicRect, "Left Trigger press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Trigger;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(155 / referenceSize * baseWidth, 325 / referenceSize * baseHeight, ref GraphicRect, "Right Trigger press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Trigger;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            /*if (VREasy_utils.CreateOverlayGUIButton(265 / referenceSize * baseWidth, 85 / referenceSize * baseHeight, ref GraphicRect, "Left Grip press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Grip;
                controller = WVR_DeviceType.WVR_DeviceType_Controller_Left;
            }

            if (VREasy_utils.CreateOverlayGUIButton(265 / referenceSize * baseWidth, 85 / referenceSize * baseHeight, ref GraphicRect, "Right Grip press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Grip;
                controller = WVR_DeviceType.WVR_DeviceType_Controller_Right;
            }*/

            if (VREasy_utils.CreateOverlayGUIButton(90 / referenceSize * baseWidth, 60 / referenceSize * baseHeight, ref GraphicRect, "Left Menu press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Menu;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(290 / referenceSize * baseWidth, 255 / referenceSize * baseHeight, ref GraphicRect, "Right Menu press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Menu;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(30 / referenceSize * baseWidth, 80 / referenceSize * baseHeight, ref GraphicRect, "Left System press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_System;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(335 / referenceSize * baseWidth, 275 / referenceSize * baseHeight, ref GraphicRect, "Right System press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_System;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(145 / referenceSize * baseWidth, 175 / referenceSize * baseHeight, ref GraphicRect, "Left Volume down press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Volume_Down;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(235 / referenceSize * baseWidth, 365 / referenceSize * baseHeight, ref GraphicRect, "Right Volume down press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Volume_Down;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(190 / referenceSize * baseWidth, 165 / referenceSize * baseHeight, ref GraphicRect, "Left Volume up press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Volume_Up;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 355 / referenceSize * baseHeight, ref GraphicRect, "Right Volume up press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_Volume_Up;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(245 / referenceSize * baseWidth, 60 / referenceSize * baseHeight, ref GraphicRect, "Left DLeft press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Left;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(345 / referenceSize * baseWidth, 60 / referenceSize * baseHeight, ref GraphicRect, "Left DRight press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Right;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(295 / referenceSize * baseWidth, 15 / referenceSize * baseHeight, ref GraphicRect, "Left DUp press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Up;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(295 / referenceSize * baseWidth, 105 / referenceSize * baseHeight, ref GraphicRect, "Left DDown press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Down;
                controller = WaveVR_Controller.EDeviceType.NonDominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(20 / referenceSize * baseWidth, 270 / referenceSize * baseHeight, ref GraphicRect, "Right DLeft press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Left;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(120 / referenceSize * baseWidth, 270 / referenceSize * baseHeight, ref GraphicRect, "Right DRight press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Right;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(75 / referenceSize * baseWidth, 220 / referenceSize * baseHeight, ref GraphicRect, "Right DUp press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Up;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }

            if (VREasy_utils.CreateOverlayGUIButton(75 / referenceSize * baseWidth, 320 / referenceSize * baseHeight, ref GraphicRect, "Right DDown press"))
            {
                defaultButton = WVR_InputId.WVR_InputId_Alias1_DPad_Down;
                controller = WaveVR_Controller.EDeviceType.Dominant;
            }


        }
        
#endif
    }


}