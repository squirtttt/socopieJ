using UnityEngine;
using System.Collections;
using System.Reflection;
#if VREASY_VREE_PLATFORM_SDK
using VREasy.Networking;
#endif
using System;

namespace VREasy
{
    [RequireComponent(typeof(BoxCollider))]
#if VREASY_VREE_PLATFORM_SDK
    public class VRSlider : VRGrabbable, IVRSliderNetworkEvents
#else
    public class VRSlider : VRGrabbable
#endif
    {
        public float value = 0.5f;
        public float min = 0.0f;
        public float max = 1.0f;

        public Component targetComponent;
        public string targetProperty;
        public string targetField;

        public event Action<Vector3> OnValueUpdated;

        //public float MIN = -0.3f;
        //public float MAX = 0.3f;

        void Start()
        {
            xAxis = true;
            yAxis = false;
            zAxis = false;
            snapToPosition = false;
            type = GRAB_TYPE.SLIDE;
            constraintMovement = true;
            RecalculateBoundaries();
        }

        /*protected override Vector3 RestrictMovement(Vector3 move)
        {
            Vector3 dest = transform.TransformDirection(move) + transform.localPosition;
            if (dest.x > maxMovement.x) move.x = 0.0f;
            if (dest.x < minMovement.x) move.x = 0.0f;
            return move;
        }*/

        public void ApplyMovement(float x, float y, float z)
        {
            ApplyMovement(new Vector3(x, y, z));
        }

        protected override void ApplyMovement(Vector3 move)
        {
            calculateValue();
            broadcast();
            base.ApplyMovement(move);
            Debug.Log("VRSlider::ApplyMovement");

            Debug.Log("VRSlider::OnValueUpdated = "+OnValueUpdated);

            if(OnValueUpdated != null) OnValueUpdated.Invoke(move);
        }

        public void RecalculateBoundaries()
        {
            //MIN = -(transform.parent.Find("Background").localScale.x * 2f + transform.parent.Find("Handle").localScale.x/2f);
            //MAX = Mathf.Abs(MIN);
            minMovement.x = -(transform.parent.Find("Background").localScale.x * 2f + transform.parent.Find("Handle").localScale.x / 2f);
            maxMovement.x = Mathf.Abs(minMovement.x);
        }

        

        public void SetValue(float val)
        {
            value = Mathf.Clamp01(val);
            transform.localPosition = new Vector3(value * (maxMovement.x - minMovement.x) - Mathf.Abs(minMovement.x), transform.localPosition.y, transform.localPosition.z);
            broadcast();
        }

        public float GetRealValue()
        {
            return value * (max - min) + (min);
        }

        private void calculateValue()
        {
            value = (transform.localPosition.x + Mathf.Abs(minMovement.x)) / (maxMovement.x - minMovement.x);
        }

        private void broadcast()
        {
            if(targetComponent != null && !string.IsNullOrEmpty(targetProperty))
            {
                try
                {
                    if(string.IsNullOrEmpty(targetField))
                    {
                        // when the target property is a number
                        targetComponent.GetType().GetProperty(targetProperty).SetValue(targetComponent, GetRealValue(), null);
                    } else
                    {
                        // When the target property is a Vector
                        if (targetComponent.GetType().GetProperty(targetProperty).PropertyType == typeof(Vector2))
                        {
                            Vector2 obj = (Vector2)targetComponent.GetType().GetProperty(targetProperty).GetValue(targetComponent, null);
                            if (targetField == "x")
                            {
                                obj.x = GetRealValue();
                            }
                            if (targetField == "y")
                            {
                                obj.y = GetRealValue();
                            }
                            targetComponent.GetType().GetProperty(targetProperty).SetValue(targetComponent, obj, null);
                        }
                        if (targetComponent.GetType().GetProperty(targetProperty).PropertyType == typeof(Vector3))
                        {
                            Vector3 obj = (Vector3)targetComponent.GetType().GetProperty(targetProperty).GetValue(targetComponent, null);
                            if (targetField == "x")
                            {
                                obj.x = GetRealValue();
                            }
                            if (targetField == "y")
                            {
                                obj.y = GetRealValue();
                            }
                            if (targetField == "z")
                            {
                                obj.z = GetRealValue();
                            }
                            targetComponent.GetType().GetProperty(targetProperty).SetValue(targetComponent, obj,null);
                        }
                        if (targetComponent.GetType().GetProperty(targetProperty).PropertyType == typeof(Vector4))
                        {
                            Vector4 obj = (Vector4)targetComponent.GetType().GetProperty(targetProperty).GetValue(targetComponent, null);
                            if (targetField == "x")
                            {
                                obj.x = GetRealValue();
                            }
                            if (targetField == "y")
                            {
                                obj.y = GetRealValue();
                            }
                            if (targetField == "z")
                            {
                                obj.z = GetRealValue();
                            }
                            if (targetField == "w")
                            {
                                obj.w = GetRealValue();
                            }
                            targetComponent.GetType().GetProperty(targetProperty).SetValue(targetComponent, obj, null);
                        }
                        if (targetComponent.GetType().GetProperty(targetProperty).PropertyType == typeof(Color))
                        {
                            Color obj = (Color)targetComponent.GetType().GetProperty(targetProperty).GetValue(targetComponent, null);
                            if (targetField == "r")
                            {
                                obj.r = GetRealValue();
                            }
                            if (targetField == "g")
                            {
                                obj.g = GetRealValue();
                            }
                            if (targetField == "b")
                            {
                                obj.b = GetRealValue();
                            }
                            if (targetField == "a")
                            {
                                obj.a = GetRealValue();
                            }
                            targetComponent.GetType().GetProperty(targetProperty).SetValue(targetComponent, obj, null);
                        }
                    }
                    
                }
                catch (System.Exception e)
                {
                    Debug.LogError("VRSlider error whilst accessing property via reflection. " + e.ToString());
                }
            }
        }

    }
}