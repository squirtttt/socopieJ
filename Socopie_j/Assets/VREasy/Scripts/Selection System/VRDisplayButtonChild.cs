using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VREasy {

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(SendMessageAction))]
    public class VRDisplayButtonChild : VRSelectable_colour {

        public SendMessageAction Action
        {
            get
            {
                if (_action == null) _action = GetComponent<SendMessageAction>();
                return _action;
            }
        }

        public SpriteRenderer Renderer
        {
            get
            {
                if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
                return _renderer;
            }
        }

        public BoxCollider Collider
        {
            get
            {
                if (_collider == null) _collider = GetComponent<BoxCollider>();
                return _collider;
            }
        }

        private SendMessageAction _action;
        private SpriteRenderer _renderer;
        private BoxCollider _collider;
        private VRDisplayButton _parentDisplay;

        protected override void Initialise()
        {
            base.Initialise();
            if (actionList.list.Count != 1)
            {
                actionList.list.Clear();
                actionList.list.Add(Action);
            }
        }
        

        protected override void Pressed(VRSelector selector)
        {
            base.Pressed(selector);
            if(_parentDisplay != null)
            {
                _parentDisplay.ResetHideTimer();
            } else
            {
                Debug.Log("VRDisplayButtonChild: VRDisplayButton parent could not be found. Are you missing the script in the parent GameObject?");
            }
        }

        public void SetDisplayParent(VRDisplayButton parent)
        {
            _parentDisplay = parent;
        }

        public void ResizeSprite(float size)
        {
            VREasy_utils.SetAndConfigureSprite(Renderer.sprite, Renderer, size, size);
            
        }
    }
}