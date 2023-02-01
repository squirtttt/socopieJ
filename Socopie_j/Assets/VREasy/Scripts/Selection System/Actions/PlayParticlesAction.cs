using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class PlayParticlesAction : PlayAction
    {
#pragma warning disable 0109
        public new ParticleSystem particleSystem;
#pragma warning restore 0109

        protected override void Play()
        {
            if (!particleSystem)
            {
                Debug.Log("PlayParticlesAction: Particle system not set");
                return;
            }
            particleSystem.Play();
        }

        protected override void Stop()
        {
            if (!particleSystem)
            {
                Debug.Log("PlayParticlesAction: Particle system not set");
                return;
            }
            particleSystem.Stop();
        }

        protected override bool getToggleState()
        {
            if (particleSystem == null) return false;
            return particleSystem.isPlaying;
        }
    }
}