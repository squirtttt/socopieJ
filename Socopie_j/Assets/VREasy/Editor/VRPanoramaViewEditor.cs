using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

namespace VREasy
{
    [CustomEditor(typeof(VRPanoramaView))]
    public class VRPanoramaViewEditor : Editor
    {
        private static string _name;
        private static Sprite _idle;
        private static Sprite _selected;
        private static HOTSPOT_TYPE _hotspotType;
        private static SceneAsset _scene;
        private static Object _infoImage;
        private static DISPLAY_IMAGE_TYPE _infoImageType;
        private static VRSelector _selector = null;

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
            VRPanoramaView panorama = (VRPanoramaView)target;
            
            ConfigurePanoramaView(ref panorama);
        }

        public static void ConfigurePanoramaView(ref VRPanoramaView panorama)
        {
            EditorGUILayout.Separator();
            if (_selector == null)
            {
                _selector = FindObjectOfType<VRSelector>();
            }
            if(_selector == null)
            {
                EditorGUILayout.HelpBox("No VR Selector (Sight, Pointer or Touch) has been found in the scene, please make sure you add one to enable VR interactions with buttons", MessageType.Error);
            }

            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            PANORAMA_MODE mode = (PANORAMA_MODE)EditorGUILayout.EnumPopup("Mode", panorama.mode);
#pragma warning disable 0219
            bool destroyVideo = false;
            if (mode != panorama.mode)
            {
                panorama.UpdateMaterial(mode);
                if (mode != PANORAMA_MODE.VIDEO_FILE)
                {
                    destroyVideo = true;
                }
            }
#pragma warning restore 0219
            Texture2D image = null;
#if UNITY_5_6_OR_NEWER
            VideoClip clip = null;
            string videourl = "";
#endif
            Cubemap cube = null;
            if (mode != panorama.mode)
            {
                
            }
            switch (mode)
            {
                case PANORAMA_MODE.IMAGE:
                    {
                        image = (Texture2D)EditorGUILayout.ObjectField("Image", panorama.Image, typeof(Texture2D), true);
                    }
                    break;
                case PANORAMA_MODE.VIDEO_FILE:
                    {
#if UNITY_5_6_OR_NEWER
                        clip = (VideoClip)EditorGUILayout.ObjectField("Video", panorama.Video, typeof(VideoClip), true);
                        panorama.videoPlayer.source = VideoSource.VideoClip;
#else
                        EditorGUILayout.HelpBox("Video player is only supported in Unity 5.6 and above", MessageType.Error);
#endif
                    }
                    break;
                case PANORAMA_MODE.VIDEO_URL:
                    {
#if UNITY_5_6_OR_NEWER
                        videourl= EditorGUILayout.DelayedTextField("URL",panorama.VideoUrl);
                        panorama.videoPlayer.source = VideoSource.Url;
#else
                        EditorGUILayout.HelpBox("Video player is only supported in Unity 5.6 and above", MessageType.Error);
#endif
                    }
                    break;
                case PANORAMA_MODE.CUBEMAP:
                    {
                        cube = (Cubemap)EditorGUILayout.ObjectField("Cubemap", panorama.Cubemap, typeof(Cubemap), true);                        
                    }
                    break;
            }
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(panorama, "Panorama settings");
                panorama.mode = mode;
                panorama.Image = image;
                panorama.Cubemap = cube;
#if UNITY_5_6_OR_NEWER
                panorama.Video = clip;
                panorama.VideoUrl = videourl;
                if (destroyVideo)
                {
                    DestroyImmediate(panorama.videoPlayer);
                    GUIUtility.ExitGUI();
                }
#endif
            }
            // Create location loaders - load scene buttons

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Hotspot creator",EditorStyles.boldLabel);
            _name = EditorGUILayout.TextField("Name", _name);
            _idle = (Sprite)EditorGUILayout.ObjectField("Idle icon", _idle, typeof(Sprite), true);
            _selected = (Sprite)EditorGUILayout.ObjectField("Selected icon", _selected, typeof(Sprite), true);
            _hotspotType = (HOTSPOT_TYPE)EditorGUILayout.EnumPopup("Hotspot type", _hotspotType);
            
            EditorGUILayout.Separator();
            switch(_hotspotType)
            {
                case HOTSPOT_TYPE.INFO:
                    {
                        _infoImageType = (DISPLAY_IMAGE_TYPE)EditorGUILayout.EnumPopup("Image type", _infoImageType);
                        switch(_infoImageType)
                        {
                            case DISPLAY_IMAGE_TYPE.TEXTURE:
                                _infoImage = (Texture2D)EditorGUILayout.ObjectField("Info (sprite or texture)", _infoImage, typeof(Texture2D), true);
                                break;
                            case DISPLAY_IMAGE_TYPE.SPRITE:
                                _infoImage = (Sprite)EditorGUILayout.ObjectField("Info (sprite or texture)", _infoImage, typeof(Sprite), true);
                                break;
                        }
                    }
                    break;
                case HOTSPOT_TYPE.LOAD_LOCATION:
                    _scene = (SceneAsset)EditorGUILayout.ObjectField("Location scene", _scene, typeof(SceneAsset), true);
                    break;
            }
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Further options can be configured after creating hotspot", EditorStyles.wordWrappedLabel);

            EditorGUILayout.Separator();
            if (GUILayout.Button("Create"))
            {
                GameObject go = new GameObject(_name);
                VR2DButton button = go.AddComponent<VR2DButton>();
                button.IdleIcon = _idle;
                button.ActivateIcon = _idle;
                button.SelectIcon = _selected;
                button.faceDirection = VRELEMENT_FACE_DIRECTION.FORWARD;
                button.type = VRBUTTON_REFRESH_TYPE.BILLBOARD;
                VRAction action = null;
                switch (_hotspotType) {
                    case HOTSPOT_TYPE.INFO:
                        {
                            action = go.AddComponent<PopUpAction>();
                            ((PopUpAction)action).type = _infoImageType;
                            ((PopUpAction)action).Image = _infoImage;
                        }
                        break;
                    case HOTSPOT_TYPE.LOAD_LOCATION:
                        { 
                            action = go.AddComponent<LoadSceneAction>();
                            ((LoadSceneAction)action).SceneObject = _scene;
                        }
                        break;
                }
                button.actionList.list.Add(action);
                /*if(_selector == null)
                    VREasy_utils.SetGameObjectInScene(go);
                else
                    VREasy_utils.SetGameObjectInScene(go, _selector.GetComponentInChildren<Camera>());*/
                go.transform.position = Vector3.zero;
                _name = "";
                _idle = null;
                _selected = null;
                _scene = null;
                _infoImage = null;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}