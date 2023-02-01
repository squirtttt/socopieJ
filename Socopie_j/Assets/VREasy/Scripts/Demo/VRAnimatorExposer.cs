using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class VRAnimatorExposer : MonoBehaviour
    {
        public ANIMATION_TYPE type = ANIMATION_TYPE.ANIMATOR;

        public float AnimationTime
        {
            set
            {
                animationTime = value;

            }
            get
            {
                return animationTime;
            }
        }
        public float animationTime = 0;

        public Animator animator
        {
            get
            {
                if (_animator == null) _animator = GetComponent<Animator>();
                return _animator;
            }
        }
        private Animator _animator;

        public AnimationClip selectedAnimation;

        public new Animation animation
        {
            get
            {
                if (_animation == null) _animation = GetComponent<Animation>();
                return _animation;
            }
        }
        private Animation _animation;

        private void Update()
        {
            try
            {
                switch (type)
                {
                    case ANIMATION_TYPE.ANIMATOR:
                        animator.Play("", 0, animationTime);
                        break;
                    case ANIMATION_TYPE.LEGACY:
                        animation[animation.clip.name].normalizedTime = animationTime;
                        animation[animation.clip.name].speed = 0;
                        animation.Play();
                        break;
                }
            } catch(System.Exception e)
            {
                Debug.LogWarning("[VREasy] VRAnimatorExposer error: " + e.ToString());
            }
            
            
        }
    }
}