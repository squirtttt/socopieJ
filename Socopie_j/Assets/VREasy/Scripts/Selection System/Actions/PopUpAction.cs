using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if BATTLE_HUD_SDK
using Battlehub.RTEditor;
using Battlehub.RTCommon;
#endif

namespace VREasy
{
    public class PopUpAction : VRAction
    {
#if BATTLE_HUD_SDK
        public Texture2D imageTex = null;
        public Vector2 scale = Vector2.one;
#endif
        public Object Image
        {
            get
            {
                switch (type)
                {
                    case DISPLAY_IMAGE_TYPE.SPRITE:
                        {
                            SpriteRenderer rend = Template.GetComponent<SpriteRenderer>();
                            if(rend == null)
                            {
                                Debug.Log("DisplayImage: error, ImagePanel child must contain a SpriteRenderer");
                                return null;
                            } else
                            {
                                return rend.sprite;
                            }
                            
                        }
                    case DISPLAY_IMAGE_TYPE.TEXTURE:
                        {
                            MeshRenderer rend = Template.GetComponent<MeshRenderer>();
                            if (rend == null)
                            {
                                Debug.Log("DisplayImage: error, ImagePanel child must contain a MeshRenderer");
                                return null;
                            }
                            else
                            {
                                return rend.sharedMaterial.mainTexture;
                            }

                        }
                    default:
                        return null;
                }
            }
            set
            {
                switch(type)
                {
                    case DISPLAY_IMAGE_TYPE.SPRITE:
                        {
                            //template.GetComponent<SpriteRenderer>().sprite = value == null ? null : (Sprite)value;
                            Sprite sp = value == null ? null : (Sprite)value;
                            //if(Template != null)
                                VREasy_utils.SetAndConfigureSprite(sp, Template.GetComponent<SpriteRenderer>(), 2.5f, 2.5f);
                        }
                        break;
                    case DISPLAY_IMAGE_TYPE.TEXTURE:
                        {
                            //if (Template != null)
                                Template.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = value == null ? null : (Texture2D)value;
                        }
                        break;
                }
                
            }
        }
        public Vector2 TemplateScale
        {
            get
            {
                return _templateScale;
            }
            set
            {
                _templateScale = value;
                setScale();
            }
        }

        public Vector2 _templateScale = Vector2.one;

        public GameObject Template
        {
            get
            {
                if (_template == null)
                {
                    generatePanel();
                    return Template;
                }
                else
                {
                    return _template;
                }
            }
            set
            {
                _template = value;
            }
        }
        public GameObject _template;

        public float hideTime = 5f;
        public DISPLAY_IMAGE_TYPE type = DISPLAY_IMAGE_TYPE.SPRITE;
        

        void Start()
        {
            Hide();

        }

        public override void Trigger()
        {
            if(IsInvoking("Hide"))
            {
                CancelInvoke("Hide");
                Hide();
            } else
            {
                Activate();
                Invoke("Hide", hideTime);
            }
                
            
        }

        public void Hide()
        {
#if BATTLE_HUD_SDK
            IRuntimeEditor rte = IOC.Resolve<IRuntimeEditor>();
            if(!rte.IsPlaying)
            {
                return;
            }
#endif
            //template.SetActive(false);
            Renderer[] rends = Template.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in rends)
            {
                r.enabled = false;
            }
        }

        public void Activate()
        {
            Renderer[] rends = Template.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
            {
                r.enabled = true;
            }
        }

        private void generatePanel()
        {
            // generate the template
            switch (type)
            {
                case DISPLAY_IMAGE_TYPE.SPRITE:
                    {
                        /*image.template = new GameObject("ImagePanel");
                        image.template.transform.parent = image.transform;
                        image.template.transform.localPosition = Vector3.right * 7f;
                        image.template.transform.localRotation = Quaternion.identity;
                        image.template.transform.localScale = Vector3.one;
                        image.template.AddComponent<SpriteRenderer>();*/
                        Template = new GameObject("ImagePanel");
                        Template.transform.parent = transform;
                        //template.transform.localScale = Vector3.one;
                        TemplateScale = Vector2.one;
                        Template.transform.localRotation = Quaternion.identity;
                        Template.transform.position = transform.position + transform.right;
                        Template.AddComponent<SpriteRenderer>();
                    }
                    break;
                case DISPLAY_IMAGE_TYPE.TEXTURE:
                    {
                        Template = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        DestroyImmediate(Template.GetComponent<Collider>());
                        Template.name = ("ImagePanel");
                        Template.transform.parent = transform;
                        //template.transform.localScale = Vector3.one * 0.2f;
                        TemplateScale = Vector2.one;
                        Template.transform.localRotation = Quaternion.Euler(90, 180, 0);
                        Template.transform.position = transform.position + transform.right;
                        Template.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
                    }
                    break;
            }
        }

        private void setScale()
        {
            switch(type)
            {
                case DISPLAY_IMAGE_TYPE.SPRITE:
                    Template.transform.localScale = new Vector3(TemplateScale.x,TemplateScale.y,1f);
                    break;
                case DISPLAY_IMAGE_TYPE.TEXTURE:
                    Template.transform.localScale = new Vector3(TemplateScale.x, 1f, TemplateScale.y) * 0.2f;
                    break;
            }
        }
    }
}