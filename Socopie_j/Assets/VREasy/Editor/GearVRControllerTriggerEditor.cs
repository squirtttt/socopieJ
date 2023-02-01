using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GearVRControllerTrigger))]
    public class GearVRControllerTriggerEditor : Editor
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
            GearVRControllerTrigger controller = (GearVRControllerTrigger)target;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Current input: " + controller.button);
            EditorGUI.BeginChangeCheck();

            GEARVR_CONTROLLER_INPUT button = controller.button;
            drawAndSelectViveInputSelector(ref button);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (GearVRControllerTrigger sg in targets)
                {
                    Undo.RecordObject(sg, "change of button");
                    sg.button = button;
                }
            }
#else
            EditorGUILayout.HelpBox("Oculus Utilities not found or not activated. Please make sure the Oculus Utilities is imported and you have activated it via the VREasy/SDK Selector GUI", MessageType.Warning);
            
#endif
        }

#if VREASY_OCULUS_UTILITIES_SDK
        private void drawAndSelectViveInputSelector(ref GEARVR_CONTROLLER_INPUT defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("GearVR_Controller");
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
            if (VREasy_utils.CreateOverlayGUIButton(315 / referenceSize * baseWidth, 270 / referenceSize * baseHeight, ref GraphicRect, "Index trigger"))
            {
                defaultButton = GEARVR_CONTROLLER_INPUT.INDEX_TRIGGER;
            }

            if (VREasy_utils.CreateOverlayGUIButton(315 / referenceSize * baseWidth, 70 / referenceSize * baseHeight, ref GraphicRect, "Touchpad touch"))
            {
                defaultButton = GEARVR_CONTROLLER_INPUT.TOUCHPAD_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(165 / referenceSize * baseWidth, 50 / referenceSize * baseHeight, ref GraphicRect, "Touchpad press"))
            {
                defaultButton = GEARVR_CONTROLLER_INPUT.TOUCHPAD_PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(70 / referenceSize * baseWidth, 130 / referenceSize * baseHeight, ref GraphicRect, "Touchpad press"))
            {
                defaultButton = GEARVR_CONTROLLER_INPUT.BACK_BUTTON;
            }

        }
#endif
    }
}