using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class PlaymakerInteractionAction : VRAction
    {
#if VREASY_PLAYMAKER_SDK
        public PlayMakerFSM playmakerFSM;
        public HutongGames.PlayMaker.FsmEvent playmakerEvent;
        public string playmakerEventString;
#endif

        public override void Trigger()
        {
#if VREASY_PLAYMAKER_SDK
            if(playmakerFSM == null || playmakerEvent == null)
            {
                Debug.Log("[VREasy] PlaymakerInteractionAction error, playmaker FSM or Event not set.");
                return;
            }
            playmakerFSM.Fsm.Event(playmakerEvent.Name);
#else
            Debug.Log("[VREasy] PlaymakerInteractionAction: PlayMaker SDK is required but has not been activated. Please go to the SDK Checker GUI to activate it after importing the package.");
#endif
        }
    }
}