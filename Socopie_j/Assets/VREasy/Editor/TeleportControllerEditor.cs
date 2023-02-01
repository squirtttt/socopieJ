using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(TeleportController))]
    public class TeleportControllerEditor : Editor
    {
        [MenuItem("VREasy/Components/Teleport controller")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<TeleportController>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add a TeleportController you must select a game object in the hierarchy first", "OK");
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

            TeleportController teleport = (TeleportController)target;

            TeleportActionEditor.ConfigureTeleportAction(teleport.Teleport);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Controller settings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            float playerHeightCorrection = EditorGUILayout.FloatField("Player height correction", teleport._playerHeightCorrection);
            EditorGUILayout.HelpBox("Use this value to set the expected headset height (when teleporting, this is the height difference that will be used between the floor and the headset)", MessageType.Info);
            EditorGUILayout.Separator();

            TELEPORT_CONTROLLER_BEAM beam = (TELEPORT_CONTROLLER_BEAM)EditorGUILayout.EnumPopup("Beam type", teleport.beam);
            float reach = teleport.reach; ;
            float maxStepDown = teleport.maxStepDown;
            float max_straight_distance = teleport.max_straight_distance;
            switch(beam)
            {
                case TELEPORT_CONTROLLER_BEAM.PARABOLE:
                    {
                        reach = EditorGUILayout.Slider("Parabole reach", teleport.reach, 1.0f, 4.0f);
                        maxStepDown = Mathf.Clamp(EditorGUILayout.FloatField("Max step down", teleport.maxStepDown), 5f, Mathf.Infinity);
                    }
                    break;
                case TELEPORT_CONTROLLER_BEAM.STRAIGHT:
                    {
                        max_straight_distance = EditorGUILayout.FloatField("Max line distance", teleport.max_straight_distance);
                    }
                    break;
            }
            
            LayerMask layers = EditorGUILayout.LayerField("Walkable layers", teleport.walkableLayers);

            GameObject obj = teleport.gameObject;
            VRGrabTrigger.DisplayGrabTriggerSelector(ref teleport.trigger, ref obj);
            EditorGUILayout.LabelField("Configure trigger in [" + teleport.name + "]'s inspector");
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Line settings", EditorStyles.boldLabel);

            TELEPORT_CONTROLLER_RENDERER type = (TELEPORT_CONTROLLER_RENDERER)EditorGUILayout.EnumPopup("Render type", teleport.type);

            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(teleport, "Teleport settings changed");
                teleport.walkableLayers = layers;
                teleport.reach = reach;
                teleport.maxStepDown = maxStepDown;
                teleport.type = type;
                teleport.beam = beam;
                teleport.max_straight_distance = max_straight_distance;
                teleport._playerHeightCorrection = playerHeightCorrection;
            }
            if (type != teleport.type)
            {
                teleport.ResetRenderers();
            }

            // Common properties
            EditorGUI.BeginChangeCheck();
            float thickness = EditorGUILayout.FloatField("Line thickness", teleport.LineThickness);
            float step = EditorGUILayout.FloatField("Path step", teleport.step);
            EditorGUILayout.HelpBox("Lower step values lead to higher resolution paths (more costly)", MessageType.Info);
            Sprite landingSprite = (Sprite)EditorGUILayout.ObjectField("Landing sprite", teleport.landingSprite, typeof(Sprite), true);
            float landingSize = EditorGUILayout.FloatField("Landing size", teleport.landingSize);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(teleport, "Teleport general settings changed");
                teleport.LineThickness = thickness;
                teleport.landingSprite = landingSprite;
                teleport.landingSize = landingSize;
                teleport.step = step;
            }

            // Specific properties
            EditorGUILayout.Separator();
            switch (teleport.type)
            {
                case TELEPORT_CONTROLLER_RENDERER.LINE:
                    {
                        EditorGUI.BeginChangeCheck();
                        Color validColour = EditorGUILayout.ColorField("Valid path", teleport.validMoveColour);
                        Color invalidColour = EditorGUILayout.ColorField("Invalid path", teleport.invalidMoveColour);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(teleport, "Teleport line settings changed");
                            teleport.validMoveColour = validColour;
                            teleport.invalidMoveColour = invalidColour;
                        }
                    }
                    break;
                case TELEPORT_CONTROLLER_RENDERER.TEXTURES:
                    {
                        EditorGUI.BeginChangeCheck();
                        Texture2D validTexture = (Texture2D)EditorGUILayout.ObjectField("Valid path", teleport.validTexture,typeof(Texture2D),true);
                        Texture2D invalidTexture = (Texture2D)EditorGUILayout.ObjectField("Invalid path", teleport.invalidTexture, typeof(Texture2D), true);
                        float scrollSpeed = EditorGUILayout.FloatField("Texture scroll speed", teleport.ScrollSpeed);

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(teleport, "Teleport line settings changed");
                            teleport.validTexture = validTexture;
                            teleport.invalidTexture = invalidTexture;
                            teleport.ScrollSpeed = scrollSpeed;
                        }
                    }
                    break;

            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Remove component"))
            {
                if (EditorUtility.DisplayDialog("VREasy: Remove Teleport controller", "You are about to remove the Teleport component. Would you like to remove the scripts associated with it? (TeleportAction, Trigger)", "Yes", "No"))
                {
                    DestroyImmediate(teleport.trigger);
                    DestroyImmediate(teleport);
                    DestroyImmediate(teleport.Teleport);
                }
                GUIUtility.ExitGUI();

            }

        }
    }
}