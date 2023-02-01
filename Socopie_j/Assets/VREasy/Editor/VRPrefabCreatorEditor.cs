using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Helper class to aid users in the creation of VR players
 * Includes selection of HMD, adding selector types and associated triggers, extras (FPS locomotion, Teleport controller)
 */

namespace VREasy
{

    public class VRPrefabCreatorEditor : EditorWindow
    {
        [MenuItem("VREasy/VR Prefab Creator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(VRPrefabCreatorEditor), false, "Prefab Creator");
        }

        private Vector2 scrollPos;
        // defaults
        private VR_HMD_TYPE hmd_type;
        private int selector_trigger_index = 0;
        private VRSELECTOR_TYPE selector_type;
        private bool left_controller = false;
        private bool right_controller = true;
        private bool headset = false;
        private bool add_locomotion = false;
        private VRLOCOMOTION_INPUT locomotion_input;
        private bool add_locomotion_body = false;
        private bool add_teleport_controller = false;
        private bool left_locomotion_input = false;
        private bool right_locomotion_input = true;
        private int teleport_trigger_index = 0;
        private bool left_teleport = false;
        private bool right_teleport = true;

        private bool add_hands = false;
        private bool left_gesture_hand = true;
        private bool right_gesture_hand = true;

        // used to select triggers
        private List<string> triggerAssemblyNames = new List<string>();
        private List<string> triggerClassNames = new List<string>();

        // object references once the prefab is created
        GameObject prefabReference = null;
        GameObject leftControllerReference = null;
        GameObject rightControllerReference = null;
        GameObject headReference = null;
        GameObject rightTeleportReference = null;
        GameObject leftTeleportReference = null;
        GameObject locomotionReference = null;


        bool handleRepaintErrors = false;
        void OnGUI()
        {
            // populate trigger assembly names
            VREasy_utils.LoadClassesFromAssembly(typeof(VRGrabTrigger), ref triggerAssemblyNames, ref triggerClassNames, "Trigger");
            
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }
            bool dependencies_met = true;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Configure your VR Player", EditorStyles.wordWrappedLabel);

            EditorGUILayout.Separator();

            // HMD selector
            dependencies_met &= selectHMDGUI();
            EditorGUILayout.Separator();

            // Add selector type
            dependencies_met &= addSelectorGUI();
            EditorGUILayout.Separator();

            // Add extras
            dependencies_met &= addExtrasGUI();
            EditorGUILayout.Separator();

            if (EditorGUI.EndChangeCheck())
            {
                // clear prefab references
                prefabReference = null;
                leftControllerReference = null;
                rightControllerReference = null;
                headReference = null;
                rightTeleportReference = null;
                leftTeleportReference = null;
                locomotionReference = null;
            }

            EditorGUILayout.Separator();
            if (dependencies_met)
            {
                EditorGUILayout.Separator();
                if (GUILayout.Button("Generate"))
                {
                    generateVRprefab();
                    EditorUtility.DisplayDialog("Prefab successfully created", "Object [" + prefabReference.name +
                                            "] has been created. Make sure you configure the customisable scripts via the inspector. " +
                                            "Use the Locate section of the VR Prefab Creator panel to find the scripts.", "OK");
                }
            } else
            {
                if (GUILayout.Button("Open SDK Checker"))
                {
                    EditorApplication.ExecuteMenuItem("VREasy/SDK Checker");
                }
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Dependencies not met", EditorStyles.boldLabel);
                
            }

            

            // 5 display message to guide the user on where certain elements can be configured
            displayLocateGUI();
            
            VREasy_utils.DrawHelperInfo();

            EditorGUILayout.EndScrollView();


        }

