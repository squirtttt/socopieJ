using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{
    public class Transform2AnimationController : Transform2Component
    {
        public ANIMATION_TYPE animationType = ANIMATION_TYPE.ANIMATOR;
        public ANIMATION_TARGET controlType = ANIMATION_TARGET.ANIMATION_FRAME;

        public Vector2 rangeValues = Vector2.zero;

        // ANIMATION
        public Animation animationComponent;
        public AnimationClip selectedAnimation;

        // ANIMATOR
        public Animator animatorComponent;
        public string animatorState = "";
        public int animatorLayer = 0;
        private float storedSpeed;
        private bool animationEnded = false;

        // PARAMETER ANIMATOR
        public string targetParameter;
        private float parameterValue;

        private float previousValue = 0f;


        // Use this for initialization
        void Start()
        {
            bool initError = false;
            switch(animationType)
            {
                case ANIMATION_TYPE.ANIMATOR:
                    {
                        if (animatorComponent == null)
                        {
                            Debug.LogWarning("[VREasy] Transform2Animation (" + gameObject.name + "): Animator component not set. Disabling script in " + gameObject.name);
                            initError = true;
                        } else
                        {
                            storedSpeed = animatorComponent.speed;
                        }
                    }
                    break;
                case ANIMATION_TYPE.LEGACY:
                    {
                        if (animationComponent == null)
                        {
                            Debug.LogWarning("[VREasy] Transform2Animation (" + gameObject.name + "): Animation component not set. Disabling script in " + gameObject.name);
                            initError = true;
                        }
                    }
                    break;
            }
            enabled = !initError;            
        }

        protected override void SetDestinationChange(float valueChange)
        {
            float cappedValue = valueChange;
            if(rangeValues.x <= rangeValues.y) // accounting for reverse ranges
            {
                cappedValue = Mathf.Clamp(GetCurrentOriginValue(), rangeValues.x, rangeValues.y); // inside rangeValues
                cappedValue = (cappedValue - rangeValues.x) / (rangeValues.y - rangeValues.x); //between 0 and 1 if within range   
                
            } else
            {
                cappedValue = Mathf.Clamp(GetCurrentOriginValue(), rangeValues.y, rangeValues.x); // inside rangeValues
                cappedValue = (cappedValue - rangeValues.y) / (rangeValues.x - rangeValues.y); //between 0 and 1 if within range
                cappedValue = 1 - cappedValue;
            }


            if (previousValue == cappedValue)
            {
                // value has not change this frame, consider controller stopped
                switch (animationType)
                {
                    case ANIMATION_TYPE.ANIMATOR:
                        {
                            stopAnimator();
                            
                        }
                        break;
                    case ANIMATION_TYPE.LEGACY:
                        {
                            stopAnimation();
                            
                        }
                        break;
                }
                return;
            }

            // Update controlled value
            previousValue = cappedValue;

            if (cappedValue < 1 && cappedValue > 0)
            {
                switch (animationType)
                {
                    case ANIMATION_TYPE.ANIMATOR:
                        {
                            startAnimator(cappedValue);
                            
                        }
                        break;
                    case ANIMATION_TYPE.LEGACY:
                        {
                            startAnimation(cappedValue);
                            
                        }
                        break;
                }
            } 

        }

        private void startAnimation(float cappedValue)
        {
            if (selectedAnimation != null)
            {
                switch(controlType)
                {
                    case ANIMATION_TARGET.ANIMATION_FRAME:
                        {
                            animationComponent.clip = selectedAnimation;
                            if (!animationComponent.isPlaying) animationComponent.Play();
                            animationComponent[selectedAnimation.name].time = cappedValue * animationComponent[selectedAnimation.name].length;
                        }
                        break;
                    case ANIMATION_TARGET.ANIMATION_SPEED:
                        {
                            if (animationComponent.clip != selectedAnimation)
                            {
                                animationComponent.Stop();
                                animationEnded = false;
                            }
                            animationComponent.clip = selectedAnimation;
                            if (animationComponent[selectedAnimation.name].normalizedTime > 0.95f)
                            {
                                animationEnded = true;
                            }
                            if (!animationEnded && !animationComponent.isPlaying) animationComponent.Play();
                            animationComponent[selectedAnimation.name].speed = cappedValue;
                        }
                        break;
                }
                
            }
        }

        private void startAnimator(float cappedValue)
        {
            if (animatorComponent != null)
            {
                switch (controlType)
                {
                    case ANIMATION_TARGET.ANIMATION_FRAME:
                        {
                            animatorComponent.speed = storedSpeed;
                            animatorComponent.Play(animatorState, animatorLayer, cappedValue);
                        }
                        break;
                    case ANIMATION_TARGET.ANIMATION_SPEED:
                        {
                            animatorComponent.speed = cappedValue;
                            animatorComponent.Play(animatorState, animatorLayer);
                        }
                        break;
                    case ANIMATION_TARGET.NUMERIC_PARAMETER:
                        {
                            animatorComponent.SetFloat(targetParameter, cappedValue);
                        }
                        break;
                }
                
            }
        }

        private void stopAnimation() {
            if (selectedAnimation != null)
            {
                switch (controlType)
                {
                    case ANIMATION_TARGET.ANIMATION_FRAME:
                        {
                            if (animationComponent.clip == selectedAnimation) animationComponent.Stop();
                        }
                        break;
                    case ANIMATION_TARGET.ANIMATION_SPEED:
                        {
                            
                        }
                        break;
                }
                
            }
        }

        private void stopAnimator()
        {
            if (animatorComponent != null)
            {
                switch (controlType)
                {
                    case ANIMATION_TARGET.ANIMATION_FRAME:
                        {
                            animatorComponent.speed = 0f;
                        }
                        break;
                    case ANIMATION_TARGET.ANIMATION_SPEED:
                        {

                        }
                        break;
                }
                
            }
        }
    }
}