using UnityEngine;
using System.Collections;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

namespace VREasy
{
#if UNITY_5_6_OR_NEWER
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(AudioSource))]
    public class MoviePlayer : MonoBehaviour
    {
        private VideoPlayer videoPlayer;

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());
        }

        public void PlayMovie()
        {
            videoPlayer.Play();
        }

        public void PauseMovie()
        {
            videoPlayer.Pause();
        }

        public void StopMovie()
        {
            StartCoroutine(stop());
        }

        IEnumerator stop()
        {
            videoPlayer.time = 0;
            yield return new WaitForSeconds(0.05f);
            videoPlayer.Pause();
            videoPlayer.frame = 0;
        }
    }

#else
    public class MoviePlayer : MonoBehaviour
    {
        // Backwards compatibility for Unity 5.4 and 5.5 (MovieTexture for standalone)
#if UNITY_STANDALONE
        private MovieTexture movie;
#endif
#pragma warning disable 0109
        private new AudioSource audio;
#pragma warning restore 0109
        void Start()
        {
#if UNITY_STANDALONE
            Renderer r = GetComponent<Renderer>();
            movie = (MovieTexture)r.material.mainTexture;
#endif
            audio = GetComponent<AudioSource>();
        }

        public void PlayMovie()
        {
#if UNITY_EDITOR
            Debug.Log("Play movie");
#endif
#if UNITY_STANDALONE
            if (movie) movie.Play();
#endif
            if (audio) audio.Play();
        }
        public void PauseMovie()
        {
#if UNITY_EDITOR
            Debug.Log("Pause movie");
#endif
#if UNITY_STANDALONE
            if (movie) movie.Pause();
#endif
            if (audio) audio.Pause();
        }

        public void StopMovie()
        {
#if UNITY_EDITOR
            Debug.Log("Stop movie");
#endif
#if UNITY_STANDALONE
            if (movie) movie.Stop();
#endif
            if (audio) audio.Stop();
        }

    }
#endif
}