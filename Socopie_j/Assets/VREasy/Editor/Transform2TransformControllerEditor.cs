using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(Transform2TransformController))]
    [CanEditMultipleObjects]
    public class Transform2TransformControllerEditor : Editor
    {

        [MenuItem("VREasy/Transform-based controllers/Transform2Transform controller")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<Transform2TransformController>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add a Transform to Transform controller you must select a game object in the hierarchy first", "OK");
            }
        }

        SerializedProperty originAxis;
        SerializedProperty originElement;
        SerializedProperty destinationAxis;
        SerializedProperty destinationElement;
        SerializedProperty origin;
        SerializedProperty destination;
        SerializedProperty mappingScale;

        private void OnEnable()
        {
            originElement = serializedObject.FindProperty("originElement");
            originAxis = serializedObject.FindProperty("originAxis");
            destinationElement = serializedObject.FindProperty("destinationElement");
            destinationAxis = serializedObject.FindProperty("destinationAxis");
            origin = serializedObject.FindProperty("origin");
            destination = serializedObject.FindProperty("destination");
            mappingScale = serializedObject.FindProperty("mappingScale");
        }

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
            Transform2TransformController selector = (Transform2TransformController)target;

            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("This component allows for a game object (destination) to observe and coordinate its transform (rotation or position) with another one in the scene (origin)", MessageType.Info);

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Origin (observed)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(origin);
            EditorGUILayout.PropertyField(originElement);
            EditorGUILayout.PropertyField(originAxis);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Mapping scale", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mappingScale);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Destination (controlled)", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Default: current transform");
            if (selector.destination == null) selector.destination = selector.transform;
            EditorGUILayout.PropertyField(destination);
            EditorGUILayout.PropertyField(destinationElement);
            EditorGUILayout.PropertyField(destinationAxis);

            

            serializedObject.ApplyModifiedProperties();
        }
    }

}