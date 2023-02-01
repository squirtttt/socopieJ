using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace VREasy
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(CombinationTrigger))]
    public class CombinationTriggerEditor : Editor
    {
        private static int grabIndex = 0;
        private static List<string> assemblyNames = new List<string>();
        private static List<string> names = new List<string>();

        // Use this for initialization
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

            CombinationTrigger combinationTrigger = (CombinationTrigger)target;
            
            ConfigureCombinationTrigger(combinationTrigger);

            if (GUI.changed)
            {
                // mark scene dirty to pick up the changes
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

        }


        public static void ConfigureCombinationTrigger(CombinationTrigger combinationTrigger)
        {
            EditorGUILayout.HelpBox("Add several triggers to create a single, combined trigger condition", MessageType.Info);
            EditorGUILayout.LabelField("Add a new Trigger", EditorStyles.boldLabel);
            combinationTrigger.empty = null;
            GameObject obj = combinationTrigger.gameObject;
            //VRGrabTrigger.DisplayGrabTriggerSelector
            ReducedTriggerSelector(ref combinationTrigger.empty, ref obj);

            // Set your Operator
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Boolean Operator", EditorStyles.boldLabel);


            int index = (int)combinationTrigger.booleanOperator;
            string[] options = new string[] { "AND", "OR"};

            EditorGUILayout.BeginHorizontal();
            index = EditorGUILayout.Popup("Boolean Operator:",  index, options, EditorStyles.popup);
            combinationTrigger.booleanOperator = (BooleanOperator)index;

            EditorGUILayout.EndHorizontal();
         

            // Currently Set Triggers

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Current Trigger List", EditorStyles.boldLabel);

            VRGrabTrigger[] triggerList = obj.GetComponents<VRGrabTrigger>();
            List<VRGrabTrigger> cleanTriggerList = triggerList.ToList();
            List<int> removeIndexs = new List<int>();

            for (int i = triggerList.Count() - 1; i >= 0; i--)
            {
                VRGrabTrigger trigger = triggerList[i];
                if (trigger.GetType() == typeof(CombinationTrigger))
                {
                    removeIndexs.Add(i);
                }
            }


            for (int i = removeIndexs.Count() - 1; i >= 0; i--)
            {
                cleanTriggerList.RemoveAt(removeIndexs[i]);
            }

            combinationTrigger.TriggerList = cleanTriggerList.ToArray();

            if (combinationTrigger.TriggerList.Count() < 2)
            {
                EditorGUILayout.HelpBox("Please add at least two triggers to use effectively.", MessageType.Warning);
            }

            for (int i = 0; i < combinationTrigger.TriggerList.Length; i++) { 
                VRGrabTrigger trigger = combinationTrigger.TriggerList[i];
                ReducedTriggerSelector(ref trigger, ref obj);
            }

        }



        public static void ReducedTriggerSelector(ref VRGrabTrigger grabTrigger, ref GameObject obj)
        {
            if (grabTrigger == null)
            {
                // add grab trigger
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                VREasy_utils.LoadClassesFromAssembly(typeof(VRGrabTrigger), ref assemblyNames, ref names);

                // Remove CombinationTrigger from list to stop recursive Combination Triggers
                for (int i = assemblyNames.Count() - 1; i >= 0; i--)
                {
                    if (names[i] == "CombinationTrigger")
                    {
                        names.RemoveAt(i);
                        assemblyNames.RemoveAt(i);
                    }

                }

                grabIndex = EditorGUILayout.Popup("Add", grabIndex, names.ToArray());

                Handles.BeginGUI();
                VRGrabTrigger trigger = null;
                bool linkTrigger = (GUILayout.Button("Link trigger"));
                Handles.EndGUI();
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(obj, "Linked trigger");
                    if (linkTrigger)
                    {
                        trigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(obj, assemblyNames[grabIndex]);
                        grabTrigger = trigger;
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                grabTrigger = (VRGrabTrigger)EditorGUILayout.ObjectField("Associated trigger", grabTrigger, typeof(VRGrabTrigger), true);
                Handles.BeginGUI();
                bool destroy = GUILayout.Button("Remove trigger");
                Handles.EndGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(grabTrigger, "Removed trigger");
                    if (destroy)
                    {
                        DestroyImmediate(grabTrigger);
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }
    }
}
