/*
 * Script inspired by: http://www.theappguruz.com/blog/bezier-curve-in-games
 */

using UnityEngine;
using System.Collections.Generic;

namespace VREasy
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SplineController : MonoBehaviour
    {

        public float LineWidth
        {
            set
            {
                bool needsRedraw = _lineWidth != value;
                _lineWidth = value;
                if(needsRedraw) DrawCurve();
            }
            get
            {
                return _lineWidth;
            }
        }
        public float _lineWidth = 1.0f;

        public int VerticalAngle
        {
            set
            {
                bool needsRedraw = _verticalAngle != value;
                _verticalAngle = value;
                up = Quaternion.AngleAxis(-_verticalAngle, Vector3.forward) * Vector3.up;
                if (needsRedraw) DrawCurve();
            }
            get
            {
                return _verticalAngle;
            }
        }
        public int _verticalAngle = 0;

        public MeshFilter Mesh_Filter
        {
            get
            {
                if (meshFilter == null)
                {
                    meshFilter = GetComponent<MeshFilter>();
                }
                return meshFilter;
            }
        }
        private MeshFilter meshFilter;

        private MeshRenderer meshRenderer
        {
            get
            {
                if(_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                }
                return _meshRenderer;
            }
        }
        private MeshRenderer _meshRenderer;

        public Texture texture
        {
            get
            {
                if (meshRenderer == null) return null;
                return meshRenderer.sharedMaterial.mainTexture;
            }
            set
            {
                meshRenderer.sharedMaterial.mainTexture = value;
            }
        }
        

        public int ArrowCount
        {
            get
            {
                return _arrowCount;
            }
            set
            {
                bool needsRedraw = _arrowCount != value;
                _arrowCount = value;
                if (needsRedraw) DrawCurve();
            }
        }
        public int _arrowCount = 10;

        public List<Transform> ControlPoints = new List<Transform>();
        public float ScrollSpeed = 1.0f;
        public Vector3 up = Vector3.up;
        
        private Vector4 uvOffset = new Vector4(0,0,0,0);
        
        
        void Start()
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = new Material(Resources.Load<Material>("SplineArrow"));
            DrawCurve();
        }
        void Update()
        {
            uvOffset.x = -ScrollSpeed * Time.time;
            uvOffset.y = 0;
            meshRenderer.sharedMaterial.SetTextureOffset("_MainTex",uvOffset);
        }

        private void OnDestroy()
        {
            //DestroyImmediate(meshRenderer.sharedMaterial);
        }

        public void DrawCurve()
        {
            List<Vector3> transformedPoints = CalculatePoints();
            for(int ii=0; ii<transformedPoints.Count; ii++)
            {
                transformedPoints[ii] = transform.InverseTransformPoint(transformedPoints[ii]);
            }
            VREasy_utils.MeshFromPoints(transformedPoints, Mesh_Filter, up, LineWidth);

        }

        public List<Vector3> CalculatePoints()
        {
            List<Vector3> points = new List<Vector3>();
            GetControlPoints();
            for (int triple = 0; triple < ControlPoints.Count - 2; triple += 2)
            {
                Vector3 lineStart = getPoint(ControlPoints[triple].position,
                                            ControlPoints[triple + 1].position,
                                            ControlPoints[triple + 2].position,
                                            0f);
                points.Add(lineStart);
                for (int ii = 0; ii < ArrowCount; ii++)
                {
                    Vector3 lineEnd = getPoint(ControlPoints[triple].position,
                                            ControlPoints[triple + 1].position,
                                            ControlPoints[triple + 2].position,
                                            ii / (float)ArrowCount);
                    points.Add(lineEnd);
                }
            }

            return points;
        }

        private Vector3 getPoint(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(a, b, t),
                                Vector3.Lerp(b, c, t),
                                t);
        }

        public List<Transform> GetControlPoints()
        {
            List<int> marked = new List<int>();

            for (int ii = 0; ii < ControlPoints.Count; ii++)
            {
                if  (ControlPoints[ii] == null)
                {
                    marked.Add(ii);
                }
            }
            for(int ii = marked.Count-1; ii >= 0; ii--)
            {
                ControlPoints.RemoveAt(ii);
            }

            return ControlPoints;
        }
    }
}