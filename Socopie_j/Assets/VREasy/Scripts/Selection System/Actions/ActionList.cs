using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if VREASY_VREE_PLATFORM_SDK
using VREasy.Networking.Commands;
using VREasy.Networking;
#endif
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
#if VREASY_VREE_PLATFORM_SDK
    public class ActionList : MonoBehaviour, IActionListNetworkEvents
#else
    public class ActionList : MonoBehaviour
#endif
    {
        public event Action OnTriggerEvent;

        public List<VRAction> list = new List<VRAction>();

        public void Trigger()
        {
            VRActionManager.instance.ExecuteActions(list);
            if (OnTriggerEvent != null)
                OnTriggerEvent();
            //StartCoroutine(triggerActions());
        }        

        /*private IEnumerator triggerActions()
        {
            foreach (VRAction a in list)
            {
                if (a != null)
                {
                    yield return new WaitForSeconds(a.delay);
                    a.Trigger();
                }
                
            }
        }*/
    }
}