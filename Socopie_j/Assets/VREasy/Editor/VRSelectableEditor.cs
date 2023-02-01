using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

namespace VREasy
{
    [CustomEditor(typeof(VRSelectable))]
    public class VRSelectableEditor : Editor
    {
        private static int actionIndex = 0;
        private static Editor _editor;
        private static List<string> actions_assemblyNames = new List<string>();
        private static List<string> actions_names = new List<string>();

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
            VRSelectable _vrButton = (VRSelectable)target;

            DisplayCommon(ref _vrButton, targets);
        }

        public static void DisplayCommon(ref VRSelectable _vrButton, Object[] targets)
        {
            DisplayStateOptions(_vrButton, targets);

            DisplayTooltip(_vrButton, targets);

            DisplayTimingOptions(_vrButton, targets);

            DisplayActionList(ref _vrButton, targets);

            DisplayAudioOptions(_vrButton, targets);
        }

        public static void DisplayTooltip(VRSelectable selectable, Object[] targets)
        {
            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Tooltip options", EditorStyles.boldLabel);
            string tooltip = EditorGUILayout.TextField("Tooltip", selectable.tooltip);
            EditorGUILayout.LabelField("(optional) Using anchored tooltip allows you to display the tooltip panel in a custom position (and not overlayed)", EditorStyles.wordWrappedLabel);
            GameObject anchoredTooltip = (GameObject)EditorGUILayout.ObjectField("Anchored tooltip object", selectable.anchoredTooltipObject, typeof(GameObject), true);
            if (anchoredTooltip != null && selectable.anchoredTooltipText == null)
            {
                foreach (VRSelectable sel in targets)
                    sel.anchoredTooltipText = anchoredTooltip.GetComponentInChildren<Text>();
            }
            if (anchoredTooltip != null && selectable.anchoredTooltipText == null)
            {
                EditorGUILayout.HelpBox("The anchored tooltip (or any of its children) does not seem to have a Text component. Attach one or link one to the anchored text property", MessageType.Error);
            }
            Text anchoredText = null;
            if (anchoredTooltip != null)
            {
                anchoredText = (Text)EditorGUILayout.ObjectField("Anchored text", selectable.anchoredTooltipText, typeof(Text), true);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach(VRSelectable sel in targets)
                {
                    Undo.RecordObject(sel, "tooltip changed");
                    sel.tooltip = tooltip;
                    sel.anchoredTooltipObject = anchoredTooltip;
                    sel.anchoredTooltipText = anchoredText;
                }
            }
        }

        public static void DisplayStateOptions(VRSelectable _vrButton, Object[] targets)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("VRElement state", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            bool state = EditorGUILayout.Toggle("Start state", _vrButton.active);
            EditorGUILayout.LabelField("Toggle timings (0 = no toggle)");
            EditorGUI.indentLevel++;
            float init_transition = EditorGUILayout.FloatField("After start", _vrButton.init_transition);
            float activation_transition = EditorGUILayout.FloatField("After activate", _vrButton.activation_transition);
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                foreach(VRSelectable sel in targets)
                {
                    Undo.RecordObject(sel, "state changed");
                    sel.active = state;
                    sel.init_transition = init_transition;
                    sel.activation_transition = activation_transition;
                }
                
            }
        }

        public static void DisplayTimingOptions(VRSelectable _vrButton, Object[] targets)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Timers", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            float coolDownTime = EditorGUILayout.FloatField("Cooldown time", _vrButton.coolDownTime);
            float deactivateTime = EditorGUILayout.FloatField("Deactivation time", _vrButton.deactivationTime);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(VRSelectable sel in targets)
                {
                    Undo.RecordObject(sel, "timers");
                    sel.coolDownTime = coolDownTime;
                    sel.deactivationTime = deactivateTime;
                }
                
            }
        }

        public static void DisplayAudioOptions(VRSelectable _vrButton, Object[] targets)
        {
            // audio
            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
            AudioClip selectSound = (AudioClip)EditorGUILayout.ObjectField("Selection sound", _vrButton.selectSound, typeof(AudioClip), true);
            AudioClip activateSound = (AudioClip)EditorGUILayout.ObjectField("Activation sound", _vrButton.activateSound, typeof(AudioClip), true);
            if (EditorGUI.EndChangeCheck())
            {
                foreach(VRSelectable sel in targets)
                {
                    Undo.RecordObject(sel, "Changed VRButton sounds");
                    sel.selectSound = selectSound;
                    sel.activateSound = activateSound;
                }
                
            }
        }

        public static void DisplayActionList(ref VRSelectable _vrButton, Object[] targets)
        {
            // action triggered
            DisplayActionList(_vrButton.actionList,targets);
            
        }

        public static void DisplayActionList(ActionList actions, Object[] targets)
        {
            if (targets.Length > 1)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("Multiobject selection is not supported to add or remove actions", MessageType.Warning);
            }
            else
            {
                // action triggered
                DisplayActionList(actions);
            }
        }

        private static void DisplayActionList(ActionList actions)
        {
            // action triggered
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Actions to trigger", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            int removeActionIndex = -1;
            VRAction addActionSpecific = null;
            //bool addBlankAction = false;
            for (int ii = 0; ii < actions.list.Count; ii++)
            {
                if (actions.list[ii] == null) continue;
                actions.list[ii].delay = EditorGUILayout.FloatField(ii + " - Delayed (sec)", actions.list[ii].delay);
                EditorGUILayout.BeginHorizontal();
                actions.list[ii] = (VRAction)EditorGUILayout.ObjectField(ii + " - Action", actions.list[ii], typeof(VRAction), true);
                Handles.BeginGUI();
                if (GUILayout.Button("-"))
                {
                    removeActionIndex = ii;
                }
                Handles.EndGUI();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
            // add actions
            EditorGUILayout.BeginHorizontal();
            VREasy_utils.LoadClassesFromAssembly(typeof(VRAction), ref actions_assemblyNames, ref actions_names, "Action");

            actionIndex = EditorGUILayout.Popup("Action type", actionIndex, actions_names.ToArray());

            Handles.BeginGUI();
            if (GUILayout.Button("Add action"))
            {
                addActionSpecific = VREasy_utils.LoadAndSetClassFromAssembly<VRAction>(actions.gameObject, actions_assemblyNames[actionIndex]); //VRAction.getAction(actions.gameObject, actionIndex);
            }
            Handles.EndGUI();
            EditorGUILayout.EndHorizontal();
            // add unspecified action
            /*Handles.BeginGUI();
            if (GUILayout.Button("+"))
            {
                addBlankAction = true;
            }
            Handles.EndGUI();*/

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(actions, "Changed multiple action properties");
                if (removeActionIndex >= 0)
                {
                    VRAction reference = actions.list[removeActionIndex];
                    actions.list.RemoveAt(removeActionIndex);
                    DestroyImmediate(reference);
                    EditorGUIUtility.ExitGUI();
                }
                if (addActionSpecific)
                    actions.list.Add(addActionSpecific);
                //if (addBlankAction)
                //    actions.list.Add(null);
            }

            EditorGUILayout.Separator();
            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Actions are placed in gameobject: " + actions.gameObject.name);
            GUI.color = Color.white;
            Handles.BeginGUI();
            if (GUILayout.Button("Locate"))
            {
                Selection.activeObject = actions.gameObject;
                EditorGUIUtility.PingObject(actions.gameObject);
            }
            Handles.EndGUI();
            EditorGUILayout.EndHorizontal();

        }

    }
}