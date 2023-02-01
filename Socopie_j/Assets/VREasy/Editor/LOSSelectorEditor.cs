using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace VREasy
{
    [CustomEditor(typeof(LOSSelector))]
    public class LOSSelectorEditor : Editor
    {
        bool handleRepaintErrors = false;
        public override void OnInspectorGUI()
        {
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }
            LOSSelector selector = (LOSSelector)target;

            ConfigureLOSSelector(ref selector);
        }

        public static void ConfigureLOSSelector(ref LOSSelector selector)
        {
            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            LayerMask layerMask = EditorGUILayout.MaskField("Interactable layers",selector.layerMask,getAllLayers());
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selector, "losselector");
                selector.layerMask = layerMask;
            }
            EditorGUILayout.Separator();

        }

        private static string[] getAllLayers()
        {
            string[] list = new string[31];
            for(int ii=0; ii < 31; ii++)
            {
                string layer = LayerMask.LayerToName(ii);
                if (!string.IsNullOrEmpty(layer))
                    list[ii] = layer;
            }
            return list;
        }
    }
}