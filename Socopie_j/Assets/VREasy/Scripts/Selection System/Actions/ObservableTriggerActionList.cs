using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace VREasy
{
    public class ObservableTriggerActionList : MonoBehaviour
    {
        public Vector2 targetValueRange = Vector2.zero;
        public bool fireOnce = true;
        //public EVENT_ACTION_COMPARATOR comparator;
        public int skipFrames = 10;

        public Type observedType;

        public Component targetComponent = null;
        public string targetProperty = "";
        public string targetField = "";

        private bool hasFired = false;
        private bool observe = true;

        public ActionList actionList
        {
            get
            {
                if(_actionList == null)
                {
                    _actionList = GetComponent<ActionList>();
                }
                if (_actionList == null)
                {
                    _actionList = gameObject.AddComponent<ActionList>();
                }
                return _actionList;
            }

        }
        private ActionList _actionList;


        private void Awake()
        {
            hasFired = false;
            if (targetComponent != null && !string.IsNullOrEmpty(targetProperty))
            {
                observedType = targetComponent.GetType().GetProperty(targetProperty).PropertyType;
            }
            else
            {
                Debug.LogWarning("[VREasy]: ObservableTriggerActionList target property or component not set. Not observing");
                observe = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!observe ||Time.frameCount % skipFrames != 0) return;
            if (targetComponent != null && !string.IsNullOrEmpty(targetProperty))
            {
                double value = getObservedValue();
                if (value < targetValueRange.y && value > targetValueRange.x) trigger();
                else recover();
                /*switch (comparator)
                {
                    case EVENT_ACTION_COMPARATOR.LESS_THAN:
                        {
                            if (value < targetValueRange.y && value < targetValueRange.x) trigger();
                        }
                        break;
                    case EVENT_ACTION_COMPARATOR.MORE_THAN:
                        {
                            if (value > targetValueRange.x && value > targetValueRange.y) trigger();
                        }
                        break;

                }*/
            }
        }

        private void trigger()
        {
            if(fireOnce)
            {
                if (!hasFired)
                {
                    actionList.Trigger();
                }
            } else
            {
                actionList.Trigger();
            }
            hasFired = true;
        }

        private void recover()
        {
            hasFired = false;
        }

        private double getObservedValue()
        {
            if (observedType == typeof(float) || observedType == typeof(int) || observedType == typeof(long) || observedType == typeof(double) || observedType == typeof(short))
            {
                return getObservedFieldValue<double>();
            }

            if (observedType == typeof(Vector2))
            {
                Vector2 obj = getObservedFieldValue<Vector2>();
                if (targetField == "x")
                {
                    return obj.x;
                }
                if (targetField == "y")
                {
                    return obj.y;
                }
            }
            if (observedType == typeof(Vector3))
            {
                Vector3 obj = getObservedFieldValue<Vector3>();
                if (targetField == "x")
                {
                    return obj.x;
                }
                if (targetField == "y")
                {
                    return obj.y;
                }
                if (targetField == "z")
                {
                    return obj.z;
                }
            }
            if (observedType == typeof(Vector4))
            {
                Vector4 obj = getObservedFieldValue<Vector4>();
                if (targetField == "x")
                {
                    return obj.x;
                }
                if (targetField == "y")
                {
                    return obj.y;
                }
                if (targetField == "z")
                {
                    return obj.z;
                }
                if (targetField == "w")
                {
                    return obj.w;
                }
            }
            if (observedType == typeof(Color))
            {
                Color obj = getObservedFieldValue<Color>();
                if (targetField == "r")
                {
                    return obj.r;
                }
                if (targetField == "g")
                {
                    return obj.g;
                }
                if (targetField == "b")
                {
                    return obj.b;
                }
                if (targetField == "a")
                {
                    return obj.a;
                }
            }
            return 0;
        }

        private T getObservedFieldValue<T>()
        {
            T value = (T)targetComponent.GetType().GetProperty(targetProperty).GetValue(targetComponent, null);
            return value;
        }


    }
}