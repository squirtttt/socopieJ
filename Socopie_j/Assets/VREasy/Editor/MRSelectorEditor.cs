using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace VREasy
{
    [CustomEditor(typeof(MRSelector))]
    public class MRSelectorEditor : Editor
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
            MRSelector selector = (MRSelector)target;

            ConfigureMRSelector(selector);
        }

        public static void ConfigureMRSelector(MRSelector selector)
        {
            // display common attributes with SightSelector
            SightSelectorEditor.ConfigureSightSelector(selector);

            // MRSelector specific attributes
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("MRSelector attributes", EditorStyles.boldLabel);

            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("You can set up separate triggers for grab and selection interactions", MessageType.Info);

            // selection trigger
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Selection trigger");
            GameObject go = selector.gameObject;
            VRGrabTrigger.DisplayGrabTriggerSelector(ref selector.selectionTrigger, ref go);
            if(selector.selectionTrigger == null)
            {
                EditorGUILayout.HelpBox("To be able to select objects, a trigger must be linked", MessageType.Error);
            }

            // selection trigger
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Grab trigger");
            VRGrabTrigger.DisplayGrabTriggerSelector(ref selector.grabTrigger, ref go);
            if (selector.grabTrigger == null)
            {
                EditorGUILayout.HelpBox("To be able to grab objects, a trigger must be linked", MessageType.Error);
            }

            // XRNode
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Reference when grabbing");
            MR_CONTROLLER refController = (MR_CONTROLLER)EditorGUILayout.EnumPopup("Reference", selector.controller);
#if UNITY_2017_2_OR_NEWER
            XRNode node = (selector.controllerNode);
#else
            VRNode node = (selector.controllerNode);
#endif
            switch (refController)
            {
                case MR_CONTROLLER.HOLOLENS_HAND:
                    {
#if !UNITY_2017_2_OR_NEWER || !UNITY_WSA

                        EditorGUILayout.HelpBox("To use Hololens hands, Unity 2017.2 or newer is required, and the build platform should be set to Universal Windows Platform", MessageType.Warning);
#else
                        EditorGUILayout.HelpBox("Physical hands will be used (Hololens only)", MessageType.Info);
#endif
                    }
                    break;
                case MR_CONTROLLER.MOTION_CONTROLLER:
                    {
#if UNITY_2017_2_OR_NEWER
                        node = (XRNode)EditorGUILayout.EnumPopup("OpenVR reference",selector.controllerNode);
#else
                        node = (VRNode)EditorGUILayout.EnumPopup("OpenVR reference",selector.controllerNode);
#endif
                        EditorGUILayout.HelpBox("This is the entity that is used to move and rotate VRGrabbable elements when grabbing", MessageType.Info);
                    }
                    break;
            }

            // body anchor
            EditorGUILayout.Separator();
            Transform anchor = (Transform)EditorGUILayout.ObjectField("Body anchor", selector.bodyAnchor, typeof(Transform),true);
            if (selector.bodyAnchor == null)
            {
                EditorGUILayout.HelpBox("Anchor used to calculate controller's world position (relative to the object). If not set, controller may appear static in position (0,0,0)", MessageType.Warning);
            } else
            {
                EditorGUILayout.HelpBox("Anchor used to calculate controller's world position (relative to that object). Controller position and rotation will appear with respect to this object", MessageType.Info);
            }


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selector, "controller node");
                selector.controllerNode = node;
                selector.bodyAnchor = anchor == null ? null : anchor.transform;
                selector.controller = refController;
            }


        }
    }
}