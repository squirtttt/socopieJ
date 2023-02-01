using UnityEngine;
using System.Collections;
using UnityEditor;
#if VREASY_STEAM_SDK
using Valve.VR;
#endif

namespace VREasy
{
    [CustomEditor(typeof(VRSimpleFPSLocomotion))]
    public class VRSimpleFPSLocomotionEditor : Editor
    {
        [MenuItem("VREasy/Components/Simple FPS locomotion")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<VRSimpleFPSLocomotion>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add FPS locomotion you must select a game object in the hierarchy first", "OK");
            }
        }

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
            VRSimpleFPSLocomotion locomotion = (VRSimpleFPSLocomotion)target;

            EditorGUI.BeginChangeCheck();
            float speed = EditorGUILayout.FloatField("Move Speed", locomotion.speed);
            
            Transform head = (Transform)EditorGUILayout.ObjectField("Head", locomotion.head, typeof(Transform), true);
            bool fixedForward = EditorGUILayout.Toggle("Fixed forward", locomotion.fixedForward);
            float fixedMovement = locomotion.fixedMovement;
            if (fixedForward)
            {
                fixedMovement = EditorGUILayout.FloatField("Forward speed", locomotion.fixedMovement);
            }
            bool inverseZaxis = EditorGUILayout.Toggle("Inverse forward", locomotion.inverseZaxis);
            X_AXIS_TYPE xType = (X_AXIS_TYPE)EditorGUILayout.EnumPopup("X move type", locomotion.xAxisType);
            float steppedRotateAngle = locomotion.stepRotationAngle;
            float steppedRotateDelay = locomotion.stepRotationDelay;
            if(xType == X_AXIS_TYPE.STEPPED_ROTATE)
            {
                EditorGUI.indentLevel++;
                steppedRotateAngle = EditorGUILayout.Slider("Step angle", locomotion.stepRotationAngle, 0.0f, 359.0f);
                steppedRotateDelay = EditorGUILayout.FloatField("Rotation delay", locomotion.stepRotationDelay);
                EditorGUI.indentLevel--;
            }
            bool fixedHeight = EditorGUILayout.Toggle("Fixed height", locomotion.fixedHeight);
            float deadZone = EditorGUILayout.Slider("Dead zone (%)", locomotion.deadZone,0.0f,100.0f);

