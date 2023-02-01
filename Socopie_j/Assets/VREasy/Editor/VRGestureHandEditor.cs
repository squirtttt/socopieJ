using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(VRGestureHand))]
public class VRGestureHandEditor : Editor
{
    [Serializable]
    public class Gesture_GUI
    {
        public VRGestureHand.Gesture gesture_data;
        public Editor editor;
    }

    private int selected_dropdown_option = 0;

    private string[] pose_names;

    private List<Gesture_GUI> gesture_list = new List<Gesture_GUI>();
    private List<bool> drop_down_flags = new List<bool>();

    bool is_delete_enabled = false;
    int id_to_delete = 0;

    private GameObject trigger_storage;

    private Editor thumbs_up_editor;
    private Editor point_editor;
    private Editor grip_editor;

    VRGestureHand control;

    void OnEnable()
    {
        control = (VRGestureHand)target;

        Grab_Gesture_Data();

        List<string> pose_name_list = new List<string>();

        foreach (var pose in control.Pose_Types)
        {
            pose_name_list.Add(pose.Key);
        }

        pose_names = pose_name_list.ToArray();
    }

    private void Grab_Gesture_Data()
    {
        //--------------------------------------------------
        var serializedObject = new SerializedObject(control);
        serializedObject.Update();
        gesture_list.Clear();

        foreach (var gesture in control.gestures)
        {
            Gesture_GUI temp_gesture = new Gesture_GUI();
            temp_gesture.gesture_data = gesture;
            temp_gesture.editor = CreateEditor(gesture.trigger);

            gesture_list.Add(temp_gesture);
        }



        serializedObject.ApplyModifiedProperties();
        EditorGUI.BeginChangeCheck();
        //--------------------------------------------------
    }

    public override void OnInspectorGUI()
    {
        control = (VRGestureHand)target;
        trigger_storage = control.trigger_storage;

        if (trigger_storage == null)
        {
            if (GUILayout.Button("Add Triggers"))
            {
                trigger_storage = new GameObject("Gesture Triggers");
                trigger_storage.transform.parent = control.gameObject.transform;
                control.trigger_storage = trigger_storage;
            }
        }
        else
        {
            Key_Select_Draw();
        }
    }

    private void Key_Select_Draw()
    {
        var serializedObject = new SerializedObject(control);
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        selected_dropdown_option = EditorGUILayout.Popup(selected_dropdown_option, pose_names);
        if (GUILayout.Button("Add Pose"))
        {
            VRGestureHand.Gesture temp_gesture = new VRGestureHand.Gesture();

            temp_gesture.pose_name = pose_names[selected_dropdown_option];
            temp_gesture.trigger = trigger_storage.AddComponent<VREasy.GenericControllerTrigger>();


            control.gestures.Add(temp_gesture);
            serializedObject.ApplyModifiedProperties();

            Grab_Gesture_Data();
        }
        EditorGUILayout.EndHorizontal();

        while (drop_down_flags.Count < gesture_list.Count)
        {
            drop_down_flags.Add(false);
        }

        int index = 0;
        foreach (var gesture in gesture_list)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(gesture.gesture_data.pose_name, EditorStyles.boldLabel);
            if (GUILayout.Button("Delete Pose"))
            {
                is_delete_enabled = true;
                id_to_delete = index;
            }
            EditorGUILayout.EndHorizontal();

            if (gesture.editor == null)
            {
                gesture_list[index].editor = CreateEditor(gesture_list[index].gesture_data.trigger);
            }

            // Drop down foldout
            drop_down_flags[index] = EditorGUILayout.Foldout(drop_down_flags[index], gesture.gesture_data.pose_name);
            if (drop_down_flags[index])
            {
                gesture.editor.OnInspectorGUI();
            }
            // End drop down foldout
            EditorGUILayout.EndVertical();
            index++;
        }

        if (is_delete_enabled)
        {
            DestroyImmediate(gesture_list[id_to_delete].gesture_data.trigger);
            DestroyImmediate(gesture_list[id_to_delete].editor);

            List<Gesture_GUI> temp_gesture_list = new List<Gesture_GUI>();
            List<bool> temp_dropdown_flag_list = new List<bool>();
            control.gestures.Clear();

            for (int i = 0; i < gesture_list.Count; i++)
            {
                if (i != id_to_delete)
                {
                    control.gestures.Add(gesture_list[i].gesture_data);
                    temp_gesture_list.Add(gesture_list[i]);

                    temp_dropdown_flag_list.Add(drop_down_flags[i]);
                }
            }

            gesture_list.Clear();
            gesture_list = temp_gesture_list;

            drop_down_flags.Clear();
            drop_down_flags = temp_dropdown_flag_list;

            is_delete_enabled = false;
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUI.BeginChangeCheck();
    }
}
