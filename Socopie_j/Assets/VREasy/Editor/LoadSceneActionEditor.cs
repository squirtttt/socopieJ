using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

namespace VREasy
{
    [CustomEditor(typeof(LoadSceneAction))]
    public class LoadSceneActionEditor : Editor
    {
        private SceneAsset _scene;

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

            LoadSceneAction loadAction = (LoadSceneAction)target;

            EditorGUI.BeginChangeCheck();
            if(_scene == null && loadAction.SceneObject != null)
            {
                _scene = (SceneAsset)loadAction.SceneObject;
            }
            if(loadAction.SceneObject == null)
            {
                _scene = (SceneAsset)EditorGUILayout.ObjectField("Scene to load", _scene, typeof(SceneAsset), true);
            } else
            {
                _scene = (SceneAsset)EditorGUILayout.ObjectField("Scene to load", loadAction.SceneObject, typeof(SceneAsset), true);
            }
            
            EditorGUILayout.Separator();
            bool useCustomParameters = EditorGUILayout.Toggle("Custom parameters", loadAction.useCustomParameters);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed loaded scene");
                loadAction.useCustomParameters = useCustomParameters;
                loadAction.SceneObject = (Object)_scene;
            }
            if (loadAction.useCustomParameters)
            {
                EditorGUI.BeginChangeCheck();
                float timeToLoad = EditorGUILayout.DelayedFloatField("Delay", loadAction.timeToLoad);
                bool doFadeOut = EditorGUILayout.Toggle("Fade out", loadAction.doFadeOut);
                if(EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed load scene parameters");
                    loadAction.timeToLoad = timeToLoad;
                    loadAction.doFadeOut = doFadeOut;
                }
            }

            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            bool rememberLastPosition = EditorGUILayout.Toggle("Store scene position", loadAction.rememberLastLocation);
            bool hasFuturePos = EditorGUILayout.Toggle("Future position", loadAction.hasFuturePosition);
            Vector3 futurePos = Vector3.zero;
            if (hasFuturePos)
            {
                futurePos = EditorGUILayout.Vector3Field("Next position", loadAction.futurePosition);
            }
            Transform t = null;
            if (rememberLastPosition || hasFuturePos)
            {
                EditorGUILayout.LabelField("Only the X (horizontal) and Z (depth) positions are stored", EditorStyles.wordWrappedLabel);
                t = (Transform)EditorGUILayout.ObjectField("Player transform", loadAction.Player, typeof(Transform), true);

            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(loadAction, "Changed location settings");
                loadAction.Player = t;
                loadAction.rememberLastLocation = rememberLastPosition;
                loadAction.hasFuturePosition = hasFuturePos;
                loadAction.futurePosition = futurePos;
            }
            if (loadAction.rememberLastLocation || loadAction.hasFuturePosition)
            {
                EditorGUILayout.Separator();
                if (loadAction.Player == null)
                {
                    EditorGUILayout.HelpBox("A player object must be linked to remember and load last position when returning to the scene", MessageType.Warning);
                }
                else
                {
                    var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == loadAction.Player.name);
                    if (objects.Count() > 1)
                    {
                        EditorGUILayout.HelpBox("Player game object [" + loadAction.Player.name + "] must have a unique name to successfully locate after scene load", MessageType.Warning);
                    }
                }
            }
        }
    }
}