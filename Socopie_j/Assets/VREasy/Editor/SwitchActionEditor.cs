using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;

namespace VREasy
{
    [CustomEditor(typeof(SwitchAction))]
    public class SwitchActionEditor : Editor
    {

        private List<string> properties = new List<string>();
        private int propertyIndex = 0;

        private GameObject targetObject = null;

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

            SwitchAction switchAction = (SwitchAction)target;
            SWITCH_TYPE type = (SWITCH_TYPE)EditorGUILayout.EnumPopup("Switch type", switchAction.Type);
            EditorGUILayout.Separator();

            switchAction.Type = type;

            switch (switchAction.Type)
            {
                case SWITCH_TYPE.MATERIAL:
                    showSwitch<Renderer,Material>(switchAction);
                    break;
                case SWITCH_TYPE.MESH:
                    showSwitch<MeshFilter, Mesh>(switchAction);
                    break;
                case SWITCH_TYPE.TEXTURE:
                    showSwitch<Material, Texture2D>(switchAction);
                    break;
                case SWITCH_TYPE.SPRITE:
                    showSwitch<SpriteRenderer, Sprite>(switchAction);
                    break;
                case SWITCH_TYPE.SOUND:
                    {
                        showSwitch<AudioSource, AudioClip>(switchAction);
                        if (targetObject != null)
                        {
                            if (!targetObject.GetComponent<AutoAudioClipPlay>())
                            {
                                if (GUILayout.Button("Add auto play"))
                                {
                                    targetObject.AddComponent<AutoAudioClipPlay>();
                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Auto play enabled", EditorStyles.wordWrappedLabel);
                            }
                        }
                    }
                    break;
                case SWITCH_TYPE.CUSTOM:
                    showSwitch<Object, Object>(switchAction);
                    findProperties(switchAction);
                    if (properties.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        propertyIndex = EditorGUILayout.Popup("Property", propertyIndex, properties.ToArray());
                        Handles.BeginGUI();
                        EditorGUI.BeginChangeCheck();
                        string propertyName = "";
                        if (GUILayout.Button("Select"))
                        {
                            propertyName = properties[propertyIndex];
                            if(EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(target, "Changed custom property");
                                switchAction.propertyName = propertyName;
                            }
                        }
                        Handles.EndGUI();
                        EditorGUILayout.EndHorizontal();
                        if (!string.IsNullOrEmpty(switchAction.propertyName))
                        {
                            EditorGUILayout.LabelField("Property selected: " + switchAction.propertyName, EditorStyles.boldLabel);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Property not selected", MessageType.Error);
                        }
                    }
                    
                    break;
            }

            // store options
            EditorGUI.BeginChangeCheck();
            bool store = switchAction.storeOption;
            string optionName = switchAction.optionName;
#if NETFX_CORE || UNITY_WSA_10_0
            store = false;
            EditorGUILayout.HelpBox("Cannot store options in a file when using Universal Windows Platform",MessageType.Warning);
#else
            store = EditorGUILayout.Toggle("Store options", switchAction.storeOption);
            if (store)
            {
                optionName = EditorGUILayout.TextField("Option name", switchAction.optionName);
            }
            
#endif
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(switchAction, "store options");
                switchAction.storeOption = store;
                switchAction.optionName = optionName;
            }


        }

        private void showSwitch<T,Q>(SwitchAction switchAction)
        {
            EditorGUI.BeginChangeCheck();
            Object target = (UnityEngine.Object)EditorGUILayout.ObjectField("Target", switchAction.Target, typeof(T), true);
            try
            {
                if (target != null) targetObject = (GameObject)target.GetType().GetProperty("gameObject").GetValue(target, null);
                else targetObject = null;
#pragma warning disable 0168
            } catch(System.Exception e) {
                targetObject = null;
            }
#pragma warning restore 0168

            int removeIndex = -1;
            bool addSlot = false;
            if (switchAction.swapObjects.Count == 0)
            {
                EditorGUILayout.HelpBox("No swappable objects added", MessageType.Error);
            } else
            {
                EditorGUILayout.LabelField("Swaps");
            }
            for (int ii = 0; ii < switchAction.swapObjects.Count; ii++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(""+ (ii + 1));
                switchAction.swapObjects[ii] = (Object)EditorGUILayout.ObjectField(switchAction.swapObjects[ii], typeof(Q), true);
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
            if (GUILayout.Button("Add slot"))
            {
                addSlot = true;
            }
            Handles.EndGUI();
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(switchAction, "Changed switch objects");
                switchAction.Target = target;
                if (removeIndex >= 0)
                {
                    switchAction.swapObjects.RemoveAt(removeIndex);
                    EditorGUIUtility.ExitGUI();
                }
                if (addSlot)
                {
                    switchAction.swapObjects.Add(null);
                    EditorGUIUtility.ExitGUI();
                }
            }
        }

        private void findProperties(SwitchAction switchAction)
        {
            if (switchAction.Target == null)
            {
                properties.Clear();
                return;
            }
            properties.Clear();
            PropertyInfo[] props = switchAction.Target.GetType().GetProperties();
            foreach(PropertyInfo p in props)
            {
                properties.Add(p.Name);
            }
        }
        
    }
}