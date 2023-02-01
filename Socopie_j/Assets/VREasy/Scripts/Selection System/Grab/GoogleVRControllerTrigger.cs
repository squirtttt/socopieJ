using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREasy
{
    public class GoogleVRControllerTrigger : VRGrabTrigger
    {
        //public GOOGLEVR_CONTROLLER_INPUT input = GOOGLEVR_CONTROLLER_INPUT.IS_TOUCHING;
#if VREASY_GOOGLEVR_SDK
        public GvrControllerButton button = GvrControllerButton.App;
        public GvrControllerHand device = GvrControllerHand.Dominant;
#endif

        private void Start()
        {
#if VREASY_GOOGLEVR_SDK
            if (FindObjectOfType<GvrControllerInput>() == null)
            {
                Debug.LogWarning("[VREasy] GvrControllerInput instance not found, adding one for GoogleVRControllerTrigger");
                GameObject n = new GameObject("[VREasy] GvrControllerInput");
                n.AddComponent<GvrControllerInput>();
            }
#endif
        }
        public override bool Triggered()
        {
#if VREASY_GOOGLEVR_SDK
            return GvrControllerInput.GetDevice(device).GetButton(button);

            /*switch(input)
            {
                case GOOGLEVR_CONTROLLER_INPUT.IS_TOUCHING:
                    return GvrControllerInput.IsTouching;
                case GOOGLEVR_CONTROLLER_INPUT.TOUCH_DOWN:
                    return GvrControllerInput.TouchDown;
                case GOOGLEVR_CONTROLLER_INPUT.TOUCH_UP:
                    return GvrControllerInput.TouchUp;
                case GOOGLEVR_CONTROLLER_INPUT.APP_BUTTON:
                    return GvrControllerInput.AppButton;
                case GOOGLEVR_CONTROLLER_INPUT.HOME_BUTTON_STATE:
                    return GvrControllerInput.HomeButtonState;
                case GOOGLEVR_CONTROLLER_INPUT.HOME_BUTTON_DOWN:
                    return GvrControllerInput.HomeButtonDown;
                case GOOGLEVR_CONTROLLER_INPUT.CLICK_BUTTON:
                    return GvrControllerInput.ClickButton;
                case GOOGLEVR_CONTROLLER_INPUT.CLICK_BUTTON_DOWN:
                    return GvrControllerInput.ClickButtonDown;
                case GOOGLEVR_CONTROLLER_INPUT.CLICK_BUTTON_UP:
                    return GvrControllerInput.ClickButtonUp;
                default:
                    return false;
            }*/    

#else
            return false;
#endif
        }

    }
}