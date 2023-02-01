using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotMakerEditor : Editor
    {

        [MenuItem("VREasy/Components/Screenshot maker")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<ScreenshotMaker>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add a ScreenshotMaker script you must select a game object in the hierarchy first", "OK");
            }
        }

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

            ScreenshotMaker screenshotmaker = (ScreenshotMaker)target;

            EditorGUI.BeginChangeCheck();

            string filename = EditorGUILayout.TextField("Filename", screenshotmaker.filename);

            EditorGUILayout.Separator();
            AudioClip soundEffect = (AudioClip)EditorGUILayout.ObjectField("Sound effect", screenshotmaker.soundEffect, typeof(AudioClip), true);

            GameObject gm = screenshotmaker.gameObject;
            EditorGUILayout.Separator();
            VRGrabTrigger.DisplayGrabTriggerSelector(ref screenshotmaker.trigger, ref gm);

            EditorGUILayout.Separator();
            int superSize = EditorGUILayout.IntSlider("Resolution multiplier", screenshotmaker.screenshotMultiplier,1,10);

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(screenshotmaker, "screenshot maker options");
                screenshotmaker.filename = filename;
                screenshotmaker.screenshotMultiplier = superSize;
                screenshotmaker.soundEffect = soundEffect;
            }

            if(screenshotmaker.gameObject.GetComponent<Camera>() == null)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("Camera not detected in current game object. Camera-shot image effect will not be applied. For some platforms (GearVR) ScreenshotMaker must be attached to a Camera", MessageType.Warning);
            }

        }
    }
}