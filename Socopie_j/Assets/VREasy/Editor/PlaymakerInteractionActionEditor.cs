using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(PlaymakerInteractionAction))]
    public class PlaymakerInteractionActionEditor : Editor
    {
        bool handleRepaintErrors = false;
        public int selection = 0;

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


#if VREASY_PLAYMAKER_SDK
            PlaymakerInteractionAction playmaker = (PlaymakerInteractionAction)target;
            EditorGUI.BeginChangeCheck();
            PlayMakerFSM fsm = (PlayMakerFSM)EditorGUILayout.ObjectField("PlayMaker FSM", playmaker.playmakerFSM, typeof(PlayMakerFSM), true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(playmaker, "Playmaker FSM set");
                playmaker.playmakerFSM = fsm;
            }
            if (playmaker.playmakerFSM != null)
            {
                EditorGUILayout.Separator();
                if (playmaker.playmakerEvent != null)
                {
                    EditorGUILayout.LabelField("Current event: " + playmaker.playmakerEvent.Name);
                    EditorGUILayout.Separator();
                }
                else
                {
                    EditorGUILayout.LabelField("No event linked");
                }
                EditorGUILayout.LabelField("Set new event", EditorStyles.boldLabel);
                selection = EditorGUILayout.Popup("Playmaker event", selection, getEventsAsString(playmaker));
                if (GUILayout.Button("Set Event"))
                {
                    playmaker.playmakerEvent = playmaker.playmakerFSM.FsmEvents[selection];
                }
                if (GUILayout.Button("Clear event"))
                {
                    playmaker.playmakerEvent = null;
                }
            }
            else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("PlayMaker FSM not set", EditorStyles.boldLabel);
            }

#else
            EditorGUILayout.HelpBox("PlayMaker SDK is not active. Please go to VREasy/SDK Checker GUI to activate it after importing the package", MessageType.Error);
#endif




        }

#if VREASY_PLAYMAKER_SDK
        private string[] getEventsAsString(PlaymakerInteractionAction playmaker)
        {
            List<string> list = new List<string>();
            if (playmaker.playmakerFSM != null)
            {
                foreach (HutongGames.PlayMaker.FsmEvent e in playmaker.playmakerFSM.FsmEvents)
                {
                    list.Add(e.Name);
                }
            }
            return list.ToArray();
        }
#endif

    }
}