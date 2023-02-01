using UnityEngine;
using System.Collections;
using UnityEditor;


namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OculusRemoteTrigger))]
    public class OculusRemoteTriggerEditor : Editor
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
            OculusRemoteTrigger oculusController = (OculusRemoteTrigger)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            OVRInput.Button button = oculusController.button;
            EditorGUILayout.LabelField("Button selected: " + button);
            drawOculusRemoteButtonSelector(ref button);
            
            EditorGUILayout.Separator();


            if (EditorGUI.EndChangeCheck())
            {
                foreach (OculusRemoteTrigger o in targets)
                {
                    Undo.RecordObject(o, "");
                    o.button = button;
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
        private void drawOculusRemoteButtonSelector(ref OVRInput.Button defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Oculus_Remote");
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
            if (VREasy_utils.CreateOverlayGUIButton(85 / referenceSize * baseWidth, 40 / referenceSize * baseHeight, ref GraphicRect, "One press"))
            {
                defaultButton = OVRInput.Button.One;
            }
            if (VREasy_utils.CreateOverlayGUIButton(255 / referenceSize * baseWidth, 310 / referenceSize * baseHeight, ref GraphicRect, "Two press"))
            {
                defaultButton = OVRInput.Button.Two;
            }
            if (VREasy_utils.CreateOverlayGUIButton(295 / referenceSize * baseWidth, 15 / referenceSize * baseHeight, ref GraphicRect, "Dpad Up"))
            {
                defaultButton = OVRInput.Button.DpadUp;
            }
            if (VREasy_utils.CreateOverlayGUIButton(295 / referenceSize * baseWidth, 100 / referenceSize * baseHeight, ref GraphicRect, "Dpad Down"))
            {
                defaultButton = OVRInput.Button.DpadDown;
            }
            if (VREasy_utils.CreateOverlayGUIButton(250 / referenceSize * baseWidth, 55 / referenceSize * baseHeight, ref GraphicRect, "Dpad Left"))
            {
                defaultButton = OVRInput.Button.DpadLeft;
            }
            if (VREasy_utils.CreateOverlayGUIButton(340 / referenceSize * baseWidth, 55 / referenceSize * baseHeight, ref GraphicRect, "Dpad Right"))
            {
                defaultButton = OVRInput.Button.DpadRight;
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