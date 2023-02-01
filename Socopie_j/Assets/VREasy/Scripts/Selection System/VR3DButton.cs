using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    //[RequireComponent(typeof(Renderer))]
    //[RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(AudioSource))]
    //[RequireComponent(typeof(ActionList))]
    [ExecuteInEditMode]
    public class VR3DButton : VRSelectable_colour
    {

        protected override void activate(bool state)
        {
            // nothing extra
        }

    }


}
