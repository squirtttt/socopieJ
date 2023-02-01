using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    public class VRSelectable_colour : VRSelectable
    {
        #region
        /*public Color IdleColour
        {
            get
            {
                return idleColour;
            }
            set
            {
                bool changed = idleColour != value;
                idleColour = value;
                if (changed) SetState();
            }
        }*/
        public Color SelectColour
        {
            get
            {
                return selectColour;
            }
            set
            {
                bool changed = selectColour != value;
                selectColour = value;
                if (changed) SetState();
            }
        }
        public Color ActivateColour
        {
            get
            {
                return activateColour;
            }
            set
            {
                bool changed = activateColour != value;
                activateColour = value;
                if (changed) SetState();
            }
        }

        #endregion PROPERTIES

        //public Color idleColour = Color.white;
        public Color selectColour = Color.red;
        public Color activateColour = Color.blue;

        public bool useColourHighlights = true;

        private List<Color> idleColours = new List<Color>();

        protected override void Initialise()
        {
            base.Initialise();
            // automatically pick idle colour
            Renderer[] rends = GetComponentsInChildren<Renderer>(true);
            foreach(Renderer rend in rends)
            {
                if (rend.gameObject != gameObject && (rend.gameObject.GetComponent<VRSelectable>() ||
                    rend.gameObject.GetComponent<SpriteRenderer>() != null)) // skip the renderer if it belongs to another VRelement or is a Sprite renderer
                {
                    continue;
                }
                MeshRenderer mrend = rend as MeshRenderer;
                SpriteRenderer srend = rend as SpriteRenderer;
                if(mrend != null)
                {
                    foreach (Material m in rend.sharedMaterials)
                    {
                        idleColours.Add(m.color);
                    }
                }
                if(srend != null)
                {
                    idleColours.Add(srend.color);
                }
                
            }
        }

        public virtual void SetState()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
            // switch between states
            if (isSelected)
            {
                // SELECTED
                setColour(activateColour);
            }
            else if (isPressed)
            {
                // PRESSED BUT NOT SELECTED
                setColour(selectColour);
            }
            else
            {
                // IDLE
                //setColour(idleColour);
                setColour();
            }
        }

        void Update()
        {
            SetState();
        }

        private void setColour(Color colour)
        {
            if (!useColourHighlights || idleColours.Count == 0) return;
            Renderer[] rends = GetComponentsInChildren<Renderer>(true);
            foreach(Renderer rend in rends)
            {
                if (rend.gameObject != gameObject && (rend.gameObject.GetComponent<VRSelectable>() ||
                    rend.gameObject.GetComponent<SpriteRenderer>() != null)) // skip the renderer if it belongs to another VRelement or is a Sprite renderer
                {
                    continue;
                }
                MeshRenderer mrend = rend as MeshRenderer;
                SpriteRenderer srend = rend as SpriteRenderer;
                if(rend as SkinnedMeshRenderer != null)
                {
                    Debug.LogWarning("[VREasy] VRSelectable_colour: Detected SkinnedMeshRenderer in the object's hierarchy. Cannot change colour of SkinnedMeshRenderer in runtime; colour skipped");
                }
                if(mrend != null)
                {
                    Material[] mats = rend.materials;
                    foreach (Material m in mats)
                    {
                        m.color = colour;
                    }
                    rend.materials = mats;
                }
                if(srend != null)
                {
                    srend.color = colour;
                }
                
            }
        }

        // set idle colour (default)
        private void setColour()
        {
            if (!useColourHighlights || idleColours.Count == 0) return;
            Renderer[] rends = GetComponentsInChildren<Renderer>(true);
            int counter = 0;
            foreach(Renderer rend in rends)
            {
                if (rend.gameObject != gameObject && (rend.gameObject.GetComponent<VRSelectable>() ||
                    rend.gameObject.GetComponent<SpriteRenderer>() != null)) // skip the renderer if it belongs to another VRelement or is a Sprite renderer
                {
                    continue;
                }
                MeshRenderer mrend = rend as MeshRenderer;
                SpriteRenderer srend = rend as SpriteRenderer;
                if (mrend != null)
                {
                    Material[] mats = mrend.materials;
                    foreach (Material m in mats)
                    {
                        m.color = idleColours[counter];
                        counter++;
                    }
                    mrend.materials = mats;
                }
                if (srend != null)
                {
                    srend.color = idleColours[counter];
                    counter++;
                }
            }
        }
    }
}