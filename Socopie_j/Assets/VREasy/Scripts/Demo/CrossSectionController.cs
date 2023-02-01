using UnityEngine;
using System.Collections;

namespace VREasy
{
    [ExecuteInEditMode]
    public class CrossSectionController : MonoBehaviour
    {

        public GameObject crossSectionPlane;

        private Vector3 normal;
        private Vector3 position;
        private Renderer rend;
        
        void Start()
        {
            rend = GetComponent<Renderer>();
        }
        void Update()
        {
            if (crossSectionPlane != null)
            {
                normal = crossSectionPlane.transform.TransformVector(new Vector3(0, 1, 0));
                position = crossSectionPlane.transform.position;
                rend.sharedMaterial.SetVector("_PlaneNormal", normal);
                rend.sharedMaterial.SetVector("_PlanePosition", position);
            }
        }
    }
}