        private void displayLocateGUI()
        {
            if (prefabReference == null) return;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(prefabReference.name + " created successfully!", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Locate scripts", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            // Main object
            displayLocateItem("Object", prefabReference);
            // teleport 
            if (add_teleport_controller)
            {
                if(left_teleport && leftTeleportReference != null)
                {
                    displayLocateItem("Left teleport script", leftTeleportReference);
                }
                if(right_teleport && rightTeleportReference != null)
                {
                    displayLocateItem("Right teleport script", rightTeleportReference);
                }
                
            }
            // locomotion 
            if (add_locomotion && locomotionReference != null)
            {
                displayLocateItem("Locomotion script", locomotionReference);
            }
            // selector
            switch(selector_type)
            {
                case VRSELECTOR_TYPE.MIXED_REALITY:
                case VRSELECTOR_TYPE.SIGHT:
                    {
                        displayLocateItem("LOS Selector", headReference);
                    }
                    break;
                case VRSELECTOR_TYPE.POINTER:
                case VRSELECTOR_TYPE.TOUCH:
                    {
                        if(left_controller)
                        {
                            displayLocateItem("Left selector", leftControllerReference);
                        }
                        if(right_controller)
                        {
                            displayLocateItem("Right selector", rightControllerReference);
                        }
                    }
                    break;

            }
        }
        private void displayLocateItem(string element, GameObject reference)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(element + ": " + reference.name);
            if (GUILayout.Button("Locate"))
            {
                Selection.activeGameObject = reference;
            }
            EditorGUILayout.EndHorizontal();
        }


