using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.XR;

namespace VREasy
{
    [CustomEditor(typeof(MouseHMD))]
    public class MouseHMDEditor : Editor
    {
        [MenuItem("VREasy/Components/Mouse HMD")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<MouseHMD>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add mouse HMD controls must select a game object in the hierarchy first", "OK");
            }
        }

        public override void OnInspectorGUI()
        {
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.HelpBox("Ensure Virtual Reality Support is disabled to use MouseHMD", MessageType.Warning);
#else
            if (PlayerSettings.virtualRealitySupported)
            {
                EditorGUILayout.HelpBox("To be able to control the camera with the mouse, the Virtual Reality Supported option must be disabled in Player Settings", MessageType.Error);
            }
            PlayerSettings.virtualRealitySupported = EditorGUILayout.Toggle("VR Supported", PlayerSettings.virtualRealitySupported);
#endif

            DrawDefaultInspector();
        }

    }
}