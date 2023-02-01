using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace VREasy
{
    [CustomEditor(typeof(TouchSelector))]
    public class TouchSelectorEditor : Editor
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
            TouchSelector selector = (TouchSelector)target;

            ConfigureTouchSelector(selector);
            
        }

        public static void ConfigureTouchSelector(TouchSelector selector)
        {
            VRSelector sel = selector;
            VRSelectorEditor.ConfigureSelector(ref sel);

            GameObject obj = selector.gameObject;
            VRGrabTrigger.DisplayGrabTriggerSelector(ref selector.grabTrigger, ref obj);
            
        }
    }
}