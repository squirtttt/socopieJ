using UnityEngine;
using System.Collections;

// Class designed to expose shader properties to Unity
// Useful when linked to a VRSlider to control individual shader properties

namespace VREasy
{
    public class VRMaterialPropertyExposer : MonoBehaviour
    {
        public string customFloatShaderProperty = "";
        public string customColourShaderProperty = "";

        public Material Material
        {
            get
            {
                if(mat == null)
                {
                    Renderer rend = GetComponent<Renderer>();
                    if(rend != null)
                    {
                        mat = rend.sharedMaterial;
                    }
                }
                return mat;
            }
            set
            {
                mat = value;
            }
        }
        public Material mat;

        // Custom colour property
        public Color CustomColour
        {
            get
            {
                if (Material != null && !Material.HasProperty(customColourShaderProperty))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no " + customColourShaderProperty + " property");
                }
                else
                    return getColorProperty(customColourShaderProperty);
            }
            set
            {
                if (Material != null && !Material.HasProperty(customColourShaderProperty))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no " + customColourShaderProperty + " property");
                }
                else
                    setProperty(customColourShaderProperty, value);
            }
        }

        // Custom flaot property
        public float CustomFloat
        {
            get
            {
                if (Material != null && !Material.HasProperty(customFloatShaderProperty))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no " + customFloatShaderProperty + " property");
                }
                else
                    return getFloatProperty(customFloatShaderProperty);
            }
            set
            {
                if (Material != null && !Material.HasProperty(customFloatShaderProperty))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no " + customFloatShaderProperty + " property");
                }
                else
                    setProperty(customFloatShaderProperty, value);
            }
        }

        // _Color
        public Color Color
        {
            get
            {
                if(Material != null && !Material.HasProperty("_Color"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Color property");
                } else
                    return getColorProperty("_Color");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_Color"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Color property");
                }
                else
                    setProperty("_Color", value);
            }
        }

        // _Cutoff
        public float Cutoff
        {
            get
            {
                if (Material != null && !Material.HasProperty("_Cutoff"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Cutoff property");
                }
                else
                    return getFloatProperty("_Cutoff");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_Cutoff"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Cutoff property");
                }
                else
                    setProperty("_Cutoff", value);
            }
        }

        // _Glossiness
        public float Glossiness
        {
            get
            {
                if (Material != null && !Material.HasProperty("_Glossiness"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Glossiness property");
                }
                else
                    return getFloatProperty("_Glossiness");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_Glossiness"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Glossiness property");
                }
                else
                    setProperty("_Glossiness", value);
            }
        }

        // _Metallic
        public float Metallic
        {
            get
            {
                if (Material != null && !Material.HasProperty("_Metallic"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Metallic property");
                }
                else
                    return getFloatProperty("_Metallic");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_Metallic"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Metallic property");
                }
                else
                    setProperty("_Metallic", value);
            }
        }

        // _BumpScale
        public float BumpScale
        {
            get
            {
                if (Material != null && !Material.HasProperty("_BumpScale"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _BumpScale property");
                }
                else
                    return getFloatProperty("_BumpScale");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_BumpScale"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _BumpScale property");
                }
                else
                    setProperty("_BumpScale", value);
            }
        }

        // _Parallax
        public float Parallax
        {
            get
            {
                if (Material != null && !Material.HasProperty("_Parallax"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Parallax property");
                }
                else
                    return getFloatProperty("_Parallax");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_Parallax"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _Parallax property");
                }
                else
                    setProperty("_Parallax", value);
            }
        }

        // _OcclusionStrength
        public float OcclusionStrength
        {
            get
            {
                if (Material != null && !Material.HasProperty("_OcclusionStrength"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _OcclusionStrength property");
                }
                else
                    return getFloatProperty("_OcclusionStrength");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_OcclusionStrength"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _OcclusionStrength property");
                }
                else
                    setProperty("_OcclusionStrength",value);
            }
        }

        // _EmissionColor
        public Color EmissionColor
        {
            get
            {
                if (Material != null && !Material.HasProperty("_EmissionColor"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _EmissionColor property");
                }
                else
                    return getColorProperty("_EmissionColor");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_EmissionColor"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _EmissionColor property");
                }
                else
                    setProperty("_EmissionColor", value);
            }
        }

        // _DetailNormalMapScale
        public float DetailNormalMapScale
        {
            get
            {
                if (Material != null && !Material.HasProperty("_DetailNormalMapScale"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _DetailNormalMapScale property");
                }
                else
                    return getFloatProperty("_DetailNormalMapScale");
            }
            set
            {
                if (Material != null && !Material.HasProperty("_DetailNormalMapScale"))
                {
                    throw new InvalidMaterialProperty("[VREasy]: Material has no _DetailNormalMapScale property");
                }
                else
                    setProperty("_DetailNormalMapScale", value);
            }
        }

        /// //////////////////////////////////

        // get and set property from material
        private float getFloatProperty(string property)
        {
            if (Material == null)
            {
                Debug.LogError("[VREasy] VRMaterialPropertyExposer: material not set");
            } else if (Material.HasProperty(property))
            {
                return Material.GetFloat(property);
            }
            
            return -1f;
        }

        private void setProperty(string property, float value)
        {
            if (Material == null)
            {
                Debug.LogError("[VREasy] VRMaterialPropertyExposer: material not set");
            }
            else
            {
                if (Material.HasProperty(property))
                {
                    Material.SetFloat(property, value);
                }
                else
                {
                    Debug.LogWarning("[VREasy] VRMaterialPropertyExposer: material has no" + property + " property");
                }
            }
        }

        private Color getColorProperty(string property)
        {
            if (Material == null)
            {
                Debug.LogError("[VREasy] VRMaterialPropertyExposer: material not set");
            } else if (Material.HasProperty(property))
            {
                return Material.GetColor(property);
            }
            return Color.black;
        }

        private void setProperty(string property, Color value)
        {
            if (Material == null)
            {
                Debug.LogError("[VREasy] VRMaterialPropertyExposer: material not set");
            }
            else
            {
                if (Material.HasProperty(property))
                {
                    Material.SetColor(property, value);
                }
                else
                {
                    Debug.LogWarning("[VREasy] VRMaterialPropertyExposer: material has no" + property + " property");
                }
            }
        }
    }
}