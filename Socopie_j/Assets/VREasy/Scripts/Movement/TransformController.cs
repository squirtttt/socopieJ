using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class TransformController : MonoBehaviour
    {

        public float X_Position
        {
            set
            {
                position = transform.position;
                position.x = value;
                transform.position = position;
            }
            get
            {
                return transform.position.x;
            }
        }
        public float Y_Position
        {
            set
            {
                position = transform.position;
                position.y = value;
                transform.position = position;
            }
            get
            {
                return transform.position.y;
            }
        }
        public float Z_Position
        {
            set
            {
                position = transform.position;
                position.z = value;
                transform.position = position;
            }
            get
            {
                return transform.position.z;
            }
        }

        public float X_Rotation
        {
            set
            {
                rotation = transform.eulerAngles;
                rotation.x = value;
                transform.eulerAngles = rotation;
            }
            get
            {
                return transform.eulerAngles.x;
            }
        }
        public float Y_Rotation
        {
            set
            {
                rotation = transform.eulerAngles;
                rotation.y = value;
                transform.eulerAngles = rotation;
            }
            get
            {
                return transform.eulerAngles.y;
            }
        }
        public float Z_Rotation
        {
            set
            {
                rotation = transform.eulerAngles;
                rotation.z = value;
                transform.eulerAngles = rotation;
            }
            get
            {
                return transform.eulerAngles.z;
            }
        }

        private Vector3 position;
        private Vector3 rotation;
    }
}