using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

namespace VREasy
{
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class VRPanoramaView : MonoBehaviour
    {
        public PANORAMA_MODE mode = PANORAMA_MODE.IMAGE;
#if UNITY_5_6_OR_NEWER
        public VideoClip Video
        {
            get
            {
                return _video;
            }
            set
            {
                _video = value;
                videoPlayer.clip = _video;
            }
        }

        public VideoClip _video;

        public VideoPlayer videoPlayer
        {
            get
            {
                VideoPlayer player = GetComponent<VideoPlayer>();
                if (player == null)
                    player = gameObject.AddComponent<VideoPlayer>();
                return player;
            }
        }

        public string VideoUrl
        {
            get
            {
                return videoUrl;
            }
            set
            {
                videoUrl = value;
                if(!string.IsNullOrEmpty(videoUrl)) videoPlayer.url = videoUrl;
            }
        }

        public string videoUrl;
#endif
        public Texture2D Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                Rend.sharedMaterial.mainTexture = _image;
            }
        }

        public Texture2D _image;

        public Cubemap Cubemap
        {
            get
            {
                return _cubemap;
            }
            set
            {
                _cubemap = value;
                Rend.sharedMaterial.SetTexture("_Cube", _cubemap);
            }
        }
        public Cubemap _cubemap;

        public List<LoadSceneAction> Locations = new List<LoadSceneAction>();

        private MeshRenderer Rend
        {
            get
            {
                return GetComponent<MeshRenderer>();
            }
        }

        // Use this for initialization
        void Awake()
        {
            Image = _image;
#if UNITY_5_6_OR_NEWER
            // Warning users from Unity Editor bug related to playing videos on awake on materials other than Unlit.
            if (mode == PANORAMA_MODE.VIDEO_FILE && !Rend.sharedMaterial.shader.ToString().Contains("Unlit") && videoPlayer.playOnAwake)
            {
                Debug.LogWarning("[VREasy]: Play On Awake Video detected. Warning, the video may not play on awake when loading the scene from a different scene in the Unity Editor with the current shader (" + Rend.sharedMaterial.shader.ToString() + "). This does not affect built scenes and videos play as expected (Unity BUG)");
            }
#endif
        }

        void Update()
        {
            Image = _image;
        }

        public void UpdateMaterial(PANORAMA_MODE mode)
        {
            if(mode == PANORAMA_MODE.CUBEMAP)
            {
                Rend.sharedMaterial = Resources.Load<Material>("PanoramaSkybox");
            } else
            {
                Rend.sharedMaterial = Resources.Load<Material>("PanoramaFlat");
            }
        }
    }
}