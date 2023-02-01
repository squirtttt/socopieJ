using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VREasy
{
    [RequireComponent(typeof(BoxCollider))]
    public class VRGUIButton : VRSelectable
    {
        public Selectable UnitySelectable
        {
            get
            {
                if(unitySel == null)
                    unitySel = GetComponent<Selectable>();
                return unitySel;
            }
            set
            {
                unitySel = value;
            }
        }
        private Selectable unitySel;

        private EventSystem eventSystem;

        protected override void Initialise()
        {
            base.Initialise();
            eventSystem = FindObjectOfType<EventSystem>();
        }

        protected override void Selection()
        {
            PointerEventData data = getPointerEventData();
            if(data != null)
            {
                eventSystem.SetSelectedGameObject(null);
                GameObject newPressed = ExecuteEvents.ExecuteHierarchy(gameObject, getPointerEventData(), ExecuteEvents.submitHandler);
                if (newPressed != null)
                {
                    eventSystem.SetSelectedGameObject(newPressed);
                }
            }
            
        }

        protected override void Pressed(VRSelector selector)
        {
            PointerEventData data = getPointerEventData();
            if (data != null)
            {
                eventSystem.SetSelectedGameObject(null);
                GameObject newPressed = ExecuteEvents.ExecuteHierarchy(gameObject, getPointerEventData(), ExecuteEvents.pointerEnterHandler);
                if (newPressed != null)
                {
                    eventSystem.SetSelectedGameObject(newPressed);
                }
            }
        }

        protected override void Unselected()
        {
            PointerEventData data = getPointerEventData();
            if (data != null)
            {
                eventSystem.SetSelectedGameObject(null);
                ExecuteEvents.ExecuteHierarchy(gameObject, getPointerEventData(), ExecuteEvents.pointerExitHandler);
                
            }

        }


        protected override void Unpressed()
        {
            PointerEventData data = getPointerEventData();
            if (data != null)
            {
                eventSystem.SetSelectedGameObject(null);
                ExecuteEvents.ExecuteHierarchy(gameObject, getPointerEventData(), ExecuteEvents.pointerExitHandler);

            }
        }

        private PointerEventData getPointerEventData()
        {
            if(eventSystem == null)
            {
                Debug.LogError("[VREasy] VRGUIButton: You must have an EventSystem to deal with Unity GUI");
                return null;
            }
            PointerEventData data = new PointerEventData(eventSystem);
            data.Reset();
            Vector2 lookPosition;
            lookPosition.x = Screen.width / 2;
            lookPosition.y = Screen.height / 2;
            data.delta = Vector2.zero;
            data.position = lookPosition;
            data.scrollDelta = Vector2.zero;
            data.scrollDelta = Vector2.zero;
            return data;
        }
    }
}