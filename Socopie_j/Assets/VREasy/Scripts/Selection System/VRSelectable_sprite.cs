using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VREasy
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class VRSelectable_sprite : VRSelectable
    {
        #region
        public Sprite IdleIcon
        {
            get
            {
                return idleIcon;
            }
            set
            {
                bool changed = idleIcon != value;
                idleIcon = value;
                if (changed)
                {
                    SetState();
                }
            }
        }
        public Sprite SelectIcon
        {
            get
            {
                return selectIcon;
            }
            set
            {
                bool changed = selectIcon != value;
                selectIcon = value;
                if (changed)
                {
                    SetState();
                }
            }
        }
        public Sprite ActivateIcon
        {
            get
            {
                return activateIcon;
            }
            set
            {
                bool changed = activateIcon != value;
                activateIcon = value;
                if (changed)
                {
                    SetState();
                }
            }
        }
        #endregion PROPERTIES

        public Sprite idleIcon;
        public Sprite selectIcon;
        public Sprite activateIcon;

        public float width = 0.5f;
        public float height = 0.5f;

        protected SpriteRenderer _renderer;

        protected SpriteRenderer getRenderer()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }
            return _renderer;
        }

        void LateUpdate()
        {
            SetState();
            //resizeSprite();
        }

        protected virtual void SetState()
        {
            // switch between states
            if (isSelected)
            {
                // SELECTED
                //_renderer.sprite = activateIcon;
                setSprite(activateIcon);
            }
            else if (isPressed)
            {
                // PRESSED BUT NOT SELECTED
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

        protected void setSprite(Sprite sp)
        {
            if (getRenderer() != null && sp != getRenderer().sprite)
            {
                VREasy_utils.SetAndConfigureSprite(sp, getRenderer(), width, height);
            }
        }

        protected void refreshSprite()
        {
            if(getRenderer() != null)
                VREasy_utils.SetAndConfigureSprite(getRenderer().sprite, getRenderer(), width, height);
        }

    }
}