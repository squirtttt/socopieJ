using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace VREasy
{
    public class SwapObjectsAction : VRAction
    {
        public List<GameObject> swapObjects = new List<GameObject>();

        private int _swapIndex = 0;

        public override void Trigger()
        {
            if (!updateSwapCount())
                return;

            swapGOs();
        }

        private bool updateSwapCount()
        {
            if (swapObjects.Count == 0)
            {
                Debug.Log("SwitchAction: no swappable objects are set.");
                return false;
            }
            _swapIndex++;
            if (_swapIndex >= swapObjects.Count)
                _swapIndex = 0;
            return true;
        }

        private void swapGOs()
        {
            for(int ii = 0; ii < swapObjects.Count; ii++)
            {
                swapObjects[ii].SetActive(ii == _swapIndex);
            }

        }
        
    }


}