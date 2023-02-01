using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRGestureHand : MonoBehaviour
{
    [Serializable]
    public struct Gesture
    {
        public VREasy.GenericControllerTrigger trigger;
        public string pose_name;
    }

    public Dictionary<string, Hand_Pose> Pose_Types = new Dictionary<string, Hand_Pose>()
    {
            {"Index Point",new Hand_Pose{
            pose_weight = 1.0f,
            thumb_state = Finger_State.NULL,
            index_state = Finger_State.UP,
            middle_state = Finger_State.NULL,
            ring_state = Finger_State.NULL,
            pinky_state = Finger_State.NULL,

        } },
            {"Thumb Up",new Hand_Pose{
            pose_weight = 1.0f,
            thumb_state = Finger_State.UP,
            index_state = Finger_State.NULL,
            middle_state = Finger_State.NULL,
            ring_state = Finger_State.NULL,
            pinky_state = Finger_State.NULL,
        } },
            {"Grip",new Hand_Pose{
            pose_weight = 1.0f,
            thumb_state = Finger_State.NULL,
            index_state = Finger_State.NULL,
            middle_state = Finger_State.CLOSED,
            ring_state = Finger_State.CLOSED,
            pinky_state = Finger_State.CLOSED,
        } },
            {"Middle Point",new Hand_Pose{
            pose_weight = 1.0f,
            thumb_state = Finger_State.NULL,
            index_state = Finger_State.NULL,
            middle_state = Finger_State.UP,
            ring_state = Finger_State.NULL,
            pinky_state = Finger_State.NULL,
        } },
            {"Open Hand",new Hand_Pose{
            pose_weight = 1.0f,
            thumb_state = Finger_State.UP,
            index_state = Finger_State.UP,
            middle_state = Finger_State.UP,
            ring_state = Finger_State.UP,
            pinky_state = Finger_State.UP,
        } },
            {"Closed Hand",new Hand_Pose{
            pose_weight = 1.0f,
            thumb_state = Finger_State.CLOSED,
            index_state = Finger_State.CLOSED,
            middle_state = Finger_State.CLOSED,
            ring_state = Finger_State.CLOSED,
            pinky_state = Finger_State.CLOSED,
        } },
            {"Grab",new Hand_Pose{
            pose_weight = 0.7f,
            thumb_state = Finger_State.CLOSED,
            index_state = Finger_State.CLOSED,
            middle_state = Finger_State.CLOSED,
            ring_state = Finger_State.CLOSED,
            pinky_state = Finger_State.CLOSED,
        } },
    };

    public List<Gesture> gestures = new List<Gesture>();
    public float weighting = 1.0f;

    private enum HAND_TYPE { RIGHT, LEFT };

    [SerializeField] public GameObject trigger_storage;

    public enum Finger_State {IDLE,CLOSED,UP,NULL };

    // Individual finger bools
    private Finger_State thumb_state = Finger_State.IDLE;
    private Finger_State index_state = Finger_State.IDLE;
    private Finger_State middle_state = Finger_State.IDLE;
    private Finger_State ring_state = Finger_State.IDLE;
    private Finger_State pinky_state = Finger_State.IDLE;
    //

    public struct Hand_Pose
    {
        public float pose_weight;
        public Finger_State thumb_state;
        public Finger_State index_state;
        public Finger_State middle_state;
        public Finger_State ring_state;
        public Finger_State pinky_state;
    };

    private Animator anim;

    private bool is_in_idle_state = true;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

        Set_Weighting(1.0f);

    }

    private void OnEnable()
    {
        if (trigger_storage == null)
        {
            Debug.LogWarning("Triggers for the VR Gesture Hand have not been added. Please add the triggers in the editor to prevent errors");
            enabled = false;
        }
    }

    private void Set_Weighting(float i_weight)
    {
        anim.SetLayerWeight(1, i_weight);
        anim.SetLayerWeight(2, i_weight);
        anim.SetLayerWeight(3, i_weight);
        anim.SetLayerWeight(4, i_weight);
        anim.SetLayerWeight(5, i_weight);

        weighting = i_weight;
    }

    private Hand_Pose Check_Gesture_List()
    {
        Hand_Pose current_pose = new Hand_Pose();

        current_pose.thumb_state = Finger_State.CLOSED;
        current_pose.index_state = Finger_State.CLOSED;
        current_pose.middle_state = Finger_State.CLOSED;
        current_pose.ring_state = Finger_State.CLOSED;
        current_pose.pinky_state = Finger_State.CLOSED;


        foreach (var gesture in gestures)
        {
            
            if (Pose_Types.ContainsKey(gesture.pose_name))
            {

                if (gesture.trigger.Triggered())
                {
                    current_pose.pose_weight = Pose_Types[gesture.pose_name].pose_weight;
                    if (Pose_Types[gesture.pose_name].thumb_state != Finger_State.NULL)
                    {
                        current_pose.thumb_state = Pose_Types[gesture.pose_name].thumb_state;
                    }
                    if (Pose_Types[gesture.pose_name].index_state != Finger_State.NULL)
                    {
                        current_pose.index_state = Pose_Types[gesture.pose_name].index_state;
                    }
                    if (Pose_Types[gesture.pose_name].middle_state != Finger_State.NULL)
                    {
                        current_pose.middle_state = Pose_Types[gesture.pose_name].middle_state;
                    }
                    if (Pose_Types[gesture.pose_name].ring_state != Finger_State.NULL)
                    {
                        current_pose.ring_state = Pose_Types[gesture.pose_name].ring_state;
                    }
                    if (Pose_Types[gesture.pose_name].pinky_state != Finger_State.NULL)
                    {
                        current_pose.pinky_state = Pose_Types[gesture.pose_name].pinky_state;
                    }

                    is_in_idle_state = false;
                }
            }
            else
            {
                Debug.LogWarning("Pose name not found");
            }
        }

        return current_pose;
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger_storage != null)
        {
            is_in_idle_state = true;

            Hand_Pose final_pose = Check_Gesture_List();

            // Either sets to final hand pose or the idle state
            if (is_in_idle_state)
            {
                Hand_Pose idle_pose = new Hand_Pose();

                idle_pose.thumb_state = Finger_State.IDLE;
                idle_pose.index_state = Finger_State.IDLE;
                idle_pose.middle_state = Finger_State.IDLE;
                idle_pose.ring_state = Finger_State.IDLE;
                idle_pose.pinky_state = Finger_State.IDLE;
                idle_pose.pose_weight = 1.0f;

                Pose_Hand(idle_pose);
            }
            else
            {
                Pose_Hand(final_pose);
            }
        }
    }

    private void Pose_Hand(Hand_Pose pose)
    {
        Set_Weighting(Mathf.Lerp(weighting,pose.pose_weight,0.2f));

        if (thumb_state != pose.thumb_state)
        {
            thumb_state = pose.thumb_state;
            Set_Advanced_Finger_State("Thumb", thumb_state);
        }
        if (index_state != pose.index_state)
        {
            index_state = pose.index_state;
            Set_Advanced_Finger_State("Index", index_state);
        }

        if (ring_state != pose.ring_state)
        {
            ring_state = pose.ring_state;
            Set_Advanced_Finger_State("Ring", ring_state);
        }
        if (middle_state != pose.middle_state)
        {
            middle_state = pose.middle_state;
            Set_Advanced_Finger_State("Middle", middle_state);
        }
        if (pinky_state != pose.pinky_state)
        {
            pinky_state = pose.pinky_state;
            Set_Advanced_Finger_State("Pinky", pinky_state);
        }
    }

    private void Set_Finger_State(string finger, bool state)
    {
        if (state)
        {
            anim.Play(finger + "_Close");
        }
        else
        {
            anim.Play(finger + "_Open");
        }
    }

    private void Set_Advanced_Finger_State(string finger, Finger_State state)
    {
        
        if (state == Finger_State.CLOSED)
        {
            anim.SetInteger(finger + "State", (int)(Finger_State.CLOSED));
        }
        else if (state == Finger_State.IDLE)
        {
            anim.SetInteger(finger + "State", (int)(Finger_State.IDLE));
        }
        else if (state == Finger_State.UP)
        {
            anim.SetInteger(finger + "State", (int)(Finger_State.UP));
        }
    }
}
