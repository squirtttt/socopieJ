using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    public class SetFontMaterialEditor : EditorWindow
    {

        public Material targetMaterial;
        public Font targetFont;
        public TextMesh targetObject;

        [MenuItem("VREasy/Font Material setter")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SetFontMaterialEditor), false, "Font Setter");
        }

        bool handleRepaintErrors = false;
        void OnGUI()
        {
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Helper editor to set custom fonts-materials to TextMesh objects", EditorStyles.wordWrappedLabel);

            targetMaterial = (Material)EditorGUILayout.ObjectField("Target material", targetMaterial, typeof(Material), true);
            targetFont = (Font)EditorGUILayout.ObjectField("Target font", targetFont, typeof(Font), true);

            if(targetMaterial == null || targetFont == null)
            {
                EditorGUILayout.LabelField("Select the material and font to apply", EditorStyles.wordWrappedLabel);

            } else
            {
                targetObject = (TextMesh)EditorGUILayout.ObjectField("Target game object", targetObject, typeof(TextMesh), true);

                if (targetObject != null)
                {
                    if (GUILayout.Button("Apply"))
                    {
                        targetObject.font = targetFont;
                        targetObject.GetComponent<Renderer>().material = targetMaterial;
                        targetObject = null;
                    }
                }
            }
            
        }
    }
}