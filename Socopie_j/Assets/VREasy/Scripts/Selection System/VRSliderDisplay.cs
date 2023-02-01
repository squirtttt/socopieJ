using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class VRSliderDisplay : VRElement
    {
#if BATTLE_HUD_SDK
        public Texture2D backgroundTex = null;
        public Texture2D handleTex = null;
#endif
        public VRSlider Slider
        {
            get
            {
                if(_slider == null)
                {
                    VRSlider sl = HandleRenderer.gameObject.GetComponent<VRSlider>();
                    if (sl == null)
                    {
                        sl = HandleRenderer.gameObject.AddComponent<VRSlider>();
                    }
                    _slider = sl;
                }
                return _slider;
            }
            set
            {
                _slider = value;
            }
        }

        public Sprite Background
        {
            get
            {
                return _background;
            }
            set {
                _background = value;
                Refresh();
            }
        }

        public Sprite Handle
        {
            get
            {
                return _handle;
            }
            set
            {
                _handle = value;
                Refresh();
            }
        }

        public float BackgroundScaleX
        {
            get
            {
                return BackgroundRenderer.transform.localScale.x;
            }
            set
            {
                Vector3 scale = BackgroundRenderer.transform.localScale;
                scale.x = value;
                BackgroundRenderer.transform.localScale = scale;
                Slider.RecalculateBoundaries();
            }
        }

        public float BackgroundScaleY
        {
            get
            {
                return BackgroundRenderer.transform.localScale.y;
            }
            set
            {
                Vector3 scale = BackgroundRenderer.transform.localScale;
                scale.y = value;
                BackgroundRenderer.transform.localScale = scale;
            }
        }

        public float HandleScaleX
        {
            get
            {
                return HandleRenderer.transform.localScale.x;
            }
            set
            {
                Vector3 scale = HandleRenderer.transform.localScale;
                scale.x = value;
                HandleRenderer.transform.localScale = scale;
            }
        }

        public float HandleScaleY
        {
            get
            {
                return HandleRenderer.transform.localScale.y;
            }
            set
            {
                Vector3 scale = HandleRenderer.transform.localScale;
                scale.y = value;
                HandleRenderer.transform.localScale = scale;
            }
        }

        public VRSlider _slider;
        public Sprite _background;
        public Sprite _handle;

        public SpriteRenderer BackgroundRenderer
        {
            get
            {
                if(backgroundRenderer == null)
                {
                    Transform b = transform.Find("Background");
                    if(b == null)
                    {
                        GameObject g = new GameObject("Background");
                        b = g.transform;
                        b.parent = transform;
                        b.localScale = new Vector3(1f, 1f, 1f);
                        b.localPosition = Vector2.zero;
                    }
                    SpriteRenderer sp = b.gameObject.GetComponent<SpriteRenderer>();
                    if(sp == null)
                    {
                        sp = b.gameObject.AddComponent<SpriteRenderer>();
                    }
                    backgroundRenderer = sp;
                }
                return backgroundRenderer;
            }
        }
        public SpriteRenderer HandleRenderer
        {
            get
            {
                if (handleRenderer == null)
                {
                    Transform b = transform.Find("Handle");
                    if (b == null)
                    {
                        GameObject g = new GameObject("Handle");
                        b = g.transform;
                        b.parent = transform;
                        b.localScale = new Vector3(1f, 1f, 1f);
                        b.localPosition = Vector2.zero;
                    }
                    SpriteRenderer sp = b.gameObject.GetComponent<SpriteRenderer>();
                    if (sp == null)
                    {
                        sp = b.gameObject.AddComponent<SpriteRenderer>();
                    }
                    handleRenderer = sp;
                }
                return handleRenderer;
            }
        }

        public SpriteRenderer backgroundRenderer;
        public SpriteRenderer handleRenderer;

        private void Refresh()
        {
            BackgroundRenderer.sprite = _background;
            HandleRenderer.sprite = _handle;
        }
    }

    
}