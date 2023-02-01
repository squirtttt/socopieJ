using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OculusGoControllerTrigger))]
    public class OculusGoControllerTriggerEditor : Editor
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

#if VREASY_OCULUS_UTILITIES_SDK
            OculusGoControllerTrigger oculusController = (OculusGoControllerTrigger)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            OVRInput.Button button = oculusController.button;
            OVRInput.Touch touch = oculusController.touch;
            OCULUS_CONTROLLER_INPUT_TYPE type = oculusController.input_type;
            switch(type)
            {
                case OCULUS_CONTROLLER_INPUT_TYPE.BUTTON:
                    EditorGUILayout.LabelField("Button selected: " + button);
                    break;
                case OCULUS_CONTROLLER_INPUT_TYPE.TOUCH:
                    EditorGUILayout.LabelField("Touch selected: " + touch);
                    break;
            }
            drawOculusGoButtonSelector(ref button, ref touch, ref type);
            
            EditorGUILayout.Separator();


            if (EditorGUI.EndChangeCheck())
            {
                foreach (OculusGoControllerTrigger o in targets)
                {
                    Undo.RecordObject(o, "");
                    o.button = button;
                    o.touch = touch;
                    o.input_type = type;
                }
            }

            CheckWarningOculusSDKOrder();


            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("More info on Oculus Developer manual, Ref OVRInput", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Go"))
            {
                Application.OpenURL("https://developer.oculus.com/documentation/unity/latest/concepts/unity-ovrinput/");
            }

#else
            EditorStyles.label.wordWrap = true;

            EditorGUILayout.HelpBox("Oculus Utilities not found or not activated. Please make sure the Oculus Utilities is imported and you have activated it via the VREasy/SDK Selector GUI", MessageType.Warning);
#endif
        }

#if VREASY_OCULUS_UTILITIES_SDK
        private void drawOculusGoButtonSelector(ref OVRInput.Button defaultButton, ref OVRInput.Touch touch, ref OCULUS_CONTROLLER_INPUT_TYPE type)
        {
            Texture2D img = Resources.Load<Texture2D>("Oculus_Go_Controller");
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
            if (VREasy_utils.CreateOverlayGUIButton(265 / referenceSize * baseWidth, 85 / referenceSize * baseHeight, ref GraphicRect, "Primary touchpad click"))
            {
                defaultButton = OVRInput.Button.PrimaryTouchpad;
                type = OCULUS_CONTROLLER_INPUT_TYPE.BUTTON;
            }

            if (VREasy_utils.CreateOverlayGUIButton(345 / referenceSize * baseWidth, 95 / referenceSize * baseHeight, ref GraphicRect, "Primary touchpad touch"))
            {
                touch = OVRInput.Touch.PrimaryTouchpad;
                type = OCULUS_CONTROLLER_INPUT_TYPE.TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(330 / referenceSize * baseWidth, 295 / referenceSize * baseHeight, ref GraphicRect, "Primary Index Trigger"))
            {
                defaultButton = OVRInput.Button.PrimaryIndexTrigger;
                type = OCULUS_CONTROLLER_INPUT_TYPE.BUTTON;
            }

            if (VREasy_utils.CreateOverlayGUIButton(190 / referenceSize * baseWidth, 80 / referenceSize * baseHeight, ref GraphicRect, "Back button"))
            {
                defaultButton = OVRInput.Button.Back;
                type = OCULUS_CONTROLLER_INPUT_TYPE.BUTTON;
            }
            

        }
        
#endif

        public static void CheckWarningOculusSDKOrder()
        {
            // If using Oculus input, check that Oculus SDK is on top of Virtual Reality SDKs in PlayerSettings
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.XRSettings.supportedDevices.Length == 0) return;
#else
            if (UnityEngine.VR.VRSettings.supportedDevices.Length == 0) return;
#endif

            EditorGUILayout.Separator();
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.XRSettings.supportedDevices[0] != "Oculus")
#else
            if (UnityEngine.VR.VRSettings.supportedDevices[0] != "Oculus")
#endif

            {
                EditorGUILayout.HelpBox("For the engine to listen to Oculus controller input, Oculus SDK must be selected on top in your Virtual Reality SDKs list (PlayerSettings > Other settings > VirtualReality SDKs)", MessageType.Error);
            }
        }
    }


}