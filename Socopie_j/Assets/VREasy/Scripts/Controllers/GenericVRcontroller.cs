using UnityEngine;
using System.Collections;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace VREasy
{
    public class GenericVRcontroller : MonoBehaviour
    {
#if UNITY_2017_2_OR_NEWER
        public XRNode vrnode = XRNode.LeftHand;
#else
        public VRNode vrnode = VRNode.LeftHand;
#endif
        public bool autohandconfig = true;
        private bool state = false;

        private void Awake()
        {
#if !UNITY_5_5_OR_NEWER
            Debug.LogWarning("[VREasy] GenericVRcontroller: API requires Unity 5.5.x and above.");
            activateNode(state);
#else

            /*if (autohandconfig)
            {
                configureController();
            }*/
            activateNode(state);

#endif
        }

        private void activateNode(bool s)
        {
            state = s;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(state);
            }
            foreach (MonoBehaviour c in GetComponentsInChildren<MonoBehaviour>())
            {
                if (this != c) c.enabled = state;
            }
        }

        void LateUpdate()
        {
#if UNITY_2019_2_OR_NEWER
            System.Collections.Generic.List<XRNodeState> nodeStates = new System.Collections.Generic.List<XRNodeState>();
            InputTracking.GetNodeStates(nodeStates);
            XRNodeState nodeState = new XRNodeState();

            foreach (var state in nodeStates)
            {
                if (state.nodeType == vrnode)
                {
                    nodeState = state;
                }
            }


            Vector3 nodePosition = new Vector3();
            Quaternion nodeRotation = new Quaternion();
            nodeState.TryGetPosition(out nodePosition);
            nodeState.TryGetRotation(out nodeRotation);

            if (!state && nodePosition != Vector3.zero)
            {
                configureController();
            }
            // update position and rotation based on the VRNode information (local)
            transform.localPosition = nodePosition;
            transform.localRotation = nodeRotation;
#elif UNITY_5_5_OR_NEWER
            if (!state && InputTracking.GetLocalPosition(vrnode) != Vector3.zero) {
                configureController();
            }
            // update position and rotation based on the VRNode information (local)
            transform.localPosition = InputTracking.GetLocalPosition(vrnode);
            transform.localRotation = InputTracking.GetLocalRotation(vrnode);
#endif
        }

        private void configureController()
        {
            if (autohandconfig)
            {
                string[] joystickNames = Input.GetJoystickNames();
                for (int ii = 0; ii < joystickNames.Length; ii++)
                {
                    if (joystickNames[ii].ToLower().Contains("left") && name.ToLower().Contains("left"))
                    {
#if UNITY_2017_2_OR_NEWER
                        vrnode = XRNode.LeftHand;
#else
                        vrnode = VRNode.LeftHand;
#endif
                        state = true;
                    }
                    if (joystickNames[ii].ToLower().Contains("right") && name.ToLower().Contains("right"))
                    {
#if UNITY_2017_2_OR_NEWER
                        vrnode = XRNode.RightHand;
#else
                        vrnode = VRNode.RightHand;
#endif
                        state = true;
                    }
                }
            } else
            {
#if UNITY_2019_2_OR_NEWER
                System.Collections.Generic.List<XRNodeState> nodeStates = new System.Collections.Generic.List<XRNodeState>();
                InputTracking.GetNodeStates(nodeStates);
                XRNodeState nodeState = new XRNodeState();

                foreach (var state in nodeStates)
                {
                    if (state.nodeType == vrnode)
                    {
                        nodeState = state;
                    }
                }


                Vector3 nodePosition = new Vector3();
                Quaternion nodeRotation = new Quaternion();
                nodeState.TryGetPosition(out nodePosition);
                nodeState.TryGetRotation(out nodeRotation);
#else

                state = InputTracking.GetLocalPosition(vrnode) != Vector3.zero;
#endif
            }
            activateNode(state);
        }
    }
}