using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace VREasy
{
    [CustomEditor(typeof(SendMessageAction))]
    public class SendMessageActionEditor : Editor
    {

        private List<Component> components_list = new List<Component>();
        private List<string> componentNames_list = new List<string>();
        private List<MethodInfo> methods_list = new List<MethodInfo>();
        private List<string> methodNames_list = new List<string>();
        private List<Type> parameters_list = new List<Type>();
        
        private int componentIndex = 0;
        private int methodIndex = 0;
        private GameObject messageReceiver;

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
            EditorGUILayout.HelpBox("SendMessage is deprecated and the new UnityEventAction should be used instead (with support for multiple parameters", MessageType.Warning);

            SendMessageAction sendAction = (SendMessageAction)target;
            EditorGUILayout.Separator();
            if (sendAction.messageReceiver == null || string.IsNullOrEmpty(sendAction.messageName))
            {

                EditorGUILayout.HelpBox("Receiver not set, please choose an event receiver and event name", MessageType.Warning);
            } else
            {
                EditorGUILayout.LabelField("Current active message", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Current active message Receiver: " + sendAction.messageReceiver, EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("Current active message name: " + sendAction.messageName, EditorStyles.wordWrappedLabel);
                //EditorGUILayout.LabelField("Current active event parameter: " + sendAction.parameter, EditorStyles.wordWrappedLabel);
            }
            
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Change your active message", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            GameObject receiver = (GameObject)EditorGUILayout.ObjectField("Receiver", messageReceiver, typeof(GameObject), true);
            bool reloadComponents = false;
            
            if (messageReceiver != receiver)
            {
                reloadComponents = true;
                messageReceiver = receiver;
            }
            if (messageReceiver == null)
            {
                clearAll();
                return;
            }

            if (reloadComponents)
            {
                clearAll();
                VREasy_utils.LoadComponents(messageReceiver,ref components_list, ref componentNames_list);
            }

            int ci = EditorGUILayout.Popup("Component", componentIndex >= 0 ? componentIndex : 0,componentNames_list.ToArray());
            if(ci != componentIndex)
            {
                componentIndex = ci;
                methodIndex = -1;
                clearMethods();
                if (components_list.Count > 0)
                    VREasy_utils.LoadMethods(components_list[componentIndex].GetType(),ref methods_list, ref methodNames_list);
            }

            int mi = EditorGUILayout.Popup("Method", methodIndex >= 0 ? methodIndex : 0, methodNames_list.ToArray());
            if(mi != methodIndex)
            {
                methodIndex = mi;
                clearParameters();
                if (methods_list.Count > 0)
                    VREasy_utils.LoadParameters(methods_list[methodIndex],ref parameters_list);
            }
            if(parameters_list.Count > 0)
            {
                EditorGUILayout.HelpBox("Current version only supports calling functions without parameters. Use UnityEventAction instead", MessageType.Warning);
            } else
            {
                /*UnityEngine.Object obj = null;
                Type type = null;
                foreach (Type p in parameters_list)
                {
                    obj = (UnityEngine.Object)EditorGUILayout.ObjectField("Parameter:", sendAction.parameter, p, true);
                    type = p;
                }*/
                Handles.BeginGUI();
                if (GUILayout.Button("Set event"))
                {
                    sendAction.messageReceiver = messageReceiver;
                    sendAction.messageName = methodNames_list[methodIndex];
                    //sendAction.parameter = obj;
                    //sendAction.parameterType = type;
                    clearAll();
                    messageReceiver = null;
                }
                Handles.EndGUI();
            }
            
        }

        private void clearAll()
        {
            componentIndex = -1;
            components_list.Clear();
            componentNames_list.Clear();
            methods_list.Clear();
            methodNames_list.Clear();
            parameters_list.Clear();
        }

        private void clearMethods()
        {
            methods_list.Clear();
            methodNames_list.Clear();
            clearParameters();
        }

        private void clearParameters()
        {
            parameters_list.Clear();
        }

        
    }
}