using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OculusControllerTrigger))]
    public class OculusControllerTriggerEditor : Editor
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
            OculusControllerTrigger oculusController = (OculusControllerTrigger)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            OCULUS_CONTROLLER_INPUT_TYPE type = (OCULUS_CONTROLLER_INPUT_TYPE)EditorGUILayout.EnumPopup("Input type", oculusController.input_type);
            EditorGUILayout.Separator();
            OVRInput.Controller controller = oculusController.controller;
            EditorGUILayout.LabelField("Controller selected: " + controller);
            OVRInput.Button button = oculusController.button;
            OVRInput.Touch touch = oculusController.touch;
            switch(type)
            {
                case OCULUS_CONTROLLER_INPUT_TYPE.BUTTON:
                    {
                        touch = OVRInput.Touch.None;
                        //button = (OVRInput.Button)EditorGUILayout.EnumPopup("Button", oculusController.button);
                        EditorGUILayout.LabelField("Button selected: " + button);
                        drawOculusButtonSelector(ref button,ref controller);
                    }
                    break;
                case OCULUS_CONTROLLER_INPUT_TYPE.TOUCH:
                    {
                        button = OVRInput.Button.None;
                        //touch = (OVRInput.Touch)EditorGUILayout.EnumPopup("Touch", oculusController.touch);
                        EditorGUILayout.LabelField("Touch selected: " + touch);
                        drawOculusTouchSelector(ref touch, ref controller);
                    }
                    break;
            }

            EditorGUILayout.Separator();
            

            if (EditorGUI.EndChangeCheck())
            {
                foreach(OculusControllerTrigger o in targets)
                {
                    Undo.RecordObject(o, "");
                    o.controller = controller;
                    o.input_type = type;
                    o.button = button;
                    o.touch = touch;
                }
            }

            CheckWarningOculusSDKOrder();

            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("More info on Oculus Developer manual, Ref OVRInput",EditorStyles.wordWrappedLabel);
            if(GUILayout.Button("Go"))
            {
                Application.OpenURL("https://developer.oculus.com/documentation/unity/latest/concepts/unity-ovrinput/");
            }

#else
            EditorStyles.label.wordWrap = true;

            EditorGUILayout.HelpBox("Oculus Utilities not found or not activated. Please make sure the Oculus Utilities is imported and you have activated it via the VREasy/SDK Selector GUI", MessageType.Warning);
#endif
        }

