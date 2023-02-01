using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VREasy
{

    public class GenericControllerTrigger : VRGrabTrigger
    {
        public GENERIC_VR_BUTTON vr_button;
        public GENERIC_CONTROLLER_TYPE type = GENERIC_CONTROLLER_TYPE.STEAM_VR;
        private KeyCode key;

        private void Start()
        {
            int leftControllerId = -1;
            int rightControllerId = -1;

            for(int ii = 0; ii < Input.GetJoystickNames().Length;ii++)
            {
                switch (type)
                {
                    case GENERIC_CONTROLLER_TYPE.STEAM_VR:
                        {
                            if ("OpenVR Controller - Right".Equals(Input.GetJoystickNames()[ii]))
                                rightControllerId = ii;
                            if ("OpenVR Controller - Left".Equals(Input.GetJoystickNames()[ii]))
                                leftControllerId = ii;
                        }
                        break;
                    case GENERIC_CONTROLLER_TYPE.OCULUS_TOUCH:
                    case GENERIC_CONTROLLER_TYPE.OCULUS_REMOTE:
                        {
                            if ("Oculus Touch Controller - Right".Equals(Input.GetJoystickNames()[ii]))
                                rightControllerId = ii;
                            if ("Oculus Touch Controller - Left".Equals(Input.GetJoystickNames()[ii]))
                                leftControllerId = ii;
                        }
                        break;
                    case GENERIC_CONTROLLER_TYPE.WINDOWS_MIXED_REALITY:
                        {
                            if ("Spatial Controller - Right".Equals(Input.GetJoystickNames()[ii]))
                                rightControllerId = ii;
                            if ("Spatial Controller - Left".Equals(Input.GetJoystickNames()[ii]))
                                leftControllerId = ii;
                        }
                        break;
                    case GENERIC_CONTROLLER_TYPE.DAYDREAM:
                        {
                            if ("Daydream Controller - Right".Equals(Input.GetJoystickNames()[ii]))
                                rightControllerId = ii;
                            if ("Daydream Controller - Left".Equals(Input.GetJoystickNames()[ii]))
                                leftControllerId = ii;
                        }
                        break;
                }
            }

            // translate generic vr button enum to keycode
            // based on https://docs.unity3d.com/Manual/OpenVRControllers.html
            switch (vr_button)
            {
                case GENERIC_VR_BUTTON.STEAMVR_RIGHT_CONTROLLER_MENU:
                    key = getKeyCode(rightControllerId, 0);
                    break;
                case GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRACKPAD_PRESS:
                    key = getKeyCode(rightControllerId, 9);
                    break;
                case GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRACKPAD_TOUCH:
                    key = getKeyCode(rightControllerId, 17);
                    break;
                case GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRIGGER_TOUCH:
                    key = getKeyCode(rightControllerId, 15);
                    break;
                case GENERIC_VR_BUTTON.STEAMVR_LEFT_CONTROLLER_MENU:
                    key = getKeyCode(leftControllerId, 2);
                    break;
                case GENERIC_VR_BUTTON.STEAMVR_LEFT_TRACKPAD_PRESS:
                    key = getKeyCode(leftControllerId, 8);
                    break;
                case GENERIC_VR_BUTTON.STEAMVR_LEFT_TRACKPAD_TOUCH:
                    key = getKeyCode(leftControllerId, 16);
                    break;
                case GENERIC_VR_BUTTON.STEAMVR_LEFT_TRIGGER_TOUCH:
                    key = getKeyCode(leftControllerId, 14);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_ONE_PRESS:
                    key = getKeyCode(rightControllerId, 0);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_ONE_TOUCH:
                    key = getKeyCode(rightControllerId, 10);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_THREE_PRESS:
                    key = getKeyCode(leftControllerId, 2);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_THREE_TOUCH:
                    key = getKeyCode(leftControllerId, 12);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMB_STICK_PRESS:
                    key = getKeyCode(leftControllerId, 8);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMB_STICK_PRESS:
                    key = getKeyCode(rightControllerId, 9);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMB_STICK_TOUCH:
                    key = getKeyCode(leftControllerId, 16);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMB_STICK_TOUCH:
                    key = getKeyCode(rightControllerId, 17);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_PRIMARY_INDEX_TRIGGER:
                    key = getKeyCode(leftControllerId, 14);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_SECONDARY_INDEX_TRIGGER:
                    key = getKeyCode(rightControllerId, 15);
                    break;
                case GENERIC_VR_BUTTON.WMR_LEFT_TOUCHPAD_TOUCH:
                    key = getKeyCode(rightControllerId, 18);
                    break;
                case GENERIC_VR_BUTTON.WMR_RIGHT_TOUCHPAD_TOUCH:
                    key = getKeyCode(rightControllerId, 19);
                    break;
                case GENERIC_VR_BUTTON.WMR_LEFT_TOUCHPAD_PRESS:
                    key = getKeyCode(rightControllerId, 16);
                    break;
                case GENERIC_VR_BUTTON.WMR_RIGHT_TOUCHPAD_PRESS:
                    key = getKeyCode(rightControllerId, 17);
                    break;
                case GENERIC_VR_BUTTON.WMR_LEFT_THUMBSTICK_PRESS:
                    key = getKeyCode(rightControllerId, 8);
                    break;
                case GENERIC_VR_BUTTON.WMR_RIGHT_THUMBSTICK_PRESS:
                    key = getKeyCode(rightControllerId, 9);
                    break;
                case GENERIC_VR_BUTTON.WMR_LEFT_SELECT_TRIGGER_PRESS:
                    key = getKeyCode(rightControllerId, 14);
                    break;
                case GENERIC_VR_BUTTON.WMR_RIGHT_SELECT_TRIGGER_PRESS:
                    key = getKeyCode(rightControllerId, 15);
                    break;
                case GENERIC_VR_BUTTON.WMR_LEFT_GRIP_PRESS:
                    key = getKeyCode(rightControllerId, 4);
                    break;
                case GENERIC_VR_BUTTON.WMR_RIGHT_GRIP_PRESS:
                    key = getKeyCode(rightControllerId, 5);
                    break;
                case GENERIC_VR_BUTTON.WMR_LEFT_MENU_BUTTON_PRESS:
                    key = getKeyCode(rightControllerId, 6);
                    break;
                case GENERIC_VR_BUTTON.WMR_RIGHT_MENU_BUTTON_PRESS:
                    key = getKeyCode(rightControllerId, 7);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_REMOTE_ONE:
                    key = getKeyCode(rightControllerId, 0);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_REMOTE_TWO:
                    key = getKeyCode(rightControllerId, 1);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_TWO_PRESS:
                    key = getKeyCode(rightControllerId, 1);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_TWO_TOUCH:
                    key = getKeyCode(rightControllerId, 11);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_FOUR_PRESS:
                    key = getKeyCode(leftControllerId, 3);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_FOUR_TOUCH:
                    key = getKeyCode(leftControllerId, 13);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_START:
                    key = getKeyCode(leftControllerId, 7);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMBREST:
                    key = getKeyCode(leftControllerId, 18);
                    break;
                case GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMBREST:
                    key = getKeyCode(rightControllerId, 19);
                    break;
                case GENERIC_VR_BUTTON.DAYDREAM_LEFT_TOUCHPAD_TOUCH:
                    key = getKeyCode(leftControllerId, 18);
                    break;
                case GENERIC_VR_BUTTON.DAYDREAM_RIGHT_TOUCHPAD_TOUCH:
                    key = getKeyCode(rightControllerId, 19);
                    break;
                case GENERIC_VR_BUTTON.DAYDREAM_LEFT_TOUCHPAD_CLICK:
                    key = getKeyCode(leftControllerId, 16);
                    break;
                case GENERIC_VR_BUTTON.DAYDREAM_RIGHT_TOUCHPAD_CLICK:
                    key = getKeyCode(rightControllerId, 17);
                    break;
                case GENERIC_VR_BUTTON.DAYDREAM_LEFT_APP_BUTTON:
                    key = getKeyCode(leftControllerId, 2);
                    break;
                case GENERIC_VR_BUTTON.DAYDREAM_RIGHT_APP_BUTTON:
                    key = getKeyCode(rightControllerId, 0);
                    break;
            }

            if (key == KeyCode.None)
                Debug.LogWarning("[VREasy] GenericControllerTrigger: VR controller button not found! Either the selected controller is not configured or has not been found");
        }

        private KeyCode getKeyCode(int joystick, int button)
        {
            switch(joystick)
            {
                case 0:
                    {
                        switch(button)
                        {
                            case 0:
                                return KeyCode.Joystick1Button0;
                            case 1:
                                return KeyCode.Joystick1Button1;
                            case 2:
                                return KeyCode.Joystick1Button2;
                            case 3:
                                return KeyCode.Joystick1Button3;
                            case 4:
                                return KeyCode.Joystick1Button4;
                            case 5:
                                return KeyCode.Joystick1Button5;
                            case 6:
                                return KeyCode.Joystick1Button6;
                            case 7:
                                return KeyCode.Joystick1Button7;
                            case 8:
                                return KeyCode.Joystick1Button8;
                            case 9:
                                return KeyCode.Joystick1Button9;
                            case 10:
                                return KeyCode.Joystick1Button10;
                            case 11:
                                return KeyCode.Joystick1Button11;
                            case 12:
                                return KeyCode.Joystick1Button12;
                            case 13:
                                return KeyCode.Joystick1Button13;
                            case 14:
                                return KeyCode.Joystick1Button14;
                            case 15:
                                return KeyCode.Joystick1Button15;
                            case 16:
                                return KeyCode.Joystick1Button16;
                            case 17:
                                return KeyCode.Joystick1Button17;
                            case 18:
                                return KeyCode.Joystick1Button18;
                            case 19:
                                return KeyCode.Joystick1Button19;

                        }
                    }
                    break;
                case 1:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.Joystick2Button0;
                            case 1:
                                return KeyCode.Joystick2Button1;
                            case 2:
                                return KeyCode.Joystick2Button2;
                            case 3:
                                return KeyCode.Joystick2Button3;
                            case 4:
                                return KeyCode.Joystick2Button4;
                            case 5:
                                return KeyCode.Joystick2Button5;
                            case 6:
                                return KeyCode.Joystick2Button6;
                            case 7:
                                return KeyCode.Joystick2Button7;
                            case 8:
                                return KeyCode.Joystick2Button8;
                            case 9:
                                return KeyCode.Joystick2Button9;
                            case 10:
                                return KeyCode.Joystick2Button10;
                            case 11:
                                return KeyCode.Joystick2Button11;
                            case 12:
                                return KeyCode.Joystick2Button12;
                            case 13:
                                return KeyCode.Joystick2Button13;
                            case 14:
                                return KeyCode.Joystick2Button14;
                            case 15:
                                return KeyCode.Joystick2Button15;
                            case 16:
                                return KeyCode.Joystick2Button16;
                            case 17:
                                return KeyCode.Joystick2Button17;
                            case 18:
                                return KeyCode.Joystick2Button18;
                            case 19:
                                return KeyCode.Joystick2Button19;

                        }
                    }
                    break;
                case 2:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.Joystick3Button0;
                            case 1:
                                return KeyCode.Joystick3Button1;
                            case 2:
                                return KeyCode.Joystick3Button2;
                            case 3:
                                return KeyCode.Joystick3Button3;
                            case 4:
                                return KeyCode.Joystick3Button4;
                            case 5:
                                return KeyCode.Joystick3Button5;
                            case 6:
                                return KeyCode.Joystick3Button6;
                            case 7:
                                return KeyCode.Joystick3Button7;
                            case 8:
                                return KeyCode.Joystick3Button8;
                            case 9:
                                return KeyCode.Joystick3Button9;
                            case 10:
                                return KeyCode.Joystick3Button10;
                            case 11:
                                return KeyCode.Joystick3Button11;
                            case 12:
                                return KeyCode.Joystick3Button12;
                            case 13:
                                return KeyCode.Joystick3Button13;
                            case 14:
                                return KeyCode.Joystick3Button14;
                            case 15:
                                return KeyCode.Joystick3Button15;
                            case 16:
                                return KeyCode.Joystick3Button16;
                            case 17:
                                return KeyCode.Joystick3Button17;
                            case 18:
                                return KeyCode.Joystick3Button18;
                            case 19:
                                return KeyCode.Joystick3Button19;

                        }
                    }
                    break;
                case 3:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.Joystick4Button0;
                            case 1:
                                return KeyCode.Joystick4Button1;
                            case 2:
                                return KeyCode.Joystick4Button2;
                            case 3:
                                return KeyCode.Joystick4Button3;
                            case 4:
                                return KeyCode.Joystick4Button4;
                            case 5:
                                return KeyCode.Joystick4Button5;
                            case 6:
                                return KeyCode.Joystick4Button6;
                            case 7:
                                return KeyCode.Joystick4Button7;
                            case 8:
                                return KeyCode.Joystick4Button8;
                            case 9:
                                return KeyCode.Joystick4Button9;
                            case 10:
                                return KeyCode.Joystick4Button10;
                            case 11:
                                return KeyCode.Joystick4Button11;
                            case 12:
                                return KeyCode.Joystick4Button12;
                            case 13:
                                return KeyCode.Joystick4Button13;
                            case 14:
                                return KeyCode.Joystick4Button14;
                            case 15:
                                return KeyCode.Joystick4Button15;
                            case 16:
                                return KeyCode.Joystick4Button16;
                            case 17:
                                return KeyCode.Joystick4Button17;
                            case 18:
                                return KeyCode.Joystick4Button18;
                            case 19:
                                return KeyCode.Joystick4Button19;

                        }
                    }
                    break;
                case 4:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.Joystick5Button0;
                            case 1:
                                return KeyCode.Joystick5Button1;
                            case 2:
                                return KeyCode.Joystick5Button2;
                            case 3:
                                return KeyCode.Joystick5Button3;
                            case 4:
                                return KeyCode.Joystick5Button4;
                            case 5:
                                return KeyCode.Joystick5Button5;
                            case 6:
                                return KeyCode.Joystick5Button6;
                            case 7:
                                return KeyCode.Joystick5Button7;
                            case 8:
                                return KeyCode.Joystick5Button8;
                            case 9:
                                return KeyCode.Joystick5Button9;
                            case 10:
                                return KeyCode.Joystick5Button10;
                            case 11:
                                return KeyCode.Joystick5Button11;
                            case 12:
                                return KeyCode.Joystick5Button12;
                            case 13:
                                return KeyCode.Joystick5Button13;
                            case 14:
                                return KeyCode.Joystick5Button14;
                            case 15:
                                return KeyCode.Joystick5Button15;
                            case 16:
                                return KeyCode.Joystick5Button16;
                            case 17:
                                return KeyCode.Joystick5Button17;
                            case 18:
                                return KeyCode.Joystick5Button18;
                            case 19:
                                return KeyCode.Joystick5Button19;

                        }
                    }
                    break;
                case 5:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.Joystick6Button0;
                            case 1:
                                return KeyCode.Joystick6Button1;
                            case 2:
                                return KeyCode.Joystick6Button2;
                            case 3:
                                return KeyCode.Joystick6Button3;
                            case 4:
                                return KeyCode.Joystick6Button4;
                            case 5:
                                return KeyCode.Joystick6Button5;
                            case 6:
                                return KeyCode.Joystick6Button6;
                            case 7:
                                return KeyCode.Joystick6Button7;
                            case 8:
                                return KeyCode.Joystick6Button8;
                            case 9:
                                return KeyCode.Joystick6Button9;
                            case 10:
                                return KeyCode.Joystick6Button10;
                            case 11:
                                return KeyCode.Joystick6Button11;
                            case 12:
                                return KeyCode.Joystick6Button12;
                            case 13:
                                return KeyCode.Joystick6Button13;
                            case 14:
                                return KeyCode.Joystick6Button14;
                            case 15:
                                return KeyCode.Joystick6Button15;
                            case 16:
                                return KeyCode.Joystick6Button16;
                            case 17:
                                return KeyCode.Joystick6Button17;
                            case 18:
                                return KeyCode.Joystick6Button18;
                            case 19:
                                return KeyCode.Joystick6Button19;

                        }
                    }
                    break;
                case 6:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.Joystick7Button0;
                            case 1:
                                return KeyCode.Joystick7Button1;
                            case 2:
                                return KeyCode.Joystick7Button2;
                            case 3:
                                return KeyCode.Joystick7Button3;
                            case 4:
                                return KeyCode.Joystick7Button4;
                            case 5:
                                return KeyCode.Joystick7Button5;
                            case 6:
                                return KeyCode.Joystick7Button6;
                            case 7:
                                return KeyCode.Joystick7Button7;
                            case 8:
                                return KeyCode.Joystick7Button8;
                            case 9:
                                return KeyCode.Joystick7Button9;
                            case 10:
                                return KeyCode.Joystick7Button10;
                            case 11:
                                return KeyCode.Joystick7Button11;
                            case 12:
                                return KeyCode.Joystick7Button12;
                            case 13:
                                return KeyCode.Joystick7Button13;
                            case 14:
                                return KeyCode.Joystick7Button14;
                            case 15:
                                return KeyCode.Joystick7Button15;
                            case 16:
                                return KeyCode.Joystick7Button16;
                            case 17:
                                return KeyCode.Joystick7Button17;
                            case 18:
                                return KeyCode.Joystick7Button18;
                            case 19:
                                return KeyCode.Joystick7Button19;

                        }
                    }
                    break;
                case 7:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.Joystick8Button0;
                            case 1:
                                return KeyCode.Joystick8Button1;
                            case 2:
                                return KeyCode.Joystick8Button2;
                            case 3:
                                return KeyCode.Joystick8Button3;
                            case 4:
                                return KeyCode.Joystick8Button4;
                            case 5:
                                return KeyCode.Joystick8Button5;
                            case 6:
                                return KeyCode.Joystick8Button6;
                            case 7:
                                return KeyCode.Joystick8Button7;
                            case 8:
                                return KeyCode.Joystick8Button8;
                            case 9:
                                return KeyCode.Joystick8Button9;
                            case 10:
                                return KeyCode.Joystick8Button10;
                            case 11:
                                return KeyCode.Joystick8Button11;
                            case 12:
                                return KeyCode.Joystick8Button12;
                            case 13:
                                return KeyCode.Joystick8Button13;
                            case 14:
                                return KeyCode.Joystick8Button14;
                            case 15:
                                return KeyCode.Joystick8Button15;
                            case 16:
                                return KeyCode.Joystick8Button16;
                            case 17:
                                return KeyCode.Joystick8Button17;
                            case 18:
                                return KeyCode.Joystick8Button18;
                            case 19:
                                return KeyCode.Joystick8Button19;

                        }
                    }
                    break;
                default:
                    {
                        switch (button)
                        {
                            case 0:
                                return KeyCode.JoystickButton0;
                            case 1:
                                return KeyCode.JoystickButton1;
                            case 2:
                                return KeyCode.JoystickButton2;
                            case 3:
                                return KeyCode.JoystickButton3;
                            case 4:
                                return KeyCode.JoystickButton4;
                            case 5:
                                return KeyCode.JoystickButton5;
                            case 6:
                                return KeyCode.JoystickButton6;
                            case 7:
                                return KeyCode.JoystickButton7;
                            case 8:
                                return KeyCode.JoystickButton8;
                            case 9:
                                return KeyCode.JoystickButton9;
                            case 10:
                                return KeyCode.JoystickButton10;
                            case 11:
                                return KeyCode.JoystickButton11;
                            case 12:
                                return KeyCode.JoystickButton12;
                            case 13:
                                return KeyCode.JoystickButton13;
                            case 14:
                                return KeyCode.JoystickButton14;
                            case 15:
                                return KeyCode.JoystickButton15;
                            case 16:
                                return KeyCode.JoystickButton16;
                            case 17:
                                return KeyCode.JoystickButton17;
                            case 18:
                                return KeyCode.JoystickButton18;
                            case 19:
                                return KeyCode.JoystickButton19;

                        }
                    }
                    break;

            }

            // in case of incorrect button id
            return KeyCode.None;
        }

        public override bool Triggered()
        {
            return Input.GetKey(key);
        }

    }
}