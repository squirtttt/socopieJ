using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(WaveVRPoseController))]
    public class WaveVRPoseControllerEditor : Editor
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
            DrawDefaultInspector();
#else
            EditorGUILayout.HelpBox("WaveVR SDK inspector must be imported in the project and the WaveVR SDK integration must be active in SDK Checker to use WaveVR Pose tracking.", MessageType.Error);
#endif

        }
    }
}