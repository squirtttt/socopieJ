using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace VREasy
{
    [CustomEditor(typeof(PointerSelector))]
    public class PointerSelectorEditor : Editor
    {
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

            PointerSelector pointer = (PointerSelector)target;

            ConfigurePointerSelector(pointer);
        }

        public static void ConfigurePointerSelector(PointerSelector pointer)
        {
            LOSSelector los = pointer;
            LOSSelectorEditor.ConfigureLOSSelector(ref los);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Line options",EditorStyles.boldLabel);
            LineRenderer line = pointer.GetComponent<LineRenderer>();
            if(line.sharedMaterial == null)
            {
                EditorGUILayout.LabelField("Please assign a material to the Line Renderer before changing its properties");
            } else
            {
                EditorGUI.BeginChangeCheck();
                Color col = EditorGUILayout.ColorField("Main colour", pointer.MainColour);
                Color hoverCol = EditorGUILayout.ColorField("Active colour", pointer.hoverColour);
                float width = EditorGUILayout.Slider("Line width", pointer.LineWidth,0.001f,0.2f);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(pointer, "Pointer renderer options");
                    pointer.MainColour = col;
                    pointer.LineWidth = width;
                    pointer.hoverColour = hoverCol;
                }
            }

            EditorGUILayout.Separator();
            
            EditorGUILayout.LabelField("Interactive options", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            bool useReticle = EditorGUILayout.Toggle("Use reticle", pointer.useReticle);
            Color reticleColour = pointer.reticleColour;
            float reticleScale = pointer.reticleScale;
            if (useReticle)
            {
                reticleColour = EditorGUILayout.ColorField("Colour", pointer.reticleColour);
                reticleScale = EditorGUILayout.Slider("Scale", pointer.reticleScale, 0.01f, 0.2f);
            }
            bool usePassiveBeam = EditorGUILayout.Toggle("Use passive beam", pointer.usePassiveBeam);
            Color beamColour = pointer.beamColour;
            if (usePassiveBeam)
            {
                beamColour = EditorGUILayout.ColorField("Beam colour", pointer.beamColour);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pointer, "Reticle properties");
                pointer.useReticle = useReticle;
                pointer.usePassiveBeam = usePassiveBeam;
                pointer.reticleColour = reticleColour;
                pointer.reticleScale = reticleScale;
                pointer.beamColour = beamColour;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Activation", EditorStyles.boldLabel);

            VRSelector sel = pointer;
            VRSelectorEditor.ConfigureSelector(ref sel);

            EditorGUI.BeginChangeCheck();
            float selectionDistance = EditorGUILayout.DelayedFloatField("Selection distance", pointer.selectionDistance);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pointer, "Pointer selector settings");
                pointer.selectionDistance = selectionDistance;
            }

            GameObject obj = pointer.gameObject;
            VRGrabTrigger.DisplayGrabTriggerSelector(ref pointer.grabTrigger, ref obj);
        }
    }
}