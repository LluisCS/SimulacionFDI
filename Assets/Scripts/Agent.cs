using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum agentState { inactive, enter, work, exit, relax, other}
[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    public Dictionary<string, SubjectInfo> subjects;
    public string currentSubject = "None";
    
    private NavMeshAgent navAgent;
    public agentState state;
    private Seat targetSeat = null;
    private Room targetRoom = null;
    private string simulation = "Regular";
    private bool moving = false;
    public int remainingSubjects = 0;

    void Awake()
    {
        state = agentState.inactive;
        subjects = new Dictionary<string, SubjectInfo>(0);
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (moving)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                moving = false;
                if (state == agentState.enter)
                    state = agentState.work;
                else if (state == agentState.exit)
                    state = agentState.relax;
            }
        }
        else
        {
            if(remainingSubjects <= 0)
            {
                if(state == agentState.relax)
                {
                    state = agentState.other;
                    navAgent.SetDestination(SimulationManager.Instance().getRandomEntrance());
                    moving = true;
                }
                else if(state == agentState.other)
                {
                    endDay();
                }
                
            }
        }

    }

    public void subjectUpdate(string n, subjectState subState, Room room)
    {
        if (!subjects.ContainsKey(n) || simulation != "Regular")
            return;
        if (state == agentState.inactive)
        {
            gameObject.SetActive(true);
            state = agentState.enter;
            gameObject.transform.position = SimulationManager.Instance().getRandomEntrance();
        }
        switch (subState)
        {
            case subjectState.active:
                if(currentSubject != n  || state != agentState.work)
                {
                    resetTarget();
                    currentSubject = n;
                    targetRoom = room;
                    targetSeat = room.getFirstFreeSeat();
                    targetSeat.occupied = true;
                    navAgent.SetDestination(targetSeat.position);
                    state = agentState.enter;
                    moving = true;
                }
                break;
            case subjectState.start:
                if(state != agentState.work )
                {
                    //Debug.Log(name + " started " + n);
                    resetTarget();
                    currentSubject = n;
                    targetRoom = room;
                    targetSeat = room.getFirstFreeSeat();
                    targetSeat.occupied = true;
                    navAgent.SetDestination(targetSeat.position);
                    state = agentState.enter;
                    moving = true;
                }    
                break;
            
            case subjectState.end:
                if(currentSubject == n)
                {
                    remainingSubjects--;
                    Room r = targetRoom;
                    resetTarget();
                    state = agentState.exit;
                    targetSeat = r.floor.getClosestSeat(transform.position);
                    if (targetSeat == null)
                        navAgent.SetDestination(SimulationManager.Instance().getRandomEntrance());
                    else
                    {
                        navAgent.SetDestination(targetSeat.position);
                        targetSeat.occupied = true;
                    }
                    moving = true;
                }
                break;
            case subjectState.inactive:
                if (currentSubject == n)
                {
                    resetTarget();
                }
                break;
            default:
                break;
        }
    }

    private void resetTarget()
    {
        targetRoom = null;
        if(targetSeat != null)
            targetSeat.occupied = false;
        targetSeat = null;
        currentSubject = "None";
    }

    private void endDay()
    {
        gameObject.SetActive(false);
        state = agentState.inactive;
    }

    public void startDay()
    {
        resetTarget();
        endDay();
        remainingSubjects = 0;
    }

    public void addSubjectCount()
    {
        remainingSubjects++;
    }
}
