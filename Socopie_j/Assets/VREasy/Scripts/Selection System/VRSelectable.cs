using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VREasy
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(ActionList))]
    public class VRSelectable : VRElement
    {
        public float coolDownTime = 2.0f;       // Time after activation in which the object cannot be selected again
        public float deactivationTime = 0.5f;   // Time after activation to return to idle state

        public AudioClip activateSound;
        public AudioClip selectSound;

        public string tooltip;
        public GameObject anchoredTooltipObject;
        public Text anchoredTooltipText;

        private AudioSource _audio
        {
            get
            {
                return GetComponent<AudioSource>();
            }
        }
        public ActionList actionList
        {
            get
            {
                if (_actionList == null) _actionList = GetComponent<ActionList>();
                return _actionList;
            }
        }
        
        protected ActionList _actionList;

        private float _lastSelectionTime = 0.0f;

        protected bool isPressed = false;
        protected bool isSelected = false;

#if UNITY_2017_1_OR_NEWER
        protected void Awake()
#else
        protected new void Awake()
#endif
        {
            Initialise();
            if (active) ReactivateElement();
            else DeactivateElement();
            if(init_transition > 0.0f)
            {
                if (active) Invoke("DeactivateElement", init_transition);
                else Invoke("ReactivateElement", init_transition);
            }
        }

        protected virtual void Initialise()
        {

        }

        public virtual bool CanSelectWithSight()
        {
            return true;
        }

        public virtual bool CanSelectWithTouch()
        {
            return true;
        }

        public virtual bool CanSelectWithPointer()
        {
            return true;
        }

        public virtual bool CanBeActivated()
        {
            return true;
        }

        // When a Selector initiates contact with IVRSelectable
        // returns whether the object is in fact selected (if it can be selected)
        public virtual bool select(VRSelector selector) {
            if (_lastSelectionTime + coolDownTime > Time.time)
            {
                return false;
            } else
            {
                // only play audio the first time
                if (!isPressed)
                    playSound(selectSound);
                Pressed(selector);
                isPressed = true;
                return true;
            }
        }

        // When a Selector stops contact with IVRSelectable
        public virtual void unselect() {
            Unselected();
            isPressed = false;
        }

        public virtual void stopInteraction()
        {
            Unpressed();
            isPressed = false;
        }

        // method that will be called when a Selector activates a IVRSelectable
        public virtual void activate() {
            if (!CanBeActivated()) return;
            if (_lastSelectionTime + coolDownTime > Time.time)
                return;
            playSound(activateSound);
            _lastSelectionTime = Time.time;
            Selection();
            isSelected = true;
            if(activation_transition > 0.0f) Invoke("DeactivateElement", activation_transition);
            Invoke("ToIdle", deactivationTime);
            Trigger();
        }

        private void ToIdle() {
            isSelected = false;
            Unselected();
        }

        private void playSound(AudioClip clip)
        {
            if (clip)
            {
                _audio.clip = clip;
                _audio.Play();
            }
        }

        // Methods to be overriden in children classes
        protected virtual void Selection()
        {
#if UNITY_EDITOR
            Debug.Log("Selected!");
#endif
        }

        protected virtual void Pressed(VRSelector selector)
        {
#if UNITY_EDITOR
            Debug.Log("Pressed!");
#endif
        }

        protected virtual void Unselected()
        {
#if UNITY_EDITOR
            Debug.Log("Unselected!");
#endif
        }

        protected virtual void Trigger()
        {
            // TRIGGER ACTION
            actionList.Trigger();
        }

        protected virtual void Unpressed()
        {
#if UNITY_EDITOR
            Debug.Log("Stopped interaction!");
#endif
        }

    }
}
