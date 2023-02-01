using UnityEngine;
using System.Collections;
using System;
#if VREASY_STEAM_SDK
using Valve.VR;
using Valve.VR.InteractionSystem;
#endif

namespace VREasy
{
    public class SteamControllerGrab : VRGrabTrigger
    {
#if VREASY_STEAM_SDK
        /*public Valve.VR.EVRButtonId button = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
        public STEAM_VR_CONTROLLER_SIDE controllerSide = STEAM_VR_CONTROLLER_SIDE.RIGHT;
        public STEAM_VR_CONTROLLER_INPUT_TYPE type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;

        SteamVR_Controller.Device rightDevice;
        SteamVR_Controller.Device leftDevice;*/

        public SteamVR_Action_Boolean boolAction;
        public SteamVR_Input_Sources inputSource;

        private bool state = false;

        private void OnEnable()
        {
            SteamVR.Initialize();

            boolAction.AddOnChangeListener(OnActionChange, inputSource);
        }

        private void OnDisable()
        {
            boolAction.RemoveOnChangeListener(OnActionChange, inputSource);
        }

        //private void OnActionChange(SteamVR_Action_In actionIn)
        private void OnActionChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
        {
            //state = boolAction.GetStateDown(inputSource);
            state = newValue;
        }

#endif
        public override bool Triggered()
        {
#if VREASY_STEAM_SDK
            
            return state;
#else
            return false;
#endif
        }
    }
}