#if VREASY_OCULUS_UTILITIES_SDK
        private void drawOculusButtonSelector(ref OVRInput.Button defaultButton, ref OVRInput.Controller controller)
        {
            Texture2D img = Resources.Load<Texture2D>("Oculus_Touch_button");
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
            if (VREasy_utils.CreateOverlayGUIButton(100 / referenceSize * baseWidth, 170 / referenceSize * baseHeight, ref GraphicRect, "Primary thumbstick"))
            {
                defaultButton = OVRInput.Button.PrimaryThumbstick;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(45 / referenceSize * baseWidth, 175 / referenceSize * baseHeight, ref GraphicRect, "Start"))
            {
                defaultButton = OVRInput.Button.Start;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(75 / referenceSize * baseWidth, 360 / referenceSize * baseHeight, ref GraphicRect, "Primary hand trigger"))
            {
                defaultButton = OVRInput.Button.PrimaryHandTrigger;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(30 / referenceSize * baseWidth, 235 / referenceSize * baseHeight, ref GraphicRect, "X (Three)"))
            {
                defaultButton = OVRInput.Button.Three;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(215 / referenceSize * baseWidth, 305 / referenceSize * baseHeight, ref GraphicRect, "Primary Index Trigger"))
            {
                defaultButton = OVRInput.Button.PrimaryIndexTrigger;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(205 / referenceSize * baseWidth, 350 / referenceSize * baseHeight, ref GraphicRect, "Y (Four)"))
            {
                defaultButton = OVRInput.Button.Four;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(260 / referenceSize * baseWidth, 295 / referenceSize * baseHeight, ref GraphicRect, "Left"))
            {
                defaultButton = OVRInput.Button.Left;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(360 / referenceSize * baseWidth, 295 / referenceSize * baseHeight, ref GraphicRect, "Right"))
            {
                defaultButton = OVRInput.Button.Right;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(310 / referenceSize * baseWidth, 255 / referenceSize * baseHeight, ref GraphicRect, "Up"))
            {
                defaultButton = OVRInput.Button.Up;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(310 / referenceSize * baseWidth, 340 / referenceSize * baseHeight, ref GraphicRect, "Down"))
            {
                defaultButton = OVRInput.Button.Down;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(265 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumbstick"))
            {
                defaultButton = OVRInput.Button.SecondaryThumbstick;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(320 / referenceSize * baseWidth, 45 / referenceSize * baseHeight, ref GraphicRect, "Back"))
            {
                defaultButton = OVRInput.Button.Back;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(310 / referenceSize * baseWidth, 215 / referenceSize * baseHeight, ref GraphicRect, "Secondary hand trigger"))
            {
                defaultButton = OVRInput.Button.SecondaryHandTrigger;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(355 / referenceSize * baseWidth, 110 / referenceSize * baseHeight, ref GraphicRect, "A (One)"))
            {
                defaultButton = OVRInput.Button.One;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(165 / referenceSize * baseWidth, 155 / referenceSize * baseHeight, ref GraphicRect, "Secondary index trigger"))
            {
                defaultButton = OVRInput.Button.SecondaryIndexTrigger;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(155 / referenceSize * baseWidth, 110 / referenceSize * baseHeight, ref GraphicRect, "B (Two)"))
            {
                defaultButton = OVRInput.Button.Two;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(80 / referenceSize * baseWidth, 15 / referenceSize * baseHeight, ref GraphicRect, "Up"))
            {
                defaultButton = OVRInput.Button.Up;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(80 / referenceSize * baseWidth, 100 / referenceSize * baseHeight, ref GraphicRect, "Down"))
            {
                defaultButton = OVRInput.Button.Down;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(30 / referenceSize * baseWidth, 55 / referenceSize * baseHeight, ref GraphicRect, "Left"))
            {
                defaultButton = OVRInput.Button.Left;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(125 / referenceSize * baseWidth, 55 / referenceSize * baseHeight, ref GraphicRect, "Right"))
            {
                defaultButton = OVRInput.Button.Right;
                controller = OVRInput.Controller.RTouch;
            }




        }

        private void drawOculusTouchSelector(ref OVRInput.Touch defaultTouch, ref OVRInput.Controller controller)
        {
            Texture2D img = Resources.Load<Texture2D>("Oculus_Touch_touch");
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
            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(100 / referenceSize * baseWidth, 170 / referenceSize * baseHeight, ref GraphicRect, "Primary thumbstick"))
            {
                defaultTouch = OVRInput.Touch.PrimaryThumbstick;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(30 / referenceSize * baseWidth, 235 / referenceSize * baseHeight, ref GraphicRect, "X (Three)"))
            {
                defaultTouch = OVRInput.Touch.Three;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(215 / referenceSize * baseWidth, 305 / referenceSize * baseHeight, ref GraphicRect, "Primary Index Trigger"))
            {
                defaultTouch = OVRInput.Touch.PrimaryIndexTrigger;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(205 / referenceSize * baseWidth, 350 / referenceSize * baseHeight, ref GraphicRect, "Y (Four)"))
            {
                defaultTouch = OVRInput.Touch.Four;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(75 / referenceSize * baseWidth, 355 / referenceSize * baseHeight, ref GraphicRect, "Primary thumbrest"))
            {
                defaultTouch = OVRInput.Touch.PrimaryThumbRest;
                controller = OVRInput.Controller.LTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(275 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumbstick"))
            {
                defaultTouch = OVRInput.Touch.SecondaryThumbstick;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(360 / referenceSize * baseWidth, 95 / referenceSize * baseHeight, ref GraphicRect, "A (One)"))
            {
                defaultTouch = OVRInput.Touch.One;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(165 / referenceSize * baseWidth, 140 / referenceSize * baseHeight, ref GraphicRect, "Secondary index trigger"))
            {
                defaultTouch = OVRInput.Touch.SecondaryIndexTrigger;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(155 / referenceSize * baseWidth, 95 / referenceSize * baseHeight, ref GraphicRect, "B (Two)"))
            {
                defaultTouch = OVRInput.Touch.Two;
                controller = OVRInput.Controller.RTouch;
            }

            if (VREasy_utils.CreateOverlayGUIButton(315 / referenceSize * baseWidth, 210 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumbrest"))
            {
                defaultTouch = OVRInput.Touch.SecondaryThumbRest;
                controller = OVRInput.Controller.RTouch;
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
                EditorGUILayout.HelpBox("For the engine to listen to Oculus controller input, Oculus SDK must be selected on top in your Virtual Reality SDKs list (PlayerSettings > XR settings > VirtualReality SDKs)", MessageType.Error);
            }
        }
        
    }

    
}