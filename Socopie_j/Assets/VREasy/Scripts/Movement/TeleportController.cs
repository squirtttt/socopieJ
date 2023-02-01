using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VREasy
{
    [RequireComponent(typeof(TeleportAction))]
    public class TeleportController : MonoBehaviour
    {
        public TELEPORT_CONTROLLER_RENDERER type = TELEPORT_CONTROLLER_RENDERER.LINE;
        public TELEPORT_CONTROLLER_BEAM beam = TELEPORT_CONTROLLER_BEAM.PARABOLE;

        public VRGrabTrigger trigger;
        public LayerMask walkableLayers;
        public float maxStepDown = 10f;
        public float reach = 1.0f; // valid values between 1 and 4 (ONLY FOR PARABOLES)
        public float max_straight_distance = 10f; // max beam distance (ONLY FOR STRAIGHTS)
        public Sprite landingSprite;
        public float landingSize = 1f;
        public float step = 0.35f;
        // Texture based renderer
        public Texture2D validTexture;
        public Texture2D invalidTexture;
        public float ScrollSpeed = 1f;
        private Vector2 uvOffset = Vector2.zero;
        // Line based renderer
        public Color validMoveColour = Color.blue;
        public Color invalidMoveColour = Color.red;

        public float PlayerHeightCorrection {
            get
            {
                return _playerHeightCorrection;
            }

            set
            {
                if (Teleport != null && Teleport.HMD != null)
                {
                    Teleport.HMD.position = Teleport.HMD.position + Vector3.up * value;
                }
                _playerHeightCorrection = value;
            }

        }
        public float _playerHeightCorrection = 1.8f;


        private const string LINE_OBJECT = "[VREasy]Line";

        public MeshFilter MeshFilter
        {
            set
            {
                _meshFilter = value;
            }
            get
            {
                GameObject lineChild = createLineGameObject();
                if(_meshFilter == null)
                {
                    _meshFilter = lineChild.GetComponent<MeshFilter>();
                    if(_meshFilter == null)
                    {
                        _meshFilter = lineChild.AddComponent<MeshFilter>();
                    }
                    return _meshFilter;
                }

                return _meshFilter;
            }
        }
        public MeshFilter _meshFilter;
        public MeshRenderer meshRenderer
        {
            get
            {
                if(_meshRenderer == null)
                {
                    _meshRenderer = createLineGameObject().GetComponent<MeshRenderer>();
                    if(_meshRenderer == null)
                    {
                        _meshRenderer = createLineGameObject().AddComponent<MeshRenderer>();
                        _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        _meshRenderer.material = new Material(Resources.Load<Material>("SplineArrow"));
                    }
                    
                }
                return _meshRenderer;
            }
            set
            {
                _meshRenderer = value;
            }
        }
        public MeshRenderer _meshRenderer;

        public LineRenderer Parabola {
            get
            {
                if (parabola == null)
                {
                    GameObject l = createLineGameObject();
                    parabola = l.AddComponent<LineRenderer>();
                    parabola.sharedMaterial = Resources.Load<Material>("TeleportPointer") as Material;
#if UNITY_5_4 || UNITY_5_5
                    parabola.SetWidth(0.05f, 0.05f);
#else
                    parabola.startWidth = 0.05f;
                    parabola.endWidth = 0.05f;
#endif
                    parabola.receiveShadows = false;
                    parabola.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                return parabola;
            }
            set
            {
                parabola = value;
            }
        }
        public LineRenderer parabola;

        public float LineThickness
        {
            get
            {
                return _lineThickness;
            }
            set
            {
                _lineThickness = value;
                if (type == TELEPORT_CONTROLLER_RENDERER.LINE)
                {
#if UNITY_5_4 || UNITY_5_5
                    Parabola.SetWidth(_lineThickness, _lineThickness);
#else
                    Parabola.startWidth = _lineThickness;
                    Parabola.endWidth = _lineThickness;
#endif
                }

            }
        }
        public float _lineThickness = 0.2f;

        public TeleportAction Teleport
        {
            get
            {
                if(teleportAction == null)
                {
                    teleportAction = GetComponent<TeleportAction>();
                }
                if(teleportAction == null)
                {
                    teleportAction = gameObject.AddComponent<TeleportAction>(); // required for backwards compatibility (1.1 did not have TeleportAction as RequiredComponent)
                }
                return teleportAction;
            }
        }
        private TeleportAction teleportAction;

        private bool hasFuturePosition = false;
        private Vector3 futurePosition = Vector3.zero;
        private List<Vector3> positions = new List<Vector3>();
        //private float maxLength = 15f;
        private GameObject teleportCircle;

        private void Awake()
        {
            teleportCircle = Resources.Load<GameObject>("TeleportCircle") as GameObject;
            if(teleportCircle == null)
            {
                Debug.Log("[VREasy] TeleportController: TeleportCircle object not found in Resources folder. Cannot display teleport circle.");
            } else
            {
                teleportCircle = GameObject.Instantiate(teleportCircle);
                if(landingSprite != null)
                {
                    SpriteRenderer sp = teleportCircle.GetComponent<SpriteRenderer>();
                    if(sp != null) sp.sprite = landingSprite;
                }
                teleportCircle.transform.localScale = new Vector3(landingSize, landingSize, landingSize);
                teleportCircle.SetActive(false);
            }
            PlayerHeightCorrection = _playerHeightCorrection; // force resetting of players height
        }

        void Update()
        {
            if(trigger != null && trigger.Triggered())
            {
                // Shoot parabole and detect future position
                hasFuturePosition = createAndDetectParabole();
                switch(type)
                {
                    case TELEPORT_CONTROLLER_RENDERER.LINE:
                        {
                            Parabola.material.color = hasFuturePosition ? validMoveColour : invalidMoveColour;
                        }
                        break;
                    case TELEPORT_CONTROLLER_RENDERER.TEXTURES:
                        {
                            meshRenderer.sharedMaterial.mainTexture = hasFuturePosition ? validTexture: invalidTexture;
                            uvOffset.x = -ScrollSpeed * Time.time;
                            meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", uvOffset);
                        }
                        break;
                }
                
            }
            else if(hasFuturePosition)
            {
                // if future destination is set, teleport
                teleport();
            } else
            {
                switch(type)
                {
                    case TELEPORT_CONTROLLER_RENDERER.LINE:
                        {
#if UNITY_5_4 || UNITY_5_5
                            Parabola.SetVertexCount(0);
#else
                            Parabola.positionCount = 0;
#endif
                        }
                        break;
                    case TELEPORT_CONTROLLER_RENDERER.TEXTURES:
                        {
                            MeshFilter.mesh.Clear();
                        }
                        break;
                }
                
            }
            if (hasFuturePosition)
            {
                if (teleportCircle != null)
                {
                    teleportCircle.transform.position = futurePosition + Vector3.up * 0.1f;
                    teleportCircle.transform.eulerAngles = new Vector3(90, 0, 0);
                    teleportCircle.SetActive(true);
                }
            } else
            {
                if(teleportCircle != null) teleportCircle.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (type == TELEPORT_CONTROLLER_RENDERER.TEXTURES)
            {
                DestroyImmediate(meshRenderer.sharedMaterial);
            }
        }

        private GameObject createLineGameObject()
        {
            Transform t = transform.Find(LINE_OBJECT);
            if(t == null)
            {
                GameObject l = new GameObject(LINE_OBJECT);
                l.transform.parent = transform;
                l.transform.localPosition = Vector3.zero;
                l.transform.localRotation = Quaternion.identity;
                return l;

            }
            return t.gameObject;
        }

        private bool createAndDetectParabole()
        {
            if(Time.frameCount % 2 != 0) return hasFuturePosition;
            float angle = Vector3.Dot(Vector3.down, transform.forward.normalized);
            angle = (1-(1 + angle)) * 45f;
            return StartTrajectory(angle/Mathf.Clamp(5f - reach,0.5f,5f), angle);
        }

        // AIM HINT
        public bool StartTrajectory(float v, float angle) {
            
            hasFuturePosition = false;

            Vector3 origin = transform.position;
            Vector3 prevPoint = origin;
            Vector3 forward = transform.forward;
            positions.Clear();

            switch (beam)
            {
                case TELEPORT_CONTROLLER_BEAM.PARABOLE:
                    {
                        forward.y = 0;
                        forward.Normalize();
                        float g = Mathf.Abs(10f);
                        float a = Mathf.Deg2Rad * angle;
                        float count = 0.1f;
                        float x, y;
                        float a_tan = Mathf.Tan(a);
                        float pow = (2 * Mathf.Pow(v * Mathf.Cos(a), 2));
                        do
                        {
                            Vector3 currentPoint = (origin + forward * count);
                            x = count;
                            y = x * a_tan - g * Mathf.Pow(x, 2) / pow;
                            currentPoint.y += y;

                            // add currentPoint to renderer
                            positions.Add(getArchPoint(currentPoint));
                            // raycast from prevPoint to currentPoint and stop if found collider
                            if (currentPoint.y - prevPoint.y < 0) // only check for collision when going downwards?
                            {
                                RaycastHit hit;
                                if (Physics.Raycast(prevPoint, (currentPoint - prevPoint), out hit, step * 2f, 1 << walkableLayers))
                                {
                                    futurePosition = hit.point;
                                    hasFuturePosition = true;
                                    // add extra point to avoid gaps in rendering line when it collides with plane
                                    positions.Add(getArchPoint(currentPoint + (currentPoint - prevPoint)));
                                    break;
                                }
                            }
                            count += step;
                            prevPoint = currentPoint;
                        } while (origin.y + y > transform.position.y - maxStepDown);//while(count <= maxLength);
                    }
                    break;
                case TELEPORT_CONTROLLER_BEAM.STRAIGHT:
                    {
                        positions.Add(origin);
                        Vector3 futureDistance = origin + forward * max_straight_distance;
                        positions.Add(futureDistance);
                        RaycastHit hit;
                        if (Physics.Raycast(origin, futureDistance, out hit, step * 2f, 1 << walkableLayers))
                        {
                            futurePosition = hit.point;
                            hasFuturePosition = true;
                        }
                    }
                    break;
            }
            /*//float mass = 1f;
            //float v = force / mass;
            float g = Mathf.Abs(10f);
		    float a = Mathf.Deg2Rad* angle;
            //float maxDistance = v* Mathf.Cos(a)/g* (v* Mathf.Sin(a) + Mathf.Sqrt(Mathf.Pow(v* Mathf.Sin(a),2) + 2*g* transform.position.y));
		    
            Vector3 origin = transform.position;
		    Vector3 prevPoint = origin;
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
		    float count = 0.1f;
            float x, y;
            positions.Clear();
            float a_tan = Mathf.Tan(a);
            float pow = (2 * Mathf.Pow(v * Mathf.Cos(a), 2));
            do {
                Vector3 currentPoint = (origin + forward * count);
                x = count;
                y = x * a_tan - g * Mathf.Pow(x, 2) / pow;
                currentPoint.y += y;

                // add currentPoint to renderer
                positions.Add(getArchPoint(currentPoint));
                // raycast from prevPoint to currentPoint and stop if found collider
                if (currentPoint.y - prevPoint.y < 0) // only check for collision when going downwards?
                {
                    RaycastHit hit;
                    if (Physics.Raycast(prevPoint, (currentPoint - prevPoint), out hit, step * 2f, 1 << walkableLayers))
                    {
                        futurePosition = hit.point;
                        hasFuturePosition = true;
                        // add extra point to avoid gaps in rendering line when it collides with plane
                        positions.Add(getArchPoint(currentPoint + (currentPoint - prevPoint)));
                    }
                }
                count += step;
                prevPoint = currentPoint;
            } while(origin.y + y > transform.position.y - maxStepDown);//while(count <= maxLength);
            */
            switch(type)
            {
                case TELEPORT_CONTROLLER_RENDERER.LINE:
                    {
#if UNITY_5_4 || UNITY_5_5
                        Parabola.SetVertexCount(positions.Count);
#else
                        Parabola.positionCount = positions.Count;
#endif
                        Parabola.SetPositions(positions.ToArray());
                    }
                    break;
                case TELEPORT_CONTROLLER_RENDERER.TEXTURES:
                    {
                        VREasy_utils.MeshFromPoints(positions, MeshFilter, Vector3.up, LineThickness);
                    }
                    break;
            }
            
            return hasFuturePosition;
	    }

        private void teleport()
        {
            // reset
            hasFuturePosition = false;
            
            Teleport.teleport(futurePosition + Vector3.up * PlayerHeightCorrection);
            
        }

        public void ResetRenderers()
        {
            switch (type)
            {
                case TELEPORT_CONTROLLER_RENDERER.LINE:
                    {
                        DestroyImmediate(meshRenderer);
                        DestroyImmediate(MeshFilter);
                    }
                    break;
                case TELEPORT_CONTROLLER_RENDERER.TEXTURES:
                    {
                        DestroyImmediate(Parabola);
                    }
                    break;
            }
        }

        private Vector3 getArchPoint(Vector3 point)
        {
            switch (type)
            {
                case TELEPORT_CONTROLLER_RENDERER.LINE:
                    return point;
                case TELEPORT_CONTROLLER_RENDERER.TEXTURES:
                    return transform.InverseTransformPoint(point);
            }
            return point;
        }
        

    }
}