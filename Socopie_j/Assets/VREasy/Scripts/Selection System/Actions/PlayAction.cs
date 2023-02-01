using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public abstract class PlayAction : VRAction
    {
        public PLAY_ACTION playType = PLAY_ACTION.PLAY;

        public override void Trigger()
        {
            switch(playType)
            {
                case PLAY_ACTION.PLAY:
                    Play();
                    break;
                case PLAY_ACTION.STOP:
                    Stop();
                    break;
                case PLAY_ACTION.TOGGLE:
                    bool state = getToggleState();
                    if (state) Stop();
                    else Play();
                    state = !state;
                    break;
            }
        }

        protected abstract bool getToggleState();

        protected abstract void Play();

        protected abstract void Stop();
    }
}