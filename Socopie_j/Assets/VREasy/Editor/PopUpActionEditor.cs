using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(PopUpAction))]
    public class PopUpActionEditor : Editor
    {
        public static bool showPanelInEditor = true;

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
            PopUpAction image = (PopUpAction)target;

            ConfigurePopUpAction(ref image);
            
        }

        public static void ConfigurePopUpAction(ref PopUpAction image)
        {
            EditorGUI.BeginChangeCheck();
            DISPLAY_IMAGE_TYPE t = (DISPLAY_IMAGE_TYPE)EditorGUILayout.EnumPopup("Image type", image.type);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(image, "Display type changed");
                if (t != image.type)
                {
                    DestroyImmediate(image.Template);
                    image.type = t;
                }
            }

            EditorGUI.BeginChangeCheck();
            float hideTime = EditorGUILayout.FloatField("Hiding time", image.hideTime);
            Object newImage = null;
            switch (image.type)
            {
                case DISPLAY_IMAGE_TYPE.SPRITE:
                    newImage = displayImageSlot<Sprite>(image);
                    break;
                case DISPLAY_IMAGE_TYPE.TEXTURE:
                    newImage = displayImageSlot<Texture2D>(image);
                    break;
            }
            Vector2 imageScale = EditorGUILayout.Vector2Field("Image scale", image.TemplateScale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(image, "Changed image");
                image.Image = newImage;
                image.hideTime = hideTime;
                image.TemplateScale = imageScale;
            }

#if UNITY_EDITOR
            EditorGUILayout.Separator();
            showPanelInEditor = EditorGUILayout.Toggle("Show panel in editor", showPanelInEditor);
            if (!Application.isPlaying)
            {
                if (showPanelInEditor)
                    image.Activate();
                else image.Hide();
            }
#endif
        }

        private static Object displayImageSlot<T>(PopUpAction image)
        {
            return (Object)EditorGUILayout.ObjectField("Image", image.Image, typeof(T), true);
        }

        
    }
}