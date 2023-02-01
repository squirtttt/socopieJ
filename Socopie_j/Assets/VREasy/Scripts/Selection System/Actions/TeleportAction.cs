using UnityEngine;
using System.Collections;
using System;
#if BATTLE_HUD_SDK
using Battlehub.RTCommon;
#endif

namespace VREasy
{
    public class TeleportAction : VRAction
    {
        public Transform targetPosition;
        public Transform HMD;
        public float fadeTimer = 0.3f;

        private LoadSceneManager _loadSceneManager = null;


        void Awake()
        {
            _loadSceneManager = LoadSceneManager.instance;
        }

        public override void Trigger()
        {
            teleport(targetPosition.position);
        }

        public void teleport(Vector3 futurePosition)
        {
            if(HMD != null)
            {
                StartCoroutine(doTeleport(futurePosition));
            }
            
        }

        private IEnumerator doTeleport(Vector3 futurePosition)
        {
            if (HMD != null)
            {
                _loadSceneManager.FadeOut(fadeTimer);
                yield return new WaitForSeconds(fadeTimer * 1.1f);
                // must check if teleported object has a collider
                // if so, place the object with enough vertical space to avoid collider overlaps
                HMD.position = correctHeight(HMD.position,futurePosition);
                _loadSceneManager.FadeIn(fadeTimer);
            }
        }

        private Vector3 correctHeight(Vector3 currentPos, Vector3 futurePosition)
        {
#if BATTLE_HUD_SDK
            return futurePosition;
#else

            Vector3 corrected = futurePosition;

            Collider[] cols = HMD.GetComponentsInChildren<Collider>();
            float biggestMargin = 0;
            foreach (Collider col in cols)
            {
                if (col.GetType() == typeof(MeshCollider) || col.isTrigger) continue;
                float margin = col.bounds.center.y + col.bounds.extents.y / 2;
                if (biggestMargin < margin)
                {
                    if(col.transform.position.y - margin < futurePosition.y)
                        biggestMargin = margin - (col.transform.position.y - futurePosition.y);
                }
            }
            if (biggestMargin > 0)
            {
                corrected.y += biggestMargin;
            }
            return corrected;
#endif
        }

    }
}