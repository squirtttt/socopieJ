using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    public class SplineCreatorHelper : EditorWindow
    {

        [MenuItem("VREasy/Path creator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SplineCreatorHelper), false, "Path Creator");
        }

        public SplineController _controller;

        private static Vector2 scrollPos;

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

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Create an animated path by adding and positioning points");

            EditorGUILayout.Separator();
            _controller = (SplineController)EditorGUILayout.ObjectField("Path object", _controller, typeof(SplineController), true);
            EditorGUILayout.Separator();

            if (_controller == null)
            {
                if (GUILayout.Button("Create path"))
                {
                    GameObject go = new GameObject("ArrowPath");
                    _controller = go.AddComponent<SplineController>();
                }
            }
            else
            {
                SplineControllerEditor.ConfigureSplineController(_controller);
            }

            EditorGUILayout.EndScrollView();

        }

        void OnInspectorUpdate()
        {
            Repaint();
        }


    }
}