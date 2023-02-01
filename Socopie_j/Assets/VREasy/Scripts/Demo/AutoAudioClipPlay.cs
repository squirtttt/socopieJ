using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{
    [RequireComponent(typeof(AudioSource))]
    public class AutoAudioClipPlay : MonoBehaviour
    {

        AudioSource source;
        AudioClip oldClip;

        // Use this for initialization
        void Start()
        {
            source = GetComponent<AudioSource>();
            oldClip = source.clip;
        }

        // Update is called once per frame
        void Update()
        {
            if(oldClip != source.clip)
            {
                source.Play();
                oldClip = source.clip;
            }
        }
    }
}