        //private bool checkTriggerDependency(ref VR_CONTROLLER_TYPE trigger)
        private bool checkTriggerDependency(ref int trigger_index)
        {
            bool dependencies_met = true;

            // oculus touch and remote controllers (oculus utilities)
            // gear VR controller (oculus utilities)
            // Vive (SteamVR)
            // vive focus (WaveSDK)
            // Daydream (GoogleVR SDK)
            string className = triggerClassNames[trigger_index].ToLower();
            if (className.Contains("oculus") || className.Contains("gearvr"))
            {
#if !VREASY_OCULUS_UTILITIES_SDK
                dependencies_met = false;
                showOculusDependency();
#endif
            }
            if(className.Contains("googlevr"))
            {
#if !VREASY_GOOGLEVR_SDK
                dependencies_met = false;
                showDaydreamDependency();
#endif
            }
            if(className.Contains("steam"))
            {
#if !VREASY_STEAM_SDK
                dependencies_met = false;
                showSteamVRdependency();
#endif
            }
            if(className.Contains("wave"))
            {
#if !VREASY_WAVEVR_SDK
                dependencies_met = false;
                showWaveSDKdependency();
#endif
            }

            // show native generic alternative for oculus and steam
            if (className.Contains("oculus") || className.Contains("steam"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("TRIGGER: You can still use the native generic integration that does not require an external SDK", MessageType.Info);
                if (GUILayout.Button("Set Trigger to generic"))
                {
                    for(int ii=0; ii < triggerClassNames.Count; ii++)
                    {
                        if(triggerClassNames[ii].ToLower().Contains("generic"))
                        {
                            trigger_index = ii;
                            break;
                        }
                    }
                    
                }
                EditorGUI.indentLevel--;
            }

            return dependencies_met;

        }

        private bool checkLocomotionInputDependency()
        {
            bool dependencies_met = true;

            switch(locomotion_input)
            {
                case VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER:
                    {
#if !VREASY_GOOGLEVR_SDK
                        dependencies_met = false;
                        showDaydreamDependency();
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.GEAR_VR_CONTROLLER:
                case VRLOCOMOTION_INPUT.OCULUS_CONTROLLER:
                    {
#if !VREASY_OCULUS_UTILITIES_SDK
                        dependencies_met = false;
                        showOculusDependency();
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.STEAM_CONTROLLER:
                    {
#if !VREASY_STEAM_SDK
                        dependencies_met = false;
                        showSteamVRdependency();
#endif
                    }
                    break;
            }

            // show native generic alternative for oculus and steam
            if(locomotion_input == VRLOCOMOTION_INPUT.STEAM_CONTROLLER 
                    || locomotion_input == VRLOCOMOTION_INPUT.OCULUS_CONTROLLER 
                    || locomotion_input == VRLOCOMOTION_INPUT.GEAR_VR_CONTROLLER)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("LOCOMOTION: You can still use the native generic integration that does not require an external SDK", MessageType.Info);
                if (GUILayout.Button("Set locomotion input to generic")) {
                    locomotion_input = VRLOCOMOTION_INPUT.GENERIC_VR_CONTROLLER;
                }
                EditorGUI.indentLevel--;
            }
            return dependencies_met;
        }

        private bool selectHMDGUI()
        {
            bool dependencies_met = true;
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Select HMD", EditorStyles.boldLabel);
            hmd_type = (VR_HMD_TYPE)EditorGUILayout.EnumPopup("VR HMD", hmd_type);

            // check HMD dependencies
            // https://unity3d.com/unity/features/multiplatform
            // Native support only for: Oculus Rift, Gear VR, PlayStation VR, Microsoft HoloLens, Steam VR and Google Daydream and Cardboard (?)
            // Google cardboard require Google VR? https://docs.unity3d.com/Manual/googlevr_sdk_overview.html
            // Vive focus https://hub.vive.com/storage/app/doc/en-us/UnityPluginGettingStart.html
            if (hmd_type == VR_HMD_TYPE.VIVE_FOCUS)
            {
#if !VREASY_WAVEVR_SDK
                dependencies_met = false;
                showWaveSDKdependency();
#endif
            }

            EditorGUILayout.EndVertical();
            return dependencies_met;
        }

        private bool addSelectorGUI()
        {
            // This function should call Selector constructors (or call same functions as AddSelectorHelper for consistency)
            bool dependencies_met = true;
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Add selector type", EditorStyles.boldLabel);
            selector_type = (VRSELECTOR_TYPE)EditorGUILayout.EnumPopup("Selector", selector_type);
            EditorGUILayout.BeginHorizontal();
            switch(selector_type)
            {
                case VRSELECTOR_TYPE.POINTER:
                    {
                        left_controller = EditorGUILayout.Toggle("Left", left_controller);
                        right_controller = EditorGUILayout.Toggle("Right", right_controller);
                        break;
                    }
                case VRSELECTOR_TYPE.TOUCH:
                    {
                        left_controller = EditorGUILayout.Toggle("Left", left_controller);
                        right_controller = EditorGUILayout.Toggle("Right", right_controller);
                        headset = EditorGUILayout.Toggle("Head", headset);
                        break;
                    }
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            // VR Controller (trigger for selector)
            switch(selector_type)
            {
                case VRSELECTOR_TYPE.MIXED_REALITY:
                case VRSELECTOR_TYPE.POINTER:
                case VRSELECTOR_TYPE.TOUCH:
                    {
                        EditorGUI.indentLevel++;
                        selector_trigger_index = EditorGUILayout.Popup("Add trigger", selector_trigger_index, triggerClassNames.ToArray());
                        dependencies_met &= checkTriggerDependency(ref selector_trigger_index);
                        EditorGUI.indentLevel--;
                    }
                    break;
            }

            EditorGUILayout.EndVertical();
            return dependencies_met;

        }
        
        private bool addExtrasGUI()
        {
            bool dependencies_met = true;
            EditorGUILayout.LabelField("Extras", EditorStyles.boldLabel);
            // FPS locomotion
            EditorGUILayout.BeginVertical("Box");
            add_locomotion = EditorGUILayout.Toggle("Add FPS Locomotion", add_locomotion);
            if(add_locomotion)
            {
                EditorGUI.indentLevel++;
                locomotion_input = (VRLOCOMOTION_INPUT)EditorGUILayout.EnumPopup("Input", locomotion_input);
                switch(locomotion_input)
                {
                    case VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER:
                    case VRLOCOMOTION_INPUT.GEAR_VR_CONTROLLER:
                    case VRLOCOMOTION_INPUT.OCULUS_CONTROLLER:
                    case VRLOCOMOTION_INPUT.STEAM_CONTROLLER:
                        {
                            EditorGUILayout.BeginHorizontal();
                            left_locomotion_input = EditorGUILayout.Toggle("Left", left_locomotion_input);
                            right_locomotion_input = EditorGUILayout.Toggle("Right", right_locomotion_input);
                            EditorGUILayout.EndHorizontal();
                        }
                        break;
                }
                add_locomotion_body = EditorGUILayout.Toggle("Add physical body", add_locomotion_body);
                dependencies_met &= checkLocomotionInputDependency();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            
            // Teleport controller
            EditorGUILayout.BeginVertical("Box");
            add_teleport_controller = EditorGUILayout.Toggle("Add Teleport Controller", add_teleport_controller);
            if(add_teleport_controller)
            {
                EditorGUI.indentLevel++;
                teleport_trigger_index = EditorGUILayout.Popup("Add trigger", teleport_trigger_index, triggerClassNames.ToArray());
                dependencies_met &= checkTriggerDependency(ref teleport_trigger_index);
                EditorGUILayout.BeginHorizontal();
                left_teleport = EditorGUILayout.Toggle("Left", left_teleport);
                right_teleport = EditorGUILayout.Toggle("Right", right_teleport);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                
            }

            EditorGUILayout.EndVertical();

            // Hands
            EditorGUILayout.BeginVertical("Box");
            add_hands = EditorGUILayout.Toggle("Add Gesture Hands", add_hands);
            if (add_hands)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                left_gesture_hand = EditorGUILayout.Toggle("Left", left_gesture_hand);
                right_gesture_hand = EditorGUILayout.Toggle("Right", right_gesture_hand);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            return dependencies_met;
        }

        private void generateVRprefab()
        {
            // generate vr prefab based on the properties selection
            // 1 Generate game object hierarchy 
            //      Body (for locomotion and teleporting)
            //      Head (main camera)
            //      Hands (controllers)
            GameObject playerPrefab = new GameObject("[VREasy] PlayerPrefab");
            GameObject head = new GameObject("Head");
            head.transform.parent = playerPrefab.transform;
            head.AddComponent<Camera>();
            head.AddComponent<AudioListener>();
            GameObject rightController = new GameObject("RightController");
            rightController.transform.parent = playerPrefab.transform;
            GameObject leftController = new GameObject("LeftController");
            leftController.transform.parent = playerPrefab.transform;

            rightTeleportReference = rightController;
            leftTeleportReference = leftController;
            prefabReference = playerPrefab;
            headReference = head;
            rightControllerReference = rightController;
            leftControllerReference = leftController;
            locomotionReference = playerPrefab;

            // configure controllers for Device
            switch(hmd_type)
            {
                case VR_HMD_TYPE.VIVE_FOCUS:
                    {
#if VREASY_WAVEVR_SDK
                        headReference.AddComponent<WaveVR_Render>();
                        WaveVR_DevicePoseTracker tracker = headReference.AddComponent<WaveVR_DevicePoseTracker>();
                        tracker.type = WaveVR_Controller.EDeviceType.Head;
                        // create controller objects
                        WaveVRPoseController left_device = leftControllerReference.AddComponent<WaveVRPoseController>();
                        left_device.device = WaveVR_Controller.EDeviceType.NonDominant;
                        WaveVRPoseController right_device = rightControllerReference.AddComponent<WaveVRPoseController>();
                        right_device.device = WaveVR_Controller.EDeviceType.Dominant;
#endif
                    }
                    break;
                case VR_HMD_TYPE.GOOGLE_DAYDREAM:
                case VR_HMD_TYPE.GOOGLE_CARDBOARD:
                    {
#if VREASY_GOOGLEVR_SDK
                        GvrTrackedController left_device = leftControllerReference.AddComponent<GvrTrackedController>();
                        left_device.IsDeactivatedWhenDisconnected = true;
                        left_device.ControllerHand = GvrControllerHand.Left;
                        left_device.gameObject.AddComponent<GvrArmModel>();
                        GvrArmModel leftArm = left_device.gameObject.AddComponent<GvrArmModel>();
                        left_device.ArmModel = leftArm;
                        GvrTrackedController right_device = rightControllerReference.AddComponent<GvrTrackedController>();
                        right_device.IsDeactivatedWhenDisconnected = true;
                        right_device.ControllerHand = GvrControllerHand.Right;
                        GvrArmModel rightArm = right_device.gameObject.AddComponent<GvrArmModel>();
                        right_device.ArmModel = rightArm;
                        // May require an instance of GvrControllerInput to be present in the scene!!
#endif
                    }
                    break;
                default:
                    {
                        GenericVRcontroller left_device = leftControllerReference.AddComponent<GenericVRcontroller>();
                        GenericVRcontroller right_device = rightControllerReference.AddComponent<GenericVRcontroller>();

#if UNITY_2017_2_OR_NEWER
                        left_device.vrnode = UnityEngine.XR.XRNode.LeftHand;
                        right_device.vrnode = UnityEngine.XR.XRNode.RightHand;
#else
                        left_device.vrnode = UnityEngine.VR.VRNode.LeftHand;
                        right_device.vrnode = UnityEngine.VR.VRNode.RightHand;
#endif
                    }
                    break;
            }

            // Add hand models (if applicable)
            // load Hand_Gloves_R from resources
            // x scale = -1 for left controller
            switch(hmd_type)
            {
                default:
                    {
                        GameObject handModel;
                        if (right_gesture_hand && add_hands)
                        {
                            handModel = Resources.Load<GameObject>("Gesture_Resources/Glove_R") as GameObject;
                            GameObject rightHand = Instantiate(handModel);
                            rightHand.name = "Hand";
                            rightHand.transform.parent = rightControllerReference.transform;
                            rightHand.transform.localScale = Vector3.one;
                            rightHand.transform.rotation = new Quaternion(0.0f,180.0f,0.0f,0.0f);

                            Animator hand_anim = rightHand.GetComponent<Animator>();
                            hand_anim.runtimeAnimatorController = Resources.Load("Gesture_Resources/Animations/Head") as RuntimeAnimatorController;

                            rightHand.AddComponent<VRGestureHand>();
                        }
                        else
                        {
                            handModel = Resources.Load<GameObject>("Hand_R") as GameObject;
                            GameObject rightHand = Instantiate(handModel);
                            rightHand.name = "Hand";
                            rightHand.transform.parent = rightControllerReference.transform;
                            rightHand.transform.localScale = Vector3.one;
                            rightHand.transform.rotation = Quaternion.identity;
                        }

                        if (left_gesture_hand && add_hands)
                        {
                            handModel = Resources.Load<GameObject>("Gesture_Resources/Glove_R") as GameObject;
                            GameObject leftHand = Instantiate(handModel);
                            leftHand.name = "Hand";
                            leftHand.transform.parent = leftControllerReference.transform;
                            leftHand.transform.localScale = new Vector3(-1, 1, 1);
                            leftHand.transform.rotation = new Quaternion(0.0f, 180.0f, 0.0f, 0.0f);

                            Animator hand_anim = leftHand.GetComponent<Animator>();
                            hand_anim.runtimeAnimatorController = Resources.Load("Gesture_Resources/Animations/Head") as RuntimeAnimatorController;

                            leftHand.AddComponent<VRGestureHand>();
                        }
                        else
                        {
                            handModel = Resources.Load<GameObject>("Hand_R") as GameObject;
                            GameObject leftHand = Instantiate(handModel);
                            leftHand.name = "Hand";
                            leftHand.transform.parent = leftControllerReference.transform;
                            leftHand.transform.localScale = new Vector3(-1, 1, 1);
                            leftHand.transform.rotation = Quaternion.identity;
                        }
                    }
                    break;
            }

            // 2 add selector
            switch (selector_type)
            {
                case VRSELECTOR_TYPE.MIXED_REALITY:
                    {
                        MRSelector selector = MRSelector.CreateMRSelector(ref headReference);
                        selector.grabTrigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(headReference, triggerAssemblyNames[selector_trigger_index]);
                        selector.selectionTrigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(headReference, triggerAssemblyNames[selector_trigger_index]);
                    }
                    break;
                case VRSELECTOR_TYPE.POINTER:
                    {
                        if (left_controller)
                        {
                            PointerSelector selector = PointerSelector.CreatePointerSelector(ref leftControllerReference);
                            selector.grabTrigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(leftControllerReference, triggerAssemblyNames[selector_trigger_index]);
                        }
                        if (right_controller)
                        {
                            PointerSelector selector = PointerSelector.CreatePointerSelector(ref rightControllerReference);
                            selector.grabTrigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(rightControllerReference, triggerAssemblyNames[selector_trigger_index]);
                        }

                    }
                    break;
                case VRSELECTOR_TYPE.TOUCH:
                    {
                        if (left_controller)
                        {
                            TouchSelector selector = TouchSelector.CreateTouchSelector(ref leftControllerReference);
                            selector.grabTrigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(leftControllerReference, triggerAssemblyNames[selector_trigger_index]);
                        }
                        if (right_controller)
                        {
                            TouchSelector selector = TouchSelector.CreateTouchSelector(ref rightControllerReference);
                            selector.grabTrigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(rightControllerReference, triggerAssemblyNames[selector_trigger_index]);
                        }
                        if (headset)
                        {
                            TouchSelector selector = TouchSelector.CreateTouchSelector(ref headReference);
                            selector.grabTrigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(headReference, triggerAssemblyNames[selector_trigger_index]);
                        }

                    }
                    break;
                case VRSELECTOR_TYPE.SIGHT:
                    {
                        SightSelector.CreateSightSelector(ref headReference);
                        
                    }
                    break;

            }

            // 3 add locomotion system
            if(add_locomotion)
            {
                VRSimpleFPSLocomotion locomotion = locomotionReference.AddComponent<VRSimpleFPSLocomotion>();
                locomotion.head = headReference.transform;
                locomotion.input = locomotion_input;
                if(add_locomotion_body)
                {
                    prefabReference.AddComponent<CapsuleCollider>();
                    prefabReference.AddComponent<Rigidbody>();
                }
                // pass on info regarding input side (left/right)
                locomotion.useLeftController = left_locomotion_input;
                locomotion.useRightController = right_locomotion_input;                
            }

            // 4 add teleport controller
            // TODO consider handness
            if (add_teleport_controller)
            {
                if (right_teleport)
                {
                    TeleportController teleport = rightTeleportReference.AddComponent<TeleportController>();
                    teleport.Teleport.HMD = playerPrefab.transform;
                    // add trigger
                    // we need to use reflection, otherwise we have to manually translate VR_CONTROLLER_TYPE to VRGrabTrigger objects
                    teleport.trigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(rightTeleportReference, triggerAssemblyNames[teleport_trigger_index]);
                }
                if (left_teleport)
                {
                    TeleportController teleport = leftTeleportReference.AddComponent<TeleportController>();
                    teleport.Teleport.HMD = playerPrefab.transform;
                    // add trigger
                    // we need to use reflection, otherwise we have to manually translate VR_CONTROLLER_TYPE to VRGrabTrigger objects
                    teleport.trigger = VREasy_utils.LoadAndSetClassFromAssembly<VRGrabTrigger>(leftTeleportReference, triggerAssemblyNames[teleport_trigger_index]);
                }


            }

        }

        ///////////////////////////////////////////////
        // helper functions to show Links to dependencies
        ///////////////////////////////////////////////
        private void showDaydreamDependency()
        {
            EditorGUILayout.HelpBox("To use the Daydream controller, users must import the external GoogleVR plugin from the official site AND activate the Daydream integration from our SDK Checker panel", MessageType.Error);
            if (GUILayout.Button("Download GoogleVR"))
            {
                string link = "https://developers.google.com/vr/develop/unity/get-started-android";
                Application.OpenURL(link);
            }
        }

        private void showWaveSDKdependency()
        {
            EditorGUILayout.HelpBox("To use the Vive Focus 6DOF headset and controller, users must import the external WaveSDK plugin from the official site AND activate WaveSDK integration from our SDK Checker panel", MessageType.Error);
            if (GUILayout.Button("Download WaveSDK"))
            {
                string link = "https://developer.vive.com/resources/knowledgebase/wave-sdk/";
                Application.OpenURL(link);
            }
        }

        private void showOculusDependency()
        {
            EditorGUILayout.HelpBox("To use the Gear VR, Oculus Rift or Oculus Go full controller input, users must import the external Oculus Integration plugin from the asset store AND activate Oculus utilities from our SDK Checker panel", MessageType.Error);
            if (GUILayout.Button("Download Oculus Integration"))
            {
                string link = "https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022";
                Application.OpenURL(link);
            }
        }

        private void showSteamVRdependency()
        {
            EditorGUILayout.HelpBox("To use the SteamVR full controller input, users must import the external SteamVR plugin from the asset store AND activate SteamVR from our SDK Checker panel", MessageType.Error);
            if (GUILayout.Button("Download SteamVR"))
            {
                string link = "https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647";
                Application.OpenURL(link);
            }
        }

    }
}