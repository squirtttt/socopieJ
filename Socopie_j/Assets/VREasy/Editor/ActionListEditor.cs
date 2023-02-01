using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(ActionList))]
    public class ActionListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // empty on purpose
        }
    }
}