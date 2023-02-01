using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    [ExecuteInEditMode]
    public class MouseHMD : MonoBehaviour
    {
        public float speed = 2.0f;
        public bool inverseY = false;
        public bool hideMouse = true;

        private void Awake()
        {
#if !UNITY_EDITOR
            if (hideMouse) Cursor.visible = !hideMouse;
#endif
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * (inverseY ? 1 : -1) * speed,Space.Self);
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * speed, Space.World);
        }

    }


}