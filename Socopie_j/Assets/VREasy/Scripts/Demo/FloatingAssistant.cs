using UnityEngine;
using System.Collections;
#if UNITY_2017
using UnityEngine.AI;
#endif

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class FloatingAssistant : MonoBehaviour {

    public Transform hmd;
    public float distance = 1.0f;

    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        if (hmd == null)
            return;
        RaycastHit hit;
        Vector3 dest = hmd.transform.position + Vector3.right * distance; 
        if(Physics.Raycast(hmd.transform.position, dest - hmd.transform.position, out hit, distance))
        {
            dest = hmd.transform.position + Vector3.left * distance;
            if (Physics.Raycast(hmd.transform.position, dest - hmd.transform.position, out hit, distance))
            {
                dest = hmd.transform.position + Vector3.back * distance;
                if (Physics.Raycast(hmd.transform.position, dest - hmd.transform.position, out hit, distance))
                {
                    dest = hmd.transform.position + Vector3.forward * distance;
                }
            }
        }
        agent.destination = dest;

    }
}
