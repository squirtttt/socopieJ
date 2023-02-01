using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VR3DButton))]
    [CanEditMultipleObjects]
    public class VR3DButtonEditor : Editor
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
            VR3DButton button_object = (VR3DButton)target;

            ConfigureObjectButton(ref button_object, targets);
        }

        public static void ConfigureObjectButton(ref VR3DButton _vrButton, Object[] targets)
        {
            bool useColourHighlights = EditorGUILayout.Toggle("Use colour highlights", _vrButton.useColourHighlights);
            foreach (VR3DButton button in targets) {
                button.useColourHighlights = useColourHighlights;
            }
            
            if (_vrButton.useColourHighlights)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Colours", EditorStyles.boldLabel);
                //Color IdleColour = EditorGUILayout.ColorField("Idle", _vrButton.IdleColour);
                Color SelectColour = EditorGUILayout.ColorField("Select", _vrButton.SelectColour);
                Color ActivateColour = EditorGUILayout.ColorField("Activate", _vrButton.ActivateColour);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach(VR3DButton button in targets)
                    {
                        Undo.RecordObject(button, "Changed VRButton colours");
                        //button.IdleColour = IdleColour;
                        button.ActivateColour = ActivateColour;
                        button.SelectColour = SelectColour;
                    }
                    
                }
            }
            /*Object meshObject = (Object)EditorGUILayout.ObjectField("Mesh or MeshFilter at root", _vrButton.TheMesh, typeof(Object), true);
            bool importError = false;
            if (meshObject != null)
            {
                if (meshObject as Mesh == null)
                {
                    if (meshObject as GameObject != null)
                    {
                        MeshFilter f = (meshObject as GameObject).GetComponent<MeshFilter>();
                        if (f != null)
                        {
                            _vrButton.TheMesh = f.sharedMesh;
                        }
                        else
                        {
                            // game object does not contain a meshfilter in root
                            importError = true;
                        }
                    }
                    else
                    {
                        // it's not a Mesh or a game object
                        importError = true;
                    }
                }
            }
            else
            {
                _vrButton.TheMesh = meshObject as Mesh;
            }
            if (importError)
            {
                EditorUtility.DisplayDialog("Error while importing mesh", "The object selected is not a mesh or does not have a MeshFilter at its root", "OK");
                _vrButton.TheMesh = null;
            }*/

            //_vrButton.TheMesh = (Mesh)EditorGUILayout.ObjectField("Mesh", _vrButton.TheMesh, typeof(Mesh), true);
            

            EditorGUILayout.Separator();

            VRSelectable selectable = _vrButton;
            VRSelectableEditor.DisplayCommon(ref selectable, targets);
        }
    }
}