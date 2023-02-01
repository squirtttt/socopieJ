using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


namespace VREasy
{
    [CustomEditor(typeof(VRGrabbable))]
    [CanEditMultipleObjects]
    public class VRGrabbableEditor : Editor
    {
        SerializedProperty grabColour;
        SerializedProperty type;
        SerializedProperty jointType;
        SerializedProperty breakForce;
        SerializedProperty breakTorque;
        SerializedProperty spring;
        SerializedProperty damper;
        SerializedProperty alignWithPivot;
        SerializedProperty moveXAxis;
        SerializedProperty moveYAxis;
        SerializedProperty moveZAxis;
        SerializedProperty minRotation;
        SerializedProperty maxRotation;
        SerializedProperty minMovement;
        SerializedProperty maxMovement;
        SerializedProperty constraintMovement;
        SerializedProperty snapToPosition;
        SerializedProperty snapToClosest;
        SerializedProperty snapToOrigin;
        SerializedProperty snapRadius;
        SerializedProperty throwingforce;
        SerializedProperty rotationWithTranslation;
        SerializedProperty translationScale;
        SerializedProperty pivot;
        SerializedProperty springToOrigin;
        SerializedProperty springSpeed;
        SerializedProperty useGrabColour;

        private void OnEnable()
        {
            grabColour = serializedObject.FindProperty("grabColour");
            type = serializedObject.FindProperty("type");
            jointType = serializedObject.FindProperty("joint_type");
            breakForce = serializedObject.FindProperty("breakForce");
            breakTorque = serializedObject.FindProperty("breakTorque");
            spring = serializedObject.FindProperty("spring");
            damper = serializedObject.FindProperty("damper");
            alignWithPivot = serializedObject.FindProperty("alignWithPivot");
            moveXAxis = serializedObject.FindProperty("xAxis");
            moveYAxis = serializedObject.FindProperty("yAxis");
            moveZAxis = serializedObject.FindProperty("zAxis");
            minRotation = serializedObject.FindProperty("minRotation");
            maxRotation = serializedObject.FindProperty("maxRotation");
            minMovement = serializedObject.FindProperty("minMovement");
            maxMovement = serializedObject.FindProperty("maxMovement");
            constraintMovement = serializedObject.FindProperty("constraintMovement");
            snapToPosition = serializedObject.FindProperty("snapToPosition");
            snapToClosest = serializedObject.FindProperty("snapToClosest");
            snapToOrigin = serializedObject.FindProperty("snapToOrigin");
            snapRadius = serializedObject.FindProperty("snapRadius");
            throwingforce = serializedObject.FindProperty("throwingForce");
            rotationWithTranslation = serializedObject.FindProperty("usePosition");
            translationScale = serializedObject.FindProperty("translationScale");
            pivot = serializedObject.FindProperty("pivot");
            springToOrigin = serializedObject.FindProperty("springToOrigin");
            springSpeed = serializedObject.FindProperty("springSpeed");
            useGrabColour = serializedObject.FindProperty("useGrabColour");
        }

