using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(HighlightObjectAction))]
    public class HighlightObjectActionEditor : Editor
    {
        SerializedProperty flashSpeed;
        SerializedProperty numberOfFlashes;
        SerializedProperty highlightColour;
        SerializedProperty outlineThickness;
        SerializedProperty stayHighlighted;

        private void OnEnable()
        {
            flashSpeed = serializedObject.FindProperty("flashSpeed");
            numberOfFlashes = serializedObject.FindProperty("numberOfFlashes");
            highlightColour = serializedObject.FindProperty("highlightColour");
            outlineThickness = serializedObject.FindProperty("outlineThickness");
            stayHighlighted = serializedObject.FindProperty("stayHighlighted");
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
            HighlightObjectAction highlighter = (HighlightObjectAction)target;

            serializedObject.Update();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(flashSpeed);
            EditorGUILayout.PropertyField(numberOfFlashes);
            EditorGUILayout.PropertyField(highlightColour);
            EditorGUILayout.PropertyField(outlineThickness);
            EditorGUILayout.PropertyField(stayHighlighted);
            
            serializedObject.ApplyModifiedProperties();

            // todo: cannot show the array in the inspector as a serialized object
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Target renderers",EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            int removeIndex = -1;
            bool addSlot = false;
            for (int ii = 0; ii < highlighter.targetRenderers.Count; ii++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("" + (ii + 1));
                highlighter.targetRenderers[ii] = (MeshRenderer)EditorGUILayout.ObjectField(highlighter.targetRenderers[ii], typeof(MeshRenderer), true);
                Handles.BeginGUI();
                if (GUILayout.Button("-"))
                {
                    removeIndex = ii;
                }
                Handles.EndGUI();
                EditorGUILayout.EndHorizontal();
            }
            // add actions
            Handles.BeginGUI();
            if (GUILayout.Button("Add renderer"))
            {
                addSlot = true;
            }
            Handles.EndGUI();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(highlighter, "Changed target renderers");
                if (removeIndex >= 0)
                {
                    highlighter.targetRenderers.RemoveAt(removeIndex);
                    EditorGUIUtility.ExitGUI();
                }
                if (addSlot)
                {
                    highlighter.targetRenderers.Add(null);
                    EditorGUIUtility.ExitGUI();
                }
            }

            // preview highlight material
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Preview highlight on all targets",EditorStyles.wordWrappedLabel);
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Reset material"))
            {
                highlighter.doFlashing(false);
            }
            if (GUILayout.Button("Preview material"))
            {
                highlighter.clearAll();
                highlighter.configureShader();
                highlighter.doFlashing(true);
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}