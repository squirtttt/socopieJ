using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace VREasy
{
    public class VRActionManager : MonoBehaviour
    {
        public static VRActionManager instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject ob = new GameObject();
                    ob.name = "[VREasy]VRActionManager";
                    _instance = ob.AddComponent<VRActionManager>();
                    DontDestroyOnLoad(ob);
                }
                return _instance;
            }
        }
        private static VRActionManager _instance = null;

        public void ExecuteActions(List<VRAction> actions)
        {
            StartCoroutine(executeActions(actions));
        }

        private IEnumerator executeActions(List<VRAction> actions)
        {
            foreach (VRAction a in actions.ToArray())
            {
                if (a != null)
                {
                    yield return new WaitForSeconds(a.delay);
                    a.Trigger();
                }

            }
        }
    }
}
