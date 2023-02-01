using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class PlayAnimationAction : PlayAction
    {
        public ANIMATION_TYPE type;

        // legacy animation
#pragma warning disable 0109
        public new Animation animation;
#pragma warning restore 0109
        public string clip;
        public float speed = 1.0f;

        // animator controllers
        public Animator animator;
        public string targetParameter;
        public ANIMATOR_PARAMETER_TYPE parameterType;
        public bool parameterValue_b;
        public float parameterValue_f;
        
        private void playAnimation(bool play)
        {
            if (!animation || string.IsNullOrEmpty(clip))
            {
                Debug.Log("PlayAnimationAction: target animation or clip not set");
                return;
            }
            try
            {
                if(speed < 0)
                {
                    animation[clip].speed = speed;
                    animation[clip].time = animation[clip].length;
                } else
                {
                    animation[clip].speed = speed;
                    animation[clip].time = 0;
                }
                
            } catch(System.Exception e)
            {
                Debug.LogError("PlayAnimation: Error, make sure clip [" + clip + "] exists in current model. " + e.ToString());
            }
            if (play) animation.Play(clip);
            else animation.Stop(clip);
                
        }

        private void playAnimator()
        {
            switch (parameterType) {
                case ANIMATOR_PARAMETER_TYPE.BOOL:
                    animator.SetBool(targetParameter, parameterValue_b);
                    break;
                case ANIMATOR_PARAMETER_TYPE.INT:
                    animator.SetInteger(targetParameter, (int)parameterValue_f);
                    break;
                case ANIMATOR_PARAMETER_TYPE.FLOAT:
                    animator.SetFloat(targetParameter, parameterValue_f);
                    break;
                case ANIMATOR_PARAMETER_TYPE.TRIGGER:
                    if(parameterValue_b)
                        animator.SetTrigger(targetParameter);
                    else
                        animator.ResetTrigger(targetParameter);
                    break;
            }
                
        }

        protected override void Play()
        {
            switch (type)
            {
                case ANIMATION_TYPE.LEGACY:
                    playAnimation(true);
                    break;
                case ANIMATION_TYPE.ANIMATOR:
                    playAnimator();
                    break;
            }
        }

        protected override void Stop()
        {
            switch (type)
            {
                case ANIMATION_TYPE.LEGACY:
                    playAnimation(false);
                    break;
                case ANIMATION_TYPE.ANIMATOR:
                    playAnimator();
                    break;
            }
        }


        protected override bool getToggleState()
        {
            switch (type)
            {
                case ANIMATION_TYPE.LEGACY:
                    return animation.isPlaying;
                default:
                    return false;
            }
        }
    }
}