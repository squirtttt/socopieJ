using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VREasy
{
    public abstract class VRSelector : MonoBehaviour
    {
        public const string TOOLTIP_CANVAS_NAME = "[vreasy]TOOLTIP Canvas";

        public float activationTime = 1.5f;
        public bool hasTooltip = true;

        public GameObject canvasObject;

        #region
        public Text Tooltip
        {
            get
            {
                if(tooltip == null)
                {
                    tooltip = GetComponentInChildren<Text>();
                }
                return tooltip;
            }
            set
            {
                tooltip = value;
            }
        }
        public Text tooltip;
        #endregion PROPERTIES

        public Font tooltipFont;
        public int tooltipSize = 30;
        public Color tooltipColour = Color.yellow;
        public float tooltipDistance = 0.15f;

        protected float _selectedTime = 0.0f;
        protected VRSelectable _previouslySelectedObject;
        protected VRGrabbable _previouslyGrabbedObject;

        protected abstract VRSelectable GetSelectable(); // must be overriden by children classes (returns the selectable object when it is selected)
        protected virtual VRGrabbable GetGrabbable()
        {
            return null;
        }

        private void Start()
        {
            InitialiseSelector();
            if(canvasObject != null) canvasObject.SetActive(false);
        }

        void Update()
        {
            CheckSelectable();
            CheckActivate();
            ChildUpdate();
            CheckGrabbable();

        }

        // return real position of the selector (for LOS objects it is not the same as the object's position but where they are looking at)
        public virtual Vector3 GetEndPointPosition()
        {
            return transform.position;
        }

        // return real rotation of the selector (for MR selectorit is not the same as the object's position but the XNode attached to them)
        public virtual Quaternion GetEndPointRotation()
        {
            return transform.rotation;
        }

        // return actual transform of selector object (MRselector it is not the same as the selector transform, but the transform around the XNode attached to them)
        public virtual Transform GetSelectorTransform()
        {
            return transform;
        }

        protected virtual void ChildUpdate() { } // to be overriden by children if they need the Update function

        protected virtual void InitialiseSelector() { } // to be overriden by children if they need the Awake function


        protected void setTooltip(VRSelectable selectable = null)
        {
            if(hasTooltip)
                reconfigureTooltip(selectable == null ? _previouslySelectedObject : selectable ,true);
        }

        protected void clearTooltip(VRSelectable selectable)
        {
            if(hasTooltip)
                reconfigureTooltip(selectable,false);
        }

        public void reconfigureTooltip(VRSelectable selectable,bool show)
        {
            if (tooltip != null)
            {
                if(tooltipFont != null) tooltip.font = tooltipFont;
                tooltip.fontSize = tooltipSize;
                tooltip.color = tooltipColour;
                
                if (!show)
                {
                    // if there is a custom anchored tooltip, use it
                    if (selectable.anchoredTooltipObject != null && selectable.anchoredTooltipText != null)
                    {
                        selectable.anchoredTooltipText.text = "";
                        selectable.anchoredTooltipObject.SetActive(false);
                    } else
                    {
                        tooltip.text = "";
                        canvasObject.SetActive(false);
                    }
                    
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectable.tooltip))
                    {   
                        // if there is a custom anchored tooltip, use it
                        if(selectable.anchoredTooltipObject != null && selectable.anchoredTooltipText != null)
                        {
                            selectable.anchoredTooltipObject.SetActive(true);
                            selectable.anchoredTooltipText.text = selectable.tooltip;
                        } else
                        {
                            canvasObject.SetActive(true);
                            tooltip.text = selectable.tooltip;
                            //reposition
                            Vector3 direction = transform.position - selectable.transform.position;
                            canvasObject.transform.position = selectable.transform.position + direction * tooltipDistance;
                            canvasObject.transform.LookAt(selectable.transform);
                        }
                        
                    }
                }
            }
        }

        private void CheckActivate()
        {
            if (_selectedTime > activationTime)
            {
                _selectedTime = 0.0f;
                ActivateSelectable(_previouslySelectedObject);
                
            }
        }

        private void CheckSelectable() {
            VRSelectable obj = filterVRelement<VRSelectable>(GetSelectable());
            if (_previouslyGrabbedObject != null) obj = null; // do not allow selection if currently grabbing
            if (obj == null || obj != _previouslySelectedObject)
            {
                StoppedInteraction(_previouslySelectedObject);
                _selectedTime = 0.0f;
            }
            if (obj != null && SelectSelectable(obj))
            {
                _selectedTime += Time.deltaTime;
                _previouslySelectedObject = obj;
                // show tooltip
                setTooltip();
            }
            else {
                if (_previouslySelectedObject != null)
                {
                    UnselectSelectable(_previouslySelectedObject);
                    // clear tooltip
                    clearTooltip(_previouslySelectedObject);
                }
                _selectedTime = 0.0f;
                _previouslySelectedObject = null;
            }
            

        }

        private void CheckGrabbable()
        {
            VRGrabbable obj = filterVRelement<VRGrabbable>(GetGrabbable());
            if (obj == null) DeactivateGrabbable(_previouslyGrabbedObject);
            // do something with it
            _previouslyGrabbedObject = obj;
            ActivateGrabbable(obj);
        }

        private T filterVRelement<T>(VRElement element) where T : VRElement
        {
            return element == null ? null : (element.active ? (T)element : null);
        }
        // Interact with selectable object //
        private void ActivateSelectable(VRSelectable obj)
        {
            if(obj != null)
                obj.activate();
        }

        private bool SelectSelectable(VRSelectable obj)
        {
            if (obj != null)
                return obj.select(this);
            else return false;
        }

        private void UnselectSelectable(VRSelectable obj)
        {
            if (obj != null)
                obj.unselect();
        }

        private void StoppedInteraction(VRSelectable obj)
        {
            if (obj != null)
                obj.stopInteraction();
        }

        private void ActivateGrabbable(VRGrabbable obj)
        {
            if (obj != null)
                obj.StartGrab(this);
        }

        private void DeactivateGrabbable(VRGrabbable obj)
        {
            if (obj != null)
                obj.StopGrab(this);
        }
    }
}