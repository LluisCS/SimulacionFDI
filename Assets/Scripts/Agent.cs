using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum agentState { inactive, enter, work, exit, relax, other}
[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    private NavMeshAgent agentComp;
    private agentState state = agentState.inactive;
    public List<SubjectInfo> subjects;
    public string currentFloor, currentRoom, objectiveRoom;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void subjectUpdate(string name, subjectState state, Room room)
    {

    }
}
