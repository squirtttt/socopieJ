using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VREasy
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Slider))]
    public class VRGUISlider : VRSelectable
    {
        public Slider UnitySlider
        {
            get
            {
                if (unitySlider == null)
                    unitySlider = GetComponent<Slider>();
                return unitySlider;
            }
            set
            {
                unitySlider = value;
            }
        }
        private Slider unitySlider;

        private EventSystem eventSystem;

        public override bool CanSelectWithSight()
        {
            return false;
        }

        public override bool CanBeActivated()
        {
            return false;
        }

        protected override void Initialise()
        {
            base.Initialise();
            eventSystem = FindObjectOfType<EventSystem>();
        }
        
        protected override void Pressed(VRSelector selector)
        {
            PointerEventData data = getPointerEventData(selector);
            if (data != null)
            {
                eventSystem.SetSelectedGameObject(null);
                //GameObject newPressed = ExecuteEvents.ExecuteHierarchy(gameObject, getPointerEventData(), ExecuteEvents.pointerEnterHandler);
                GameObject newPressed = ExecuteEvents.ExecuteHierarchy(gameObject, data, ExecuteEvents.dragHandler);
                if (newPressed != null)
                {
                    eventSystem.SetSelectedGameObject(newPressed);
                }
            }
        }
        
        protected override void Unpressed()
        {
            PointerEventData data = getPointerEventData(null);
            if (data != null)
            {
                eventSystem.SetSelectedGameObject(null);
                ExecuteEvents.ExecuteHierarchy(gameObject, data, ExecuteEvents.endDragHandler);

            }
        }

        private PointerEventData getPointerEventData(VRSelector selector)
        {
            if (eventSystem == null)
            {
                Debug.LogError("[VREasy] VRGUISlider: You must have at least one EventSystem in the scene to deal with Unity GUI");
                return null;
            }
            PointerEventData data = new PointerEventData(eventSystem);
            data.Reset();
            data.delta = Vector2.zero;
            if (selector != null)
            {
                data.position = selector.GetEndPointPosition();
                data.scrollDelta = Vector2.zero;
                data.scrollDelta = Vector2.zero;
            }
            
            return data;
        }
    }
}