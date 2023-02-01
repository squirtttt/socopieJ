using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GoogleVRControllerTrigger))]
    public class GoogleVRControllerTriggerEditor : Editor
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

#if VREASY_GOOGLEVR_SDK
            GoogleVRControllerTrigger controller = (GoogleVRControllerTrigger)target;
            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            GvrControllerButton input = controller.button;
            GvrControllerHand device = controller.device;
            EditorGUILayout.LabelField("Current button: " + input);
            EditorGUILayout.LabelField("Current device: " + device);
            EditorGUILayout.Separator();

            drawAndSelectGoogleVRInputSelector(ref input, ref device);
            if (EditorGUI.EndChangeCheck())
            {
                foreach(GoogleVRControllerTrigger g in targets)
                {
                    Undo.RecordObject(g, "changed googlevr controller");
                    g.button = input;
                    g.device = device;
                }
            }

#else
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.HelpBox("Google VR SDK not found or not activated. Please make sure the Google VR SDK is imported and you have activated it via the VREasy/SDK Selector GUI", MessageType.Warning);
            
#endif
        }

#if VREASY_GOOGLEVR_SDK
        private void drawAndSelectGoogleVRInputSelector(ref GvrControllerButton defaultButton, ref GvrControllerHand device)
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
            float baseX = GraphicRect.x;
            float baseY = GraphicRect.y;
            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(200 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad touch"))
            {
                defaultButton = GvrControllerButton.TouchPadTouch;
                device = GvrControllerHand.Right;
            }

            if (VREasy_utils.CreateOverlayGUIButton(280 / referenceSize * baseWidth, 50 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad press"))
            {
                defaultButton = GvrControllerButton.TouchPadButton;
                device = GvrControllerHand.Right;
            }

            if (VREasy_utils.CreateOverlayGUIButton(240 / referenceSize * baseWidth, 165 / referenceSize * baseHeight, ref GraphicRect, "Right App button"))
            {
                defaultButton = GvrControllerButton.App;
                device = GvrControllerHand.Right;
            }

            if (VREasy_utils.CreateOverlayGUIButton(195 / referenceSize * baseWidth, 195 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad touch"))
            {
                defaultButton = GvrControllerButton.TouchPadTouch;
                device = GvrControllerHand.Left;
            }

            if (VREasy_utils.CreateOverlayGUIButton(110 / referenceSize * baseWidth, 220 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad press"))
            {
                defaultButton = GvrControllerButton.TouchPadButton;
                device = GvrControllerHand.Left;
            }

            if (VREasy_utils.CreateOverlayGUIButton(155 / referenceSize * baseWidth, 355 / referenceSize * baseHeight, ref GraphicRect, "Left App button"))
            {
                defaultButton = GvrControllerButton.App;
                device = GvrControllerHand.Left;
            }

        }
#endif
    }
}