using UnityEngine;
using System.Collections;
using UnityEditor;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

namespace VREasy
{
    [CustomEditor(typeof(PlayVideo))]
    public class PlayVideoActionEditor : Editor
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
            PlayVideo playVideo = (PlayVideo)target;

#if UNITY_5_6_OR_NEWER

            EditorGUILayout.LabelField("Video options", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            VIDEO_PLAYER_MODE mode = (VIDEO_PLAYER_MODE)EditorGUILayout.EnumPopup("Mode", playVideo.Mode);
            VideoPlayer player = null;
            VideoClip clip = null;
            string url = "";
            player = (VideoPlayer)EditorGUILayout.ObjectField("Video player", playVideo.videoPlayer, typeof(VideoPlayer), true);
            switch (mode)
            {
                case VIDEO_PLAYER_MODE.FILE:
                    {
                        clip = (VideoClip)EditorGUILayout.ObjectField("Video clip", playVideo.videoClip, typeof(VideoClip), true);
                    }
                    break;
                case VIDEO_PLAYER_MODE.URL:
                    {
                        url = EditorGUILayout.DelayedTextField("Video URL", playVideo.videoUrl);
                    }
                    break;
            }

            EditorGUILayout.Separator();
            PLAY_ACTION type = (PLAY_ACTION)EditorGUILayout.EnumPopup("Action", playVideo.playType);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(playVideo, "play video mod");
                playVideo.Mode = mode;
                playVideo.videoClip = clip;
                playVideo.videoPlayer = player;
                playVideo.videoUrl = url;
                playVideo.playType = type;
            }
            //playType
#else
            
            DrawDefaultInspector();
#endif
        }
    }
}