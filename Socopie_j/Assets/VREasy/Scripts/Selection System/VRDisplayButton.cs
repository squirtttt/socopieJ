using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if VREASY_VREE_PLATFORM_SDK
using VREasy.Networking;
#endif

namespace VREasy
{
    [RequireComponent(typeof(SwitchAction))]
#if VREASY_VREE_PLATFORM_SDK
    public class VRDisplayButton : VR2DButton, IVRDisplayButtonNetworkEvents
#else
    public class VRDisplayButton : VR2DButton
#endif
    {
#if BATTLE_HUD_SDK
        public List<Texture2D> representationsTex = new List<Texture2D>();
#endif
        public List<Sprite> representations = new List<Sprite>();
        public float timeToHide = 5.0f;
        //public float hideAfterActivationTime = 1.0f;
        public event Action<string> OnSwitchChild;

        private List<VRDisplayButtonChild> buttonRepresentations = new List<VRDisplayButtonChild>();

        public SwitchAction Action
        {
            get
            {
                return GetComponent<SwitchAction>();
            }
        }

        
        

        protected override void Initialise()
        {
            base.Initialise();
            coolDownTime = 0.0f;
            deactivationTime = 0.0f;
            if (actionList.list.Count != 1)
            {
                actionList.list.Clear();
                actionList.list.Add(Action);
            }
        }

        protected override void Selection()
        {
            base.Selection();
            CreateChildButtons();
        }

        protected override void Pressed(VRSelector selector)
        {
            base.Pressed(selector);
            ResetHideTimer();
        }
        
        protected override void Trigger()
        {
            // do nothing
        }

        protected override void SetState()
        {

            // set select icon when active / selected; idle otherwise
            /*if (_renderer == null)
            {
                _renderer = getRenderer();
                if (_renderer == null) return;
            }*/
            // switch between states
            if (isSelected || isPressed)
            {
                // SELECTED
                //_renderer.sprite = selectIcon;
                setSprite(selectIcon);
            }
            else
            {
                // IDLE
                //_renderer.sprite = idleIcon;
                setSprite(idleIcon);
            }
        }

        public void SetRepresentationLength(int length)
        {
            int diff = representations.Count - length;
            if(diff > 0)
            {
                representations.RemoveRange(length, diff);
#if BATTLE_HUD_SDK
                representationsTex.RemoveRange(length, diff);
#endif
            } else if(diff < 0)
            {
                for (int i = 0; i < Mathf.Abs(diff); i++)
                {
                    representations.Add(null);
#if BATTLE_HUD_SDK
                    representationsTex.Add(null);
#endif
                }
            }
        }

        public void SwitchChild(string index)
        {
            if(OnSwitchChild != null) OnSwitchChild.Invoke(index);

            // switch the element in Action[index]
            int i;
            if (int.TryParse(index, out i))
            {
                Action.SwapTo(i);
                //Invoke("hide", hideAfterActivationTime);
            }
            CancelInvoke("hideChildren");
            Invoke("hideChildren", 0.5f);
        }

        public void ResetHideTimer()
        {
            // start timeToHide (1s default)
            CancelInvoke("hideChildren");
            Invoke("hideChildren", timeToHide);
        }

        private void CreateChildButtons()
        {
            if (buttonRepresentations.Count > 0)
                return;
            // instead of triggering action list, show options
            float angle = 360.0f / representations.Count;
            //float size = _localScale * (0.5f / Mathf.Clamp(representations.Count * 0.13f, 0.50f, 0.90f));// 0.65f;
            //float position = Mathf.Clamp(representations.Count * 0.09f, 0.30f, 0.75f);// * _localScale;
            float size =  _localScale * (0.25f / Mathf.Clamp(representations.Count * 0.13f,0.50f,0.90f));// 0.65f;
            float position =  Mathf.Clamp(representations.Count * 0.25f,0.70f,1.65f);// * _localScale;
            Quaternion displayRotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            for (int ii = 0; ii < representations.Count; ii++)
            {
                if(representations[ii] == null)
                {
                    Debug.Log("VRDisplayButton: swappable not assigned a representation, index " + ii);
                }
                // calculate x and y and scatter in circle
                // instantiate VRDisplayButtonChild
                // add representations[ii] sprite to object
                // configure SendMessageAction to send index back upon Trigger
                VRDisplayButtonChild child = (new GameObject("VRDisplayButtonChild" + ii)).AddComponent<VRDisplayButtonChild>();
                child.Renderer.sprite = representations[ii];
                child.ResizeSprite(size * (_localScale));
                child.SetDisplayParent(this);
                child.transform.parent = transform;
                child.transform.localRotation = Quaternion.identity;
                
                //child.Renderer.sprite = representations[ii];
                //child.ResizeSprite(size * (4.5f / _localScale));
                child.Action.parameter = "" + ii;
                child.Action.messageReceiver = gameObject;
                child.Action.messageName = "SwitchChild";
                child.transform.position = transform.position + transform.TransformDirection((Vector3.up * Mathf.Sin(Mathf.Deg2Rad * angle * ii) * (size * position) + Vector3.right * Mathf.Cos(Mathf.Deg2Rad * angle * ii) * (size* position)) - Vector3.forward * 0.01f);
                buttonRepresentations.Add(child);
            }
            hideDisplayButton();
            transform.rotation = displayRotation;
        }

        private void hideDisplayButton()
        {
            // hide main sprite
            getRenderer().enabled = false;
        }

        private void hideChildren()
        {
            // destroy / hide representations
            for(int ii=0; ii<buttonRepresentations.Count; ii++)
            {
                Destroy(buttonRepresentations[ii].gameObject);
            }
            buttonRepresentations.Clear();
            getRenderer().enabled = true;
        }

    }
}