        [MenuItem("VREasy/Components/VRGrabbable")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<VRGrabbable>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add a VR grabbable you must select a game object in the hierarchy first", "OK");
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

            VRGrabbable grabbable = (VRGrabbable)target;

            serializedObject.Update();
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useGrabColour);
            if (grabbable.useGrabColour)
            {
                EditorGUILayout.PropertyField(grabColour);
            }
            EditorGUILayout.PropertyField(springToOrigin);
            if(grabbable.springToOrigin) EditorGUILayout.PropertyField(springSpeed);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(type);

            
            EditorGUILayout.Separator();
            switch (grabbable.type)
            {
                case GRAB_TYPE.DRAG:
                    {
                        EditorGUILayout.PropertyField(throwingforce);
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("Joint properties", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(jointType);
                        EditorGUILayout.PropertyField(alignWithPivot);
                        switch (grabbable.joint_type)
                        {
                            case JOINT_TYPE.FIXED:
                                {
                                    EditorGUILayout.PropertyField(breakForce);
                                    EditorGUILayout.PropertyField(breakTorque);
                                }
                                break;
                            case JOINT_TYPE.SPRING:
                                {
                                    EditorGUILayout.PropertyField(breakForce);
                                    EditorGUILayout.PropertyField(breakTorque);
                                    EditorGUILayout.PropertyField(spring);
                                    EditorGUILayout.PropertyField(damper);
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.LabelField("Joint distance", EditorStyles.wordWrappedLabel);
                                    float minimumTargetRange = EditorGUILayout.FloatField("Minimum", grabbable.jointDistance.x);
                                    float maximumTargetRange = EditorGUILayout.FloatField("Maximum", grabbable.jointDistance.y);
                                    Vector2 targetValueRange = new Vector2(minimumTargetRange, maximumTargetRange);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        foreach (VRGrabbable g in targets)
                                        {
                                            Undo.RecordObject(g, "");
                                            g.jointDistance = targetValueRange;
                                        }
                                    }
                                }
                                break;
                        }
                        Rigidbody rb = grabbable.GetComponent<Rigidbody>();
                        if (rb != null && rb.collisionDetectionMode == CollisionDetectionMode.Discrete)
                        {
                            EditorGUILayout.HelpBox("Rigidbody detected, but collision detection mode is set to Discrete. Note that Continuous mode is recommended to avoid object's going through colliders when dragged", MessageType.Warning);
                            if(GUILayout.Button("Set RB to Continuous dynamic"))
                            {
                                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                            }
                        }
                        MeshCollider _collider = grabbable.GetComponent<MeshCollider>();
                        if (_collider != null && !_collider.convex)
                        {
                            EditorGUILayout.HelpBox("Unity does no longer support non-convex mesh colliders with a non kinematic Rigidbody. Dragging requires a non kinematic Rigidbody. When dragging, the Rigidbody will be temporarily set to kinematic, which may restrict the rotation of the object and its physical behaviour. ", MessageType.Warning);
                        }
                    }
                    break;
                case GRAB_TYPE.SLIDE:

                case GRAB_TYPE.ROTATE:
                    {
                        EditorGUILayout.LabelField("Grab controls", EditorStyles.boldLabel);
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("If None, rotation / translation is applied to self",EditorStyles.wordWrappedLabel);
                        EditorGUILayout.PropertyField(pivot);
                        EditorGUILayout.Separator();

                        if (grabbable.type == GRAB_TYPE.SLIDE)
                        {
                            EditorGUILayout.LabelField("Sliding axis (local)", EditorStyles.boldLabel);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(rotationWithTranslation);
                            if(grabbable.usePosition)
                            {
                                EditorGUILayout.PropertyField(translationScale);
                                EditorGUILayout.HelpBox("The rotation of the grabbed object will be controlled by changes in the POSITION of the selector controller", MessageType.Info);
                            } else
                            {

                                EditorGUILayout.HelpBox("The rotation of the grabbed object will be controlled by changes in the ROTATION of the selector controller", MessageType.Info);
                            }
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("Rotating axis", EditorStyles.boldLabel);
                        }
                        EditorGUILayout.PropertyField(moveXAxis);
                        EditorGUILayout.PropertyField(moveYAxis);
                        EditorGUILayout.PropertyField(moveZAxis);
                        
                        EditorGUILayout.Separator();
                        EditorGUILayout.PropertyField(constraintMovement);
                        if (grabbable.constraintMovement)
                        {
                            if (grabbable.type == GRAB_TYPE.ROTATE)
                            {
                                if (grabbable.xAxis && !grabbable.yAxis && !grabbable.zAxis || !grabbable.xAxis && grabbable.yAxis && !grabbable.zAxis || !grabbable.xAxis && !grabbable.yAxis && grabbable.zAxis)
                                {
                                    if (grabbable.type == GRAB_TYPE.ROTATE)
                                    {
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(minRotation);
                                        if(GUILayout.Button("Same as current"))
                                        {
                                            grabbable.minRotation = grabbable.GetCurrentAxisValue();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PropertyField(maxRotation);
                                        if (GUILayout.Button("Same as current"))
                                        {
                                            grabbable.maxRotation = grabbable.GetCurrentAxisValue();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUI.indentLevel--;
                                    }
                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("Select single axis to constraint rotation", MessageType.Warning);
                                }
                            }
                            else if (grabbable.type == GRAB_TYPE.SLIDE)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(minMovement);
                                EditorGUILayout.PropertyField(maxMovement);
                                EditorGUI.indentLevel--;
                            }
                        } 

                    }
                    break;
            }

            
            EditorGUILayout.Separator();
            if (grabbable.type == GRAB_TYPE.ROTATE)
            {
                grabbable.snapToPosition = false;
            }
            else
            {
                // Snap positions properties
                EditorGUILayout.PropertyField(snapToPosition);
                if (snapToPosition.boolValue != grabbable.snapToPosition)
                    SceneView.RepaintAll();
                if (grabbable.snapToPosition)
                {
                    EditorGUILayout.PropertyField(snapToClosest);
                    EditorGUILayout.PropertyField(snapRadius);
                    if (snapRadius.floatValue != grabbable.snapRadius)
                    {
                        if (Mathf.Abs(snapRadius.floatValue - grabbable.snapRadius) > 0.001f)
                        {
                            SceneView.RepaintAll();
                        }
                    }
                    if(targets.Length > 1)
                    {
                        EditorGUILayout.HelpBox("Multi-object editing not supported for snap positions", MessageType.Error);
                    } else
                    {
                        EditorGUILayout.PropertyField(snapToOrigin);
                        if(grabbable.snapPositions.Count > 0) EditorGUILayout.LabelField("Snapping positions");
                        
                        for (int ii = 0; ii < grabbable.snapPositions.Count; ii++)
                        {
                            if (grabbable.snapPositions[ii] == null) continue;
                            grabbable.snapPositions[ii] = (Transform)EditorGUILayout.ObjectField("Point" + ii, grabbable.snapPositions[ii], typeof(Transform), true);
                            EditorGUILayout.BeginHorizontal();
                            grabbable.snapPositions[ii].position = EditorGUILayout.Vector3Field("", grabbable.snapPositions[ii].position);
                            
                            if(GUILayout.Button("Locate"))
                            {
                                Selection.activeGameObject = grabbable.snapPositions[ii].gameObject;
                            }
                            if (GUILayout.Button("Remove"))
                            {
                                DestroyImmediate(grabbable.snapPositions[ii].gameObject);
                                grabbable.snapPositions.RemoveAt(ii);
                                if (grabbable.snapPositions.Count == 0) {
                                    GameObject parent = GameObject.Find(getSnapParentName(grabbable.gameObject.name));
                                    DestroyImmediate(parent);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Separator();
                        }
                        if (GUILayout.Button("Add snap position"))
                        {
                            GameObject g = new GameObject("point" + grabbable.snapPositions.Count);
                            GameObject parent = GameObject.Find(getSnapParentName(grabbable.gameObject.name));
                            if (parent == null)
                            {
                                parent = new GameObject(getSnapParentName(grabbable.gameObject.name));
                            }
                            g.transform.parent = parent.transform;
                            // set position equals to last point
                            if (grabbable.snapPositions.Count > 0 && grabbable.snapPositions[grabbable.snapPositions.Count - 1] != null)
                            {
                                g.transform.position = grabbable.snapPositions[grabbable.snapPositions.Count - 1].position;
                            }
                            else
                            {
                                g.transform.position = grabbable.transform.position;
                            }
                            grabbable.snapPositions.Add(g.transform);
                            
                        }
                    }
                    

                }
            }
            if (!Application.isPlaying)
            {
                foreach (Object obj in targets)
                {
                    VRGrabbable g = (VRGrabbable)obj;
                    g.SetOrigins();
                }

            }

            
            // display begin and end grab actions
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Actions to trigger", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("[On start grab]", EditorStyles.boldLabel);
            VRSelectableEditor.DisplayActionList(grabbable.StartGrabActions,targets);

            // display exit trigger actionlist
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("[On end grab]", EditorStyles.boldLabel);
            VRSelectableEditor.DisplayActionList(grabbable.EndGrabActions,targets);
            

            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Separator();
            if (GUILayout.Button("Remove component"))
            {
                if (EditorUtility.DisplayDialog("VREasy: Remove Grabbable", "You are about to remove the Grabbable component. Would you like to remove the actions attached to it?", "Yes", "No"))
                {
                    foreach (VRGrabbable g in targets)
                    {
                        DestroyImmediate(g.EndGrabActions.gameObject);
                        DestroyImmediate(g.StartGrabActions.gameObject);
                    }
                }
                List<int> list = new List<int>();
                for(int ii=0; ii < targets.Length; ii++)
                {
                    list.Add(ii);
                }
                for(int ii=list.Count-1; ii >= 0; ii--)
                {
                    VRGrabbable g = (VRGrabbable)targets[list[ii]];
                    DestroyImmediate(g);
                }
                GUIUtility.ExitGUI();

            }

        }

        private static string getSnapParentName(string gameObjectName)
        {
            return "[VREasy]SnapPositions " + gameObjectName;
        }
    }
}

