using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class SightSelector : LOSSelector
    {
#if BATTLE_HUD_SDK
        public Texture2D crosshairTex;
        public Texture2D idleTex;
#endif

        public static SightSelector CreateSightSelector(ref GameObject _ref)
        {
            SightSelector sel = _ref.AddComponent<SightSelector>();
            return sel;
        }

        private const string CROSSHAIR_NAME = "[vreasy]crosshair";

        public bool useCrosshair = true;
        
        public Sprite crosshairSprite;
        public float crosshairSize = 0.1f;
        public Color crosshairActiveColour = Color.red;
        public Color crosshairIdleColour = Color.white;
        public CROSSHAIR_TYPE crosshairType = CROSSHAIR_TYPE.SINGLE_SPRITE;
        public Sprite idleSprite;
        public float resizeMultiplier = 50;
        public float resizeSpeed = 5f;
        public bool dynamicSize = false;
        // animated sprite
        public string idleAnimatorState = "";
        public string activeAnimatorState = "";
        public string selectedAnimatorState = "";


        private float targetCrosshairSize;
        private float originalCrosshairSize;

        #region
        public Sprite CrosshairSprite
        {
            get
            {
                return crosshairSprite;
            }
            set
            {
                bool reconfigure = crosshairSprite != value;
                crosshairSprite = value;
                if (reconfigure) reconfigureCrosshair();
            }
        }

        public float CrosshairSize
        {
            get
            {
                return crosshairSize;
            }
            set
            {
                bool reconfigure = crosshairSize != value;
                crosshairSize = value;
                if (reconfigure) reconfigureCrosshair();
            }
        }
        public Color CrosshairActiveColour
        {
            get
            {
                return crosshairActiveColour;
            }
            set
            {
                bool reconfigure = crosshairActiveColour != value;
                crosshairActiveColour = value;
                if (reconfigure) SetCrosshairState(false);
            }
        }
        public Color CrosshairIdleColour
        {
            get
            {
                return crosshairIdleColour;
            }
            set
            {
                bool reconfigure = crosshairIdleColour != value;
                crosshairIdleColour = value;
                if (reconfigure) SetCrosshairState(false);
            }
        }
        public SpriteRenderer Crosshair
        {
            get
            {
                if(crosshair == null)
                {
                    Transform t = transform.Find(CROSSHAIR_NAME);
                    if (t)
                        crosshair = t.GetComponent<SpriteRenderer>();
                    else
                    {
                        GameObject ob = new GameObject(CROSSHAIR_NAME);
                        ob.transform.parent = transform;
                        SpriteRenderer rend = ob.AddComponent<SpriteRenderer>();
                        rend.sharedMaterial = Resources.Load<Material>("Crosshair");
                        Crosshair = rend;
                    }
                }
                return crosshair;
            }
            set
            {
                crosshair = value;
            }
        }
        public SpriteRenderer crosshair;

        public Animator Animator
        {
            get
            {
                if(animator == null)
                {
                    animator = Crosshair.gameObject.GetComponent<Animator>();
                    if (animator == null)
                        animator = Crosshair.gameObject.AddComponent<Animator>();
                }
                return animator;
            }
            set
            {
                animator = value;
            }
        }
        public Animator animator;


        #endregion PROPERTIES

        protected override VRSelectable GetSelectable() {
            /*VRSelectable obj = null;
            if(debugMode) Debug.DrawLine(transform.position, transform.position + transform.forward * selectionDistance, Color.red);
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, transform.forward, out _hit, selectionDistance)) {
                obj = _hit.collider.gameObject.GetComponent<VRSelectable>();
                if(obj != null && !obj.CanSelectWithSight())
                {
                    obj = null;
                }
            }*/
            VRSelectable obj = GetElement<VRSelectable>();
            if (obj != null && !obj.CanSelectWithSight())
            {
                obj = null;
            }

            SetCrosshairState(obj != null);
            
            return obj;
        }

        protected override void InitialiseSelector()
        {
            originalCrosshairSize = CrosshairSize;
        }
        /*private void Awake()
        {
            originalCrosshairSize = CrosshairSize;
        }*/

        public void reconfigureCrosshair()
        {
            if (!useCrosshair)
            {
                if(crosshairType != CROSSHAIR_TYPE.ANIMATED_SPRITE) removeCrosshair();
                return;
            }

            if(crosshairSprite != null)
            {
                Crosshair.transform.rotation = transform.rotation;
                if(Camera.main == null)
                {
                    Camera cam = GetComponent<Camera>();
                    if (cam == null)
                    {
                        Debug.LogWarning("SightSelector: Main camera has not been found. Impossible to dynamically situate crosshair. Setting it to default distance");
                        Crosshair.transform.localPosition = Vector3.forward * 0.31f;
                    } else
                    {
                        Crosshair.transform.localPosition = Vector3.forward * cam.nearClipPlane * 6.01f; // 1.01
                    }
                        
                } else
                {
                    Crosshair.transform.localPosition = Vector3.forward * Camera.main.nearClipPlane * 6.01f;
                }
                Crosshair.transform.localScale = Vector3.one * crosshairSize;
                Crosshair.sprite = crosshairSprite;
            } else
            {
                if (crosshairType != CROSSHAIR_TYPE.ANIMATED_SPRITE) removeCrosshair();
            }
        }

        protected override void ChildUpdate() {
            if (dynamicSize)
            {
                // set crosshair size
                float scale = Mathf.Lerp(CrosshairSize, targetCrosshairSize, resizeSpeed * Time.deltaTime);
                CrosshairSize = scale;
            }
            
        } 

        protected void SetCrosshairState(bool isActive)
        {
            if (crosshair == null) return;

            switch(crosshairType)
            {
                case CROSSHAIR_TYPE.SINGLE_SPRITE:
                    {
                        if (isActive)
                        {
                            crosshair.sharedMaterial.SetColor("_Color",crosshairActiveColour);
                        }
                        else
                        {
                            crosshair.sharedMaterial.SetColor("_Color", crosshairIdleColour);
                        }
                    }
                    break;
                case CROSSHAIR_TYPE.DUAL_SPRITE:
                    {
                        if (isActive)
                        {
                            crosshair.sprite = CrosshairSprite;
                        }
                        else
                        {
                            crosshair.sprite = idleSprite;
                        }
                    }
                    break;
                case CROSSHAIR_TYPE.ANIMATED_SPRITE:
                    {
                        
                        if (isActive)
                        {
                            Animator.Play(activeAnimatorState);
                        } else
                        {
                            Animator.Play(idleAnimatorState);
                        }
                    }
                    break;
            }

            targetCrosshairSize = isActive ? originalCrosshairSize + originalCrosshairSize * resizeMultiplier / 100.0f : originalCrosshairSize;

        }

        public void removeCrosshair()
        {
            if(Crosshair)
            {
                DestroyImmediate(Crosshair.gameObject);
            }
        }

        

    }
}
