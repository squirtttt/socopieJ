using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace VREasy
{
    [CustomEditor(typeof(SplineController))]
    public class SplineControllerEditor : Editor
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

            ConfigureSplineController((SplineController)target);
            
        }

        private void OnSceneGUI()
        {
            // source : https://catlikecoding.com/unity/tutorials/curves-and-splines/
            SplineController curveController = target as SplineController;

            //List<Transform> controlPoints = curveController.ControlPoints;

            if (curveController.GetControlPoints().Count == 0) return;

            // draw handles for each control point
            for (int ii=0; ii < curveController.ControlPoints.Count; ii++)
            {
                Vector3 p = curveController.ControlPoints[ii].position;
                Quaternion rotation = Tools.pivotRotation == PivotRotation.Local ? curveController.ControlPoints[ii].rotation : Quaternion.identity;
                EditorGUI.BeginChangeCheck();
                p = Handles.DoPositionHandle(p,rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curveController, "move point");
                    // mark scene dirty to pick up the changes
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    curveController.ControlPoints[ii].position = p;
                    curveController.DrawCurve();
                }
            }

            // draw bezier curve
            List<Vector3> points = curveController.CalculatePoints();
            for(int ii=0; ii < points.Count-1; ii += 1)
            {
                Handles.DrawLine(points[ii], points[ii+1]);
            }
        }

        public static void ConfigureSplineController(SplineController _controller)
        {

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("General properties", EditorStyles.boldLabel);
            Texture texture = (Texture)EditorGUILayout.ObjectField("Texture", _controller.texture, typeof(Texture), true);
            float width = EditorGUILayout.FloatField("Arrow width", _controller.LineWidth);
            float scrollSpeed = EditorGUILayout.FloatField("Scroll speed", _controller.ScrollSpeed);
            int arrowCount = EditorGUILayout.IntField("Arrows per step", _controller.ArrowCount);

            int angle = EditorGUILayout.IntSlider("Vertical angle", _controller.VerticalAngle,0,360);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_controller, "Changed arrow properties");
                _controller.LineWidth = width;
                _controller.ScrollSpeed = scrollSpeed;
                _controller.ArrowCount = arrowCount;
                _controller.VerticalAngle = angle;
                _controller.texture = texture;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Control points", EditorStyles.boldLabel);
            bool redrawNeeded = false;
            _controller.GetControlPoints(); // make sure no points have null references
            for (int ii = 0; ii < _controller.ControlPoints.Count; ii++)
            {
                _controller.ControlPoints[ii] = (Transform)EditorGUILayout.ObjectField("Point" + ii, _controller.ControlPoints[ii], typeof(Transform), true);
                EditorGUILayout.BeginHorizontal();
                _controller.ControlPoints[ii].position = EditorGUILayout.Vector3Field("", _controller.ControlPoints[ii].position);
                if (GUILayout.Button("Delete"))
                {
                    DestroyImmediate(_controller.ControlPoints[ii].gameObject);
                    _controller.ControlPoints.RemoveAt(ii);
                    redrawNeeded = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
            if (GUILayout.Button("Add point"))
            {
                redrawNeeded = true;
                GameObject g = new GameObject("point" + _controller.ControlPoints.Count);
                g.transform.parent = _controller.transform;
                // set position equals to last point plus displacement
                if (_controller.ControlPoints.Count == 1 )
                {
                    g.transform.position = _controller.ControlPoints[_controller.ControlPoints.Count - 1].position + _controller.transform.forward;
                } else if (_controller.ControlPoints.Count > 1 )
                {
                    Vector3 displacement = _controller.ControlPoints[_controller.ControlPoints.Count - 1].position - _controller.ControlPoints[_controller.ControlPoints.Count - 2].position;
                    g.transform.position = _controller.ControlPoints[_controller.ControlPoints.Count - 1].position + displacement;
                }
                else
                {
                    g.transform.position = _controller.transform.forward;
                }
                _controller.ControlPoints.Add(g.transform);
            }
            // ensure number of points is adequate 
            if (_controller.ControlPoints.Count < 3)
            {
                EditorGUILayout.HelpBox("Minimum path points is 3. Add " + (3-_controller.ControlPoints.Count) + " more", MessageType.Warning);
                
            }
            else
            {
                int reminder = (_controller.ControlPoints.Count - 3) % 2;
                if (reminder > 0)
                {
                    EditorGUILayout.HelpBox("Please add " + (2 - reminder) + " more to complete path", MessageType.Error);
                }
            }

            EditorGUILayout.Separator();

            if(GUILayout.Button("Redraw"))
            {
                redrawNeeded = true;
            }

            if (redrawNeeded)
                _controller.DrawCurve();
            
            
            VREasy_utils.DrawHelperInfo();

            
        }
    }
}