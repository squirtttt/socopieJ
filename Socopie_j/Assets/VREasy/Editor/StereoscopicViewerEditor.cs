using UnityEngine;
using System.Collections;
using UnityEditor;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

namespace VREasy
{
    [CustomEditor(typeof(StereoscopicViewer))]
    public class StereoscopicViewerEditor : Editor
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

            StereoscopicViewer stereoscopic = (StereoscopicViewer)target;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            STEREOSCOPIC_MODE mode = (STEREOSCOPIC_MODE)EditorGUILayout.EnumPopup("Mode", stereoscopic.mode);

            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Eye Layers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Select different layers for each eye (you may need to create new layers in your Unity Project)", MessageType.Warning);
            int leftEyeLayer = EditorGUILayout.LayerField("Left eye", stereoscopic.leftEyeLayer);
            int rightEyeLayer = EditorGUILayout.LayerField("Right eye", stereoscopic.rightEyeLayer);

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(stereoscopic, "Main parameters");
                stereoscopic.leftEyeLayer = leftEyeLayer;
                stereoscopic.rightEyeLayer = rightEyeLayer;
                if (mode != stereoscopic.mode) {
                    if(mode != STEREOSCOPIC_MODE.VIDEO)
                    {
                        stereoscopic.ClearVideoPlayers();
                    } else
                    {
                        stereoscopic.SetVideoPlayers();
                    }
                }
                stereoscopic.mode = mode;
            }
            
            switch (stereoscopic.mode)
            {
                case STEREOSCOPIC_MODE.IMAGE:
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("Stereoscopic images", EditorStyles.boldLabel);
                        Texture2D leftEyeImage = (Texture2D)EditorGUILayout.ObjectField("Left eye", stereoscopic.leftEyeImage, typeof(Texture2D), true);
                        Texture2D rightEyeImage = (Texture2D)EditorGUILayout.ObjectField("Right eye", stereoscopic.rightEyeImage, typeof(Texture2D), true);

                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("Image configuration", EditorStyles.boldLabel);
                        bool autoDetectSize = EditorGUILayout.Toggle("Autodetect image size", stereoscopic.autodetectImageSize);
                        float imageScale = EditorGUILayout.FloatField("Image scale", stereoscopic.ImageScale);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(stereoscopic, "Stereoscopic properties");

                            stereoscopic.LeftEyeImage = leftEyeImage;
                            stereoscopic.RightEyeImage = rightEyeImage;
                            stereoscopic.autodetectImageSize = autoDetectSize;
                            stereoscopic.ImageScale = imageScale;
                        }
                    }
                    break;
                case STEREOSCOPIC_MODE.VIDEO:
                    
#if UNITY_5_6_OR_NEWER
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("Stereoscopic videos", EditorStyles.boldLabel);
                        VideoClip leftEyeClip = (VideoClip)EditorGUILayout.ObjectField("Left eye", stereoscopic.LeftEyeClip, typeof(VideoClip), true);
                        VideoClip rightEyeClip = (VideoClip)EditorGUILayout.ObjectField("Right eye", stereoscopic.RightEyeClip, typeof(VideoClip), true);

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(stereoscopic, "Stereoscopic properties");

                            stereoscopic.LeftEyeClip = leftEyeClip;
                            stereoscopic.RightEyeClip = rightEyeClip;
                        }
                    }
#else
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("Video Player requires Unity 5.6 or above", MessageType.Error);
                    }
#endif
                    break;
            }
            

        }
    }
}