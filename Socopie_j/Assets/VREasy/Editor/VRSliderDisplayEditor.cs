using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRSliderDisplay))]
    public class VRSliderDisplayEditor : Editor
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
            VRSliderDisplay sliderDisplay = (VRSliderDisplay)target;

            ConfigureSliderDisplay(ref sliderDisplay);
        }

        public static void ConfigureSliderDisplay(ref VRSliderDisplay sliderDisplay)
        {
            // aspect //
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Aspect", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            Sprite background = (Sprite)EditorGUILayout.ObjectField("Background image", sliderDisplay.Background, typeof(Sprite), true);
            EditorGUILayout.BeginHorizontal();
            float bScaleX = EditorGUILayout.FloatField("X scale", sliderDisplay.BackgroundScaleX);
            float bScaleY = EditorGUILayout.FloatField("Y scale", sliderDisplay.BackgroundScaleY);
            EditorGUILayout.EndHorizontal();
            Sprite handle = (Sprite)EditorGUILayout.ObjectField("Handle image", sliderDisplay.Handle, typeof(Sprite), true);
            EditorGUILayout.BeginHorizontal();
            float hScaleX = EditorGUILayout.FloatField("X scale", sliderDisplay.HandleScaleX);
            float hScaleY = EditorGUILayout.FloatField("Y scale", sliderDisplay.HandleScaleY);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(sliderDisplay, "Changed aspect");
                sliderDisplay.Background = background;
                sliderDisplay.Handle = handle;
                sliderDisplay.BackgroundScaleX = bScaleX;
                sliderDisplay.BackgroundScaleY = bScaleY;
                sliderDisplay.HandleScaleX = hScaleX;
                sliderDisplay.HandleScaleY = hScaleY;
            }

            // slider properties
            EditorGUILayout.Separator();
            VRSlider slider = sliderDisplay.Slider;
            EditorGUILayout.LabelField("Slider properties", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Attached to object [" + slider.name + "]");
            VRSliderEditor.ConfigureSlider(ref slider);
        }
    }
}