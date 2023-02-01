using UnityEngine;
#if VREASY_VREE_PLATFORM_SDK
using VREasy.Networking;
#endif

namespace VREasy
{
    [System.Serializable]
    #if VREASY_VREE_PLATFORM_SDK
    public class VRElement : NetworkedElement
#else
    public class VRElement : MonoBehaviour
#endif
    {
        public bool active = true;
        public float init_transition = 0f;
        public float activation_transition = 0f;       

        public void DeactivateElement()
        {
            active = false;
            activate(active);
        }        

        public void ReactivateElement()
        {
            active = true;
            activate(active);
        }        

        protected virtual void activate(bool state)
        {
            activateAll(active);
        }

        private void activateAll(bool state)
        {
            Collider[] cols = GetComponentsInChildren<Collider>();
            foreach (Collider col in cols)
            {
                col.enabled = state;
            }

            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                rend.enabled = state;
            }
            Canvas[] cns = GetComponentsInChildren<Canvas>();
            foreach (Canvas c in cns)
            {
                c.enabled = state;
            }
        }
       

    }
}