            // onlyy show handness for controllers that have this option (double controls)
            // Steam VR also accepts handness but it is controlled by a custom enum later
            bool useLeftController = locomotion.useLeftController;
            bool useRightController = locomotion.useRightController;
            if (locomotion.input == VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER ||
#if UNITY_2019_1_OR_NEWER
                    locomotion.input == VRLOCOMOTION_INPUT.GENERIC_VR_CONTROLLER ||
#endif
                    locomotion.input == VRLOCOMOTION_INPUT.OCULUS_CONTROLLER ||
                    locomotion.input == VRLOCOMOTION_INPUT.WAVEVR)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Hand control");
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                useLeftController = EditorGUILayout.Toggle("Left", locomotion.useLeftController);
                useRightController = EditorGUILayout.Toggle("Right", locomotion.useRightController);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
#if !UNITY_2019_1_OR_NEWER
            if(locomotion.input == VRLOCOMOTION_INPUT.GENERIC_VR_CONTROLLER)
            {
                EditorGUILayout.HelpBox("Handness via the Generic Controller is only supported from Unity 2019.1", MessageType.Warning);
            }
#endif
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(locomotion,"Changed locomotion parameters");
                locomotion.head = head;
                locomotion.speed = speed;
                locomotion.fixedHeight = fixedHeight;
                locomotion.fixedForward = fixedForward;
                locomotion.fixedMovement = fixedMovement;
                locomotion.xAxisType = xType;
                locomotion.useLeftController = useLeftController;
                locomotion.useRightController = useRightController;
                locomotion.inverseZaxis = inverseZaxis;
                locomotion.deadZone = deadZone;
                locomotion.stepRotationDelay = steppedRotateDelay;
                locomotion.stepRotationAngle = steppedRotateAngle;
            }
            EditorGUILayout.Separator();

            

            locomotion.input = (VRLOCOMOTION_INPUT)EditorGUILayout.EnumPopup("Input type", locomotion.input);
            switch (locomotion.input)
            {
                case VRLOCOMOTION_INPUT.UNITY_INPUT:
                    EditorGUILayout.LabelField("Movement input based on Horizontal and Vertical axis from the Input System", EditorStyles.wordWrappedLabel);
                    break;
                case VRLOCOMOTION_INPUT.STEAM_CONTROLLER:
                    {
#if VREASY_STEAM_SDK
                        EditorGUI.indentLevel++;
                        //locomotion.trackedObject = (SteamVR_TrackedObject)EditorGUILayout.ObjectField("Tracked controller", locomotion.trackedObject, typeof(SteamVR_TrackedObject), true);
                        locomotion.inputSource = (SteamVR_Input_Sources)EditorGUILayout.EnumPopup("Input source",locomotion.inputSource);
                        var serializedObject = new SerializedObject(target);
                        var property = serializedObject.FindProperty("directionAction");
                        serializedObject.Update();
                        EditorGUILayout.PropertyField(property, true);
                        serializedObject.ApplyModifiedProperties();
                        //locomotion.directionAction = (SteamVR_Action_Vector2)EditorGUILayout.ObjectField("Tracked action", locomotion.directionAction, typeof(SteamVR_Action_Vector2), true);
                        EditorGUI.indentLevel--;
#else
                        EditorGUILayout.HelpBox("Import Steam SDK and activate Steam SDK from the VREasy SDK helper window", MessageType.Error);
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.OCULUS_CONTROLLER:
                    {
#if !VREASY_OCULUS_UTILITIES_SDK
                        EditorGUILayout.HelpBox("Import Oculus Utilities and activate Oculus Utilities from the VREasy SDK helper window", MessageType.Error);
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.MOBILE_TILT:
                    {
                        locomotion.forwardAngle = EditorGUILayout.FloatField("Start movement tilt angle", locomotion.forwardAngle);
                        EditorGUILayout.LabelField("Movement based on HMD horizontal tilt, look down to start moving, look up to stop", EditorStyles.wordWrappedLabel);
                    }
                    break;
                case VRLOCOMOTION_INPUT.TRIGGER:
                    GameObject obj = locomotion.gameObject;
                    VRGrabTrigger.DisplayGrabTriggerSelector(ref locomotion.trigger, ref obj);
                    break;
                case VRLOCOMOTION_INPUT.GENERIC_VR_CONTROLLER:
                    //string axis = DisplayInputManagerAxisSelector();
                    break;
                case VRLOCOMOTION_INPUT.WAVEVR:
                    {
#if !VREASY_WAVEVR_SDK
                        EditorGUILayout.HelpBox("Import WaveVR SDK and activate WaveVR SDK from the VREasy SDK helper window", MessageType.Error);
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER:
                    {
#if !VREASY_GOOGLEVR_SDK
                        EditorGUILayout.HelpBox("Import Google VR SDK and activate it from the VREasy SDK helper window", MessageType.Error);
#endif
                    }
                    break;
            }

            // add physical embodiment
            if(locomotion.GetComponent<Collider>() == null && locomotion.GetComponent<Rigidbody>() == null)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("Physical body not detected in Locomotion object. You can make your locomotion object have a physical body by adding Rigidbody and collider components", MessageType.Info);
                if (GUILayout.Button("Add body"))
                {
                    locomotion.gameObject.AddComponent<CapsuleCollider>();
                    locomotion.gameObject.AddComponent<Rigidbody>();
                }
            } else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Physical body detected in Locomotion object. Change its physical properties in the Rigidbody and Collider components", EditorStyles.wordWrappedLabel);
            }
        }

        private static string DisplayInputManagerAxisSelector()
        {
            // TESTING//
            // manipulating InputManager http://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html
            /*SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                Debug.Log(axis.stringValue);
            }*/
            return "";
        }

        // INPUT MANAGER TESTING //
        /*private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }
        private static bool AxisDefined(string axisName)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
            return false;
        }

        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        private static void AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name)) return;

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
        }*/
    }
}