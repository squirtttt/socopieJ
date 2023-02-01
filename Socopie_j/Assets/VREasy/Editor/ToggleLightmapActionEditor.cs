using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(ToggleLightmapAction))]
    public class ToggleLightmapActionEditor : Editor
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

            ToggleLightmapAction toggleLightmap = (ToggleLightmapAction)target;

            
            if ((toggleLightmap.Day_near_dir.Count != toggleLightmap.Day_far_light.Count) || (toggleLightmap.Night_near_dir.Count != toggleLightmap.Night_far_light.Count))
            {
                EditorGUILayout.HelpBox("Far and Near sets must be of the same length", MessageType.Error);
                EditorGUILayout.Separator();
            }

            EditorGUI.BeginChangeCheck();
            LIGHTMAP_STATE lightState = (LIGHTMAP_STATE)EditorGUILayout.EnumPopup("Default lightmap", toggleLightmap.lightmapState);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(toggleLightmap, "Changed default lightmap");
                toggleLightmap.lightmapState = lightState;
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("LIGHTMAP 1", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Near (directional) lightmaps", EditorStyles.boldLabel);
            showEditableList(toggleLightmap.Day_near_dir,toggleLightmap);
            EditorGUILayout.LabelField("Far (light) lightmaps", EditorStyles.boldLabel);
            showEditableList(toggleLightmap.Day_far_light, toggleLightmap);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("LIGHTMAP 2", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Near (directional) lightmaps", EditorStyles.boldLabel);
            showEditableList(toggleLightmap.Night_near_dir, toggleLightmap);
            EditorGUILayout.LabelField("Far (light) lightmaps", EditorStyles.boldLabel);
            showEditableList(toggleLightmap.Night_far_light, toggleLightmap);
            EditorGUI.indentLevel--;

            // add lightmaps
            bool addSlotDay = false;
            bool addSlotNight = false;
            EditorGUI.BeginChangeCheck();
            Handles.BeginGUI();
            if (GUILayout.Button("Add Lightmap 1 slot"))
            {
                addSlotDay = true;
            }
            if (GUILayout.Button("Add Lightmap 2 slot"))
            {
                addSlotNight = true;
            }
            Handles.EndGUI();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(toggleLightmap, "Changed lightmap objects");
                if (addSlotDay)
                {
                    toggleLightmap.Day_near_dir.Add(null);
                    toggleLightmap.Day_far_light.Add(null);
                    EditorGUIUtility.ExitGUI();
                }
                if (addSlotNight)
                {
                    toggleLightmap.Night_near_dir.Add(null);
                    toggleLightmap.Night_far_light.Add(null);
                    EditorGUIUtility.ExitGUI();
                }
            }
        }

        private void showEditableList(List<Texture2D> list, ToggleLightmapAction toggle)
        {
            EditorGUI.BeginChangeCheck();
            int index = -1;
            for (int ii = 0; ii < list.Count; ii++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("" + (ii + 1));
                list[ii] = (Texture2D)EditorGUILayout.ObjectField(list[ii], typeof(Texture2D), true);
                Handles.BeginGUI();
                if (GUILayout.Button("-"))
                {
                    index = ii;
                }
                Handles.EndGUI();
                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(toggle, "Changed lightmap objects");
                if (index >= 0)
                {
                    list.RemoveAt(index);
                    EditorGUIUtility.ExitGUI();
                }
            }
        }

    }
}