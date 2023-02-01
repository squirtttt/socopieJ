#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class PlayVideo : PlayAction
    {
#if UNITY_5_6_OR_NEWER
        public VIDEO_PLAYER_MODE Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                if (videoPlayer == null) return;
                switch (_mode)
                {
                    case VIDEO_PLAYER_MODE.FILE:
                        videoPlayer.source = VideoSource.VideoClip;
                        break;
                    case VIDEO_PLAYER_MODE.URL:
                        videoPlayer.source = VideoSource.Url;
                        break;
                }
            }
        }
        public VIDEO_PLAYER_MODE _mode = VIDEO_PLAYER_MODE.FILE;

        public VideoClip videoClip
        {
            get
            {
                return clip;
            }
            set
            {
                clip = value;
                if(videoPlayer !=null)
                {
                    videoPlayer.clip = clip;
                }
            }
        }
        public VideoClip clip;
        public string videoUrl
        {
            get
            {
                return _vidUrl;
            }
            set
            {
                _vidUrl = value;
                if (!string.IsNullOrEmpty(_vidUrl) && videoPlayer != null)
                {
                    videoPlayer.url = _vidUrl;
                }
                
            }
        }
        public string _vidUrl;

        public VideoPlayer videoPlayer;

        void Start()
        {
            if(videoPlayer != null) videoPlayer.clip = clip;
        }
        protected override void Play()
        {
            if (videoPlayer != null) videoPlayer.Play();
        }

        protected override void Stop()
        {
            if (videoPlayer != null)
            {
                StartCoroutine(stop());
            }
        }

        protected override bool getToggleState()
        {
            return (videoPlayer != null) ? videoPlayer.isPlaying : false;
        }

        IEnumerator stop()
        {
            videoPlayer.time = 0;
            yield return new WaitForSeconds(0.05f);
            videoPlayer.Pause();
            videoPlayer.frame = 0;
        }
#endif

    }
}
