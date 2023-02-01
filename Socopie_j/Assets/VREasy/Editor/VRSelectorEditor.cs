using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace VREasy
{
    [CustomEditor(typeof(VRSelector))]
    public class VRSelectorEditor : Editor
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
            VRSelector selector = (VRSelector)target;

            ConfigureSelector(ref selector);
        }

        public static void ConfigureSelector(ref VRSelector selector)
        {
            EditorGUI.BeginChangeCheck();
            float activationTime = EditorGUILayout.FloatField("Activation time", selector.activationTime);
            //bool useTooltip = EditorGUILayout.Toggle("Use tooltip", selector.hasTooltip);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selector, "Selector settings");
                selector.activationTime = activationTime;
                //selector.hasTooltip = useTooltip;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Tooltip", EditorStyles.boldLabel);
            if(selector.canvasObject == null) selector.canvasObject = GameObject.Find(VRSelector.TOOLTIP_CANVAS_NAME);
            if(selector.canvasObject != null)
            {
                selector.Tooltip = selector.canvasObject.GetComponentInChildren<Text>();
            }
            if (!selector.Tooltip)
            {
                // add text
                EditorGUILayout.LabelField("Tooltip not configured");
                Handles.BeginGUI();
                if (GUILayout.Button("Add tooltip"))
                {
                    // add text
                    selector.canvasObject = new GameObject(VRSelector.TOOLTIP_CANVAS_NAME);
                    Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f);
                    selector.canvasObject.transform.localScale = scale;
                    selector.canvasObject.AddComponent<Canvas>();
                    selector.canvasObject.gameObject.AddComponent<CanvasRenderer>();
                    GameObject t = new GameObject("[vreasy]Text");
                    t.transform.parent = selector.canvasObject.transform;
                    t.transform.localScale = scale / 2f;
                    t.transform.localPosition = Vector3.zero;
                    t.transform.localRotation = Quaternion.identity;
                    selector.Tooltip = t.AddComponent<Text>();
                    selector.Tooltip.horizontalOverflow = HorizontalWrapMode.Overflow;
                    selector.Tooltip.verticalOverflow = VerticalWrapMode.Overflow;
                    selector.Tooltip.alignment = TextAnchor.UpperCenter;
                    //selector.Tooltip.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    //selector.Tooltip.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                    //selector.Tooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                }
            }
            else
            {
                // modify text properties
                EditorGUI.BeginChangeCheck();
                // show tooltip Text options
                Font font = (Font)EditorGUILayout.ObjectField("Font", selector.tooltipFont, typeof(Font), true);
                int size = EditorGUILayout.IntField("Size", selector.tooltipSize);
                Color colour = EditorGUILayout.ColorField("Colour", selector.tooltipColour);
                float dist = EditorGUILayout.Slider("Eye to object pos", selector.tooltipDistance, 0.1f, 0.9f);
                //FontStyle style = (FontStyle)EditorGUILayout.EnumPopup("Tooltip style",)
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(selector, "tooltip");
                    selector.tooltipFont = font;
                    selector.tooltipSize = size;
                    selector.tooltipColour = colour;
                    selector.tooltipDistance = dist;
                }
                EditorGUILayout.Separator();
            }

        }
    }
}