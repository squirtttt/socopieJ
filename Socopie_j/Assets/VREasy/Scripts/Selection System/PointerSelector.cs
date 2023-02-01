using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class PointerSelector : LOSSelector
    {

        // creator method
        public static PointerSelector CreatePointerSelector(ref GameObject _ref)
        {
            Rigidbody rb = _ref.GetComponent<Rigidbody>();
            bool hadRb = (rb != null);
            PointerSelector _sel = _ref.AddComponent<PointerSelector>();
            if (!hadRb)
            {
                _sel.ConfigureRigidbody();
            }
            LineRenderer line = _ref.GetComponent<LineRenderer>();
            Material defaultMaterial = Instantiate<Material>(Resources.Load("Pointer", typeof(Material)) as Material);
            if (defaultMaterial == null)
            {
                Debug.LogWarning("Default material for Pointer Selector \"Pointer\" not found in Resources. Please make sure you assign your own material to the Line Renderer of your new Pointer Selector");
            }
            else
            {
                line.sharedMaterial = defaultMaterial;
            }
            _sel.LineWidth = 0.02f;
            line.receiveShadows = false;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return _sel;
        }

        public bool useReticle = true;
        public bool usePassiveBeam = true;
        public Color reticleColour = Color.white;
        public float reticleScale = 0.05f;

        public Color MainColour
        {
            get
            {
                return _mainColour;
            }
            set
            {
                _mainColour = value;
                setCurrentColour(_mainColour);
            }
        }

        public Color _mainColour = Color.green;
        public Color hoverColour = Color.red;
        public Color beamColour = Color.grey;

        public VRGrabTrigger grabTrigger;
        public float LineWidth
        {
            get
            {
                return _lineWidth;
            } 
            set
            {
                _lineWidth = value;

#if UNITY_5_4 || UNITY_5_5
                Line.SetWidth(_lineWidth, _lineWidth);
#else
                Line.startWidth = _lineWidth;
                Line.endWidth = _lineWidth;

#endif
            }
        }
        public float _lineWidth = 0.015f;

        private bool isPointerActive = false;
        //private bool selectableActive = false;
        private float distanceToSelectable = 0f;

        private VRGrabbable _grabbedObject = null;

        public LineRenderer Line
        {
            get
            {
                if(_line == null)
                {
                    _line = GetComponent<LineRenderer>();
                }
                return _line;
            } set
            {
                _line = value;
            }
        }
        public LineRenderer _line = null;

        
        private GameObject reticle;
        private VRSelectable _currentObservedSelectable;

        private VRElement _currentObservedElement;
        private VRElement _previousObservedElement;

        //private void Awake()
        protected override void InitialiseSelector()
        {
            reticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(reticle.GetComponent<Collider>());
            reticle.transform.localScale = new Vector3(reticleScale, reticleScale, reticleScale);
            //reticle.transform.parent = transform;
            reticle.name = "Reticle";
            Renderer rend = reticle.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Unlit/Color"));
            reticle.GetComponent<Renderer>().material.color = reticleColour;
            reticle.SetActive(useReticle);
        }

        /*protected override VRSelectable GetSelectable()
        {
            if (isPointerActive)
            {
                //VRSelectable obj = GetElement<VRSelectable>();
                if (_currentObservedSelectable != null && !_currentObservedSelectable.CanSelectWithSight())
                {
                    _currentObservedSelectable = null;
                }
                if (_currentObservedSelectable != null)
                {
                    selectableActive = true;
                    setCurrentColour(hoverColour);
                } else
                {
                    selectableActive = false;
                    if(_grabbedObject == null) setCurrentColour(MainColour);
                }
                drawLine();
                return _currentObservedSelectable;

            } else
            {
                if(_currentObservedSelectable != null && usePassiveBeam)
                {
                    setCurrentColour(beamColour);
                    drawLine();
                } else
                {
                    Line.enabled = false;
                }
                
                return null;
            }
        }

        protected override VRGrabbable GetGrabbable()
        {
            if (isPointerActive)
            {
                if (_grabbedObject != null)
                    return _grabbedObject;
                _grabbedObject = GetElement<VRGrabbable>();
                if (_grabbedObject != null)
                {
                    distanceToSelectable = Vector3.Distance(_grabbedObject.transform.position, transform.position);
                    setCurrentColour(hoverColour);
                }
                else
                {
                    //grabbable = false;
                }
                drawLine();
                return _grabbedObject;

            }
            else
            {
                _grabbedObject = null;
                if (_currentObservedSelectable == null) Line.enabled = false;
                return null;
            }
        }

        protected override void ChildUpdate()
        {
            _currentObservedSelectable = GetElement<VRSelectable>();
            if (_currentObservedSelectable != null) distanceToSelectable = Vector3.Distance(_currentObservedSelectable.transform.position, transform.position);

            if (grabTrigger != null)
            {
                isPointerActive = grabTrigger.Triggered();
            } else
            {
                isPointerActive = false;
            }
            if(!selectableActive && _grabbedObject == null && _currentObservedSelectable == null || _currentObservedSelectable == null && _grabbedObject == null)
            {
                    distanceToSelectable = selectionDistance;
            }
            if (useReticle) drawReticle();
        }*/

        protected override VRSelectable GetSelectable()
        {
            return _currentObservedSelectable;
        }

        protected override VRGrabbable GetGrabbable()
        {
            return _grabbedObject;
        }

        protected override void ChildUpdate()
        {
            VRElement currentElement = GetElement<VRElement>();
            bool draw = false;
            bool recalculateDistance = false;
            if (grabTrigger != null)
            {
                isPointerActive = grabTrigger.Triggered();
            }
            else
            {
                isPointerActive = false;
                Line.enabled = false;
            }
            if(isPointerActive)
            {
                if (currentElement != null && _currentObservedElement == null) recalculateDistance = true;
                draw = true;
                if(_currentObservedElement == null) _currentObservedElement = currentElement;
                if (_currentObservedElement != null)
                {
                    VRSelectable sel = _currentObservedElement as VRSelectable;
                    _currentObservedSelectable = sel != null ? (sel.CanSelectWithPointer() ? sel : null) : null;
                    _grabbedObject = _currentObservedElement as VRGrabbable;
                    if(_currentObservedSelectable != null || _grabbedObject != null)
                    {
                        setCurrentColour(hoverColour);
                    } else
                    {
                        setCurrentColour(MainColour);
                    }
                }
            }
            else
            {
                if (currentElement == null || !usePassiveBeam)
                {
                    setCurrentColour(MainColour);
                    Line.enabled = false;
                    distanceToSelectable = selectionDistance;
                    if (_previousObservedElement as VRSelectable != null) clearTooltip(_previousObservedElement as VRSelectable);

                }
                else if(usePassiveBeam)
                {
                    recalculateDistance = true;// distanceToSelectable = Vector3.Distance(currentElement.transform.position, transform.position);
                    setCurrentColour(beamColour);
                    draw = true;
                    if (currentElement as VRSelectable != null) setTooltip(currentElement as VRSelectable);
                }
                if (currentElement != null) recalculateDistance = true;//distanceToSelectable = Vector3.Distance(currentElement.transform.position, transform.position);

                _currentObservedElement = null;
                _currentObservedSelectable = null;
                _grabbedObject = null;

            }
            if(recalculateDistance)
            {
                distanceToSelectable = Vector3.Distance(currentElement.transform.position, transform.position);
            }
            if (draw) drawLine();
            if (useReticle) drawReticle();
            _previousObservedElement = currentElement;
        }

        public override Vector3 GetEndPointPosition()
        {
            return transform.position + (transform.forward * distanceToSelectable);

        }

        public void ConfigureRigidbody()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        private void drawLine()
        {
            // use line renderer to draw select line?? https://docs.unity3d.com/Manual/class-LineRenderer.html
            Line.enabled = true;
            Line.SetPosition(0, transform.position);
            Line.SetPosition(1, transform.position + transform.forward * distanceToSelectable);
            

        }

        private void drawReticle()
        {
            // draw reticle (aid to select)
            if(reticle != null) reticle.transform.position = transform.position + transform.forward * distanceToSelectable;
        }

        private void setCurrentColour(Color col)
        {
            Line.sharedMaterial.color = col;
        }
    }
}