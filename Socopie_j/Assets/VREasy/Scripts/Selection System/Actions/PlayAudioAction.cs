using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayAudioAction : PlayAction
    {
        public AudioClip clip;

        private AudioSource _audio;

        void Start()
        {
            _audio = GetComponent<AudioSource>();
            _audio.clip = clip;
        }
        protected override void Play()
        {
            _audio.Play();
        }

        protected override void Stop()
        {
            _audio.Stop();
        }

        protected override bool getToggleState()
        {
            return _audio.isPlaying;
        }
    }
}