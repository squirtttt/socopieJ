using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{

    public class HighlightObjectAction : VRAction
    {
        private string shader_name = "VREasy/OutlineShader";

        public float flashSpeed = 3f;
        public int numberOfFlashes = 5;
        public Color highlightColour = Color.yellow;
        [Range(0f,2f)]
        public float outlineThickness = 0.1f;
        public bool stayHighlighted = false;
        
        public List<MeshRenderer> targetRenderers = new List<MeshRenderer>();

        private Material[] defaultMaterials;
        private Material[] highlightMaterials;

        private bool isInit = false;
        private bool highlight = false;
        private float targetWidth = 0;
        private int flashCounter = 0;

        private void OnEnable()
        {
            configureShader();
        }

        private void OnDestroy()
        {
            if (stayHighlighted) return;

            restoreMaterials();
            
        }

        private void Update()
        {
            if(highlight)
            {
                for(int ii = 0; ii < targetRenderers.Count; ii++)
                {
                    if (targetRenderers[ii] == null) continue;
                    float w = highlightMaterials[ii].GetFloat("_OutlineWidth");
                    w = Mathf.Lerp(w, targetWidth, Time.deltaTime * flashSpeed);
                    highlightMaterials[ii].SetFloat("_OutlineWidth", w);
                    if (Mathf.Abs(w - targetWidth) < 0.005)
                    {
                        targetWidth = outlineThickness - targetWidth;
                        flashCounter++;
                        if (flashCounter > numberOfFlashes * 2)
                        {
                            doFlashing(false);
                            break;
                        }
                    }
                }
                
            }
        }

        public override void Trigger()
        {
            doFlashing(true);
            
        }

        public void doFlashing(bool state)
        {
            highlight = state;
            for (int ii = 0; ii < targetRenderers.Count; ii++)
            {
                if (targetRenderers[ii] != null && isInit)
                {
                    targetRenderers[ii].material = state || stayHighlighted ? highlightMaterials[ii] : defaultMaterials[ii];
                    flashCounter = 0;
                }
            }
        }

        public void configureShader(bool force = false)
        {
            if (isInit && !force) return;

            targetWidth = outlineThickness;
            defaultMaterials = new Material[targetRenderers.Count];
            for (int ii = 0; ii < targetRenderers.Count; ii++)
            {
                if (targetRenderers[ii] == null)
                {
                    Debug.LogWarning("[HighlightObjectAction]: One or more target renderers are null");
                }
                else
                {
                    defaultMaterials[ii] = targetRenderers[ii].sharedMaterial;
                }

            }

            highlightMaterials = new Material[targetRenderers.Count];
            for(int ii = 0; ii < highlightMaterials.Length; ii++)
            {
                highlightMaterials[ii] = new Material(Shader.Find(shader_name));
                highlightMaterials[ii].SetColor("_OutlineColor", highlightColour);
                highlightMaterials[ii].SetFloat("_OutlineWidth", outlineThickness);
                if (defaultMaterials[ii] != null)
                {
                    if (defaultMaterials[ii].HasProperty("_MainTex"))
                    {
                        highlightMaterials[ii].SetTexture("_MainTex", defaultMaterials[ii].mainTexture);
                    }
                    else
                    {
                        Debug.Log("[HighlightObjectAction on " + gameObject.name + "]: default material does not have a property _MainTex and hence it will not be used when applying highlight shader");
                    }
                    
                    if(defaultMaterials[ii].HasProperty("_Color"))
                    {
                        highlightMaterials[ii].SetColor("_Color", defaultMaterials[ii].color);
                    } else
                    {
                        highlightMaterials[ii].SetColor("_Color", Color.white);
                        Debug.Log("[HighlightObjectAction on " + gameObject.name + "]: default material does not have a property _Color and hence default white will be used for the highlight shader");
                    }
                }
            }
            isInit = true;
            
        }

        public void restoreMaterials() {
            for (int ii = 0; ii < targetRenderers.Count; ii++)
            {
                if (targetRenderers[ii] != null && defaultMaterials[ii] != null)
                {
                    targetRenderers[ii].material = defaultMaterials[ii];
                }
                if (highlightMaterials[ii] != null)
                {
                    DestroyImmediate(highlightMaterials[ii]);
                }
            }
        }

        public void clearAll()
        {
            if(isInit) restoreMaterials();
            highlightMaterials = null;
            defaultMaterials = null;
            isInit = false;
        }
        
    }
}