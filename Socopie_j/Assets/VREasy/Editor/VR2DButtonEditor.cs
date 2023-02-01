using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace VREasy
{
    [CustomEditor(typeof(VR2DButton))]
    [CanEditMultipleObjects]
    public class VR2DButtonEditor : Editor
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
            VR2DButton button_2d = (VR2DButton)target;
            Configure2DButton(ref button_2d, targets);
        }

        public static void displayGraphicalRepresentation(ref VR2DButton _vrButton, Object[] targets, bool idleRep = true, bool selectRep = true, bool activeRep = true)
        {
            // Graphical representation
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            float scale = EditorGUILayout.FloatField("Local scale", _vrButton._localScale);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Icons", EditorStyles.boldLabel);
            Sprite IdleIcon = null;
            Sprite SelectIcon = null;
            Sprite ActivateIcon = null;
            if (idleRep) IdleIcon = (Sprite)EditorGUILayout.ObjectField("Idle", _vrButton.IdleIcon, typeof(Sprite), true);
            if(selectRep) SelectIcon = (Sprite)EditorGUILayout.ObjectField("Select", _vrButton.SelectIcon, typeof(Sprite), true);
            if(activeRep) ActivateIcon = (Sprite)EditorGUILayout.ObjectField("Activate", _vrButton.ActivateIcon, typeof(Sprite), true);
            if (EditorGUI.EndChangeCheck())
            {
                foreach(VR2DButton button in targets)
                {
                    Undo.RecordObject(button, "Changed VRButton icons");
                    button.IdleIcon = IdleIcon;
                    button.ActivateIcon = ActivateIcon;
                    button.SelectIcon = SelectIcon;
                    button.SetScale(scale);
                }
                
            }
        }

        public static void displayTypeAndFaceDirection(ref VR2DButton _vrButton, Object[] targets)
        {
            // Type of button and face direction
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Type", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            bool showFacing = false;
            switch (_vrButton.type)
            {
                case VRBUTTON_REFRESH_TYPE.BILLBOARD:
                    EditorGUILayout.LabelField("Billboard buttons are always vertically aligned with camera");
                    showFacing = true;
                    break;
                case VRBUTTON_REFRESH_TYPE.NORMAL:
                    EditorGUILayout.LabelField("Normal buttons maintain their position and rotation");
                    break;

                case VRBUTTON_REFRESH_TYPE.STICKY:
                    EditorGUILayout.LabelField("Sticky buttons are attached to the HMD");
                    break;
            }
            VRBUTTON_REFRESH_TYPE type = (VRBUTTON_REFRESH_TYPE)EditorGUILayout.EnumPopup("", _vrButton.type);
            EditorGUILayout.EndHorizontal();
            VRELEMENT_FACE_DIRECTION faceDirection = _vrButton.faceDirection;
            if (showFacing) { 
                
                faceDirection = (VRELEMENT_FACE_DIRECTION)EditorGUILayout.EnumPopup("Face", _vrButton.faceDirection);
            }
            if (EditorGUI.EndChangeCheck()) { 
            
                foreach(VR2DButton button in targets)
                {
                    Undo.RecordObject(button, "Changed VRButton settings");
                    button.type = type;
                    button.faceDirection = faceDirection;
                }
                
            }
        }

        public static void Configure2DButton(ref VR2DButton _vrButton, Object[] targets)
        {
            displayGraphicalRepresentation(ref _vrButton,targets);
            EditorGUILayout.Separator();

            displayTypeAndFaceDirection(ref _vrButton,targets);
            EditorGUILayout.Separator();
            
            // Display common button properties
            VRSelectable selectable = _vrButton;
            VRSelectableEditor.DisplayCommon(ref selectable, targets);

            // text 
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Button text", EditorStyles.boldLabel);
            if (!_vrButton.Label)
            {
                // add text
                EditorGUILayout.LabelField("Text not found");
                Handles.BeginGUI();
                if (GUILayout.Button("Add text"))
                {
                    foreach(VR2DButton button in targets)
                    {
                        GameObject canvas = new GameObject("[vreasy]Canvas");
                        canvas.transform.parent = button.transform;
                        Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f);
                        canvas.transform.localScale = scale;
                        canvas.transform.localPosition = Vector3.zero;
                        canvas.transform.localRotation = Quaternion.identity;
                        canvas.AddComponent<Canvas>();
                        canvas.AddComponent<CanvasRenderer>();
                        GameObject t = new GameObject("[vreasy]Text");
                        t.transform.parent = canvas.transform;
                        t.transform.localScale = scale;
                        t.transform.localPosition = Vector3.zero;
                        t.transform.localRotation = Quaternion.identity;
                        button.Label = t.AddComponent<Text>();
                        button.Label.text = "Button";
                        button.Label.fontSize = 120;
                        button.Label.horizontalOverflow = HorizontalWrapMode.Overflow;
                        button.Label.verticalOverflow = VerticalWrapMode.Overflow;
                        button.Label.alignment = TextAnchor.LowerCenter;
                        button.Label.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                        button.Label.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                        button.Label.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                    }
                    
                }
            }
            else
            {
                // modify text properties
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Text found. To modify, access component");
                string text = EditorGUILayout.TextField("Text", _vrButton.Label.text);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach(VR2DButton button in targets)
                    {
                        Undo.RecordObject(button.Label, "Changed VRButton Label");
                        button.Label.text = text;
                    }
                    
                }
            }

        }
    }
}