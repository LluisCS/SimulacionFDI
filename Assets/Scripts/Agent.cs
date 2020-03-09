using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum agentAction { inactive, enter, work, exit, relax}
public enum agentType { student, teacher, other }
public enum simulation { regular, fire, special}
[System.Serializable]
public enum personality { standard, late, early, chaotic }

public struct agentState
{
    public bool moving, pendingActivity;
    public agentAction action;
    public agentType type;
    public simulation sim;
    public personality per;
}

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    public Dictionary<string, SubjectInfo> subjects;
    public string currentSubject = "None";
    
    private NavMeshAgent navAgent;
    public agentState state;
    private Seat targetSeat = null;
    private Room targetRoom = null;
    private int remainingSubjects = 0, activityIndex = -1;
    public List<activity> activities;
    private bool pause = false;

    void Awake()
    {
        state.moving = false;
        state.pendingActivity = false;
        state.sim = simulation.regular;
        state.type = agentType.student;
        state.action = agentAction.inactive;
        state.per = personality.standard;

        subjects = new Dictionary<string, SubjectInfo>(0);
        activities = new List<activity>(0);
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (pause) return;
        activityUpdate();
        //check destination reached
        if (state.moving)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                state.moving = false;
                if (state.action == agentAction.enter)
                    state.action = agentAction.work;
                else if (state.action == agentAction.exit)
                    state.action = agentAction.relax;
            }
        }
        else
        {
            if(remainingSubjects <= 0 && !state.pendingActivity)
            {
                if(state.action == agentAction.relax)
                {
                    state.action = agentAction.inactive;
                    moveToRandomExit();
                }
                else if(state.action == agentAction.inactive)
                {
                    endDay();
                }
                
            }
        }

    }

    public void subjectUpdate(string n, subjectState subState, Room room)
    {
        if (!subjects.ContainsKey(n) || state.sim != simulation.regular)
            return;
        if (state.action == agentAction.inactive)
        {
            gameObject.SetActive(true);
            state.action = agentAction.enter;
            gameObject.transform.position = SimulationManager.Instance().getRandomEntrance();
            navAgent.speed = SimulationManager.Instance().getAgentSpeed();
        }
        switch (subState)
        {
            case subjectState.start:
                if (state.action != agentAction.work)
                {
                    resetTarget();
                    currentSubject = n;
                    targetRoom = room;
                    if (state.type == agentType.teacher)
                        targetSeat = room.getTeacherSeat();
                    else
                        targetSeat = room.getFirstFreeSeat();
                    targetSeat.occupied = true;
                    state.action = agentAction.enter;
                    Invoke("moveToDestination", getDelay());
                }
                break;
            case subjectState.active:
                if(currentSubject != n  || state.action != agentAction.work)
                {
                    resetTarget();
                    currentSubject = n;
                    targetRoom = room;
                    if (state.type == agentType.teacher)
                        targetSeat = room.getTeacherSeat();
                    else
                        targetSeat = room.getFirstFreeSeat();
                    targetSeat.occupied = true;
                    state.action = agentAction.enter;
                    Invoke("moveToDestination", getDelay());
                }
                break;
            case subjectState.end:
                if(currentSubject == n)
                {
                    remainingSubjects--;
                    Room r = targetRoom;
                    resetTarget();
                    state.action = agentAction.exit;
                    targetSeat = r.floor.getClosestSeat(transform.position);
                    if (targetSeat == null)
                        moveToRandomExit();
                    else
                    {
                        targetSeat.occupied = true;
                        Invoke("moveToDestination", getDelay());
                    }
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

    private void activityUpdate() {
        if (!state.pendingActivity)
            return;
        if(activityIndex != -1)
        {
            activity a = activities[activityIndex];
            if (state.action == agentAction.work && DayTime.Instance().activityEnded(a))
            {
                Room r = targetRoom;
                resetTarget();
                state.action = agentAction.exit;
                targetSeat = r.floor.getClosestSeat(transform.position);
                if (targetSeat == null)
                    moveToRandomExit();
                else
                {
                    targetSeat.occupied = true;
                    Invoke("moveToDestination", getDelay());
                }
            }
        }
        else if(activityIndex == -1 && currentSubject == "None")
        {
            state.pendingActivity = false;
            foreach (activity a in activities)
            {
                if (!DayTime.Instance().activityEnded(a))
                { 
                    state.pendingActivity = true;
                    if (DayTime.Instance().activityStarted(a))
                    {
                        resetTarget();
                        activityIndex = activities.IndexOf(a);
                        targetRoom = SimulationManager.Instance().GetRoom(a.roomName);
                        if (state.type == agentType.teacher)
                            targetSeat = targetRoom.getTeacherSeat();
                        else
                            targetSeat = targetRoom.getFirstFreeSeat();
                        targetSeat.occupied = true;
                        state.action = agentAction.enter;
                        if (state.action == agentAction.inactive)
                        {
                            gameObject.SetActive(true);
                            gameObject.transform.position = SimulationManager.Instance().getRandomEntrance();
                            navAgent.speed = SimulationManager.Instance().getAgentSpeed();
                        }
                        Invoke("moveToDestination", getDelay());
                    }
                }
            }
        }
    }


    private void resetTarget()
    {
        targetRoom = null;
        if(targetSeat != null)
            targetSeat.occupied = false;
        targetSeat = null;
        currentSubject = "None";
        activityIndex = -1;
    }

    private void endDay()
    {
        gameObject.SetActive(false);
        state.action = agentAction.inactive;
    }

    public void startDay()
    {
        resetTarget();
        endDay();
        remainingSubjects = 0;
        weekDay day = DayTime.Instance().WeekDay();
        state.pendingActivity = activities.Count > 0;
    }

    public void addSubjectCount()
    {
        remainingSubjects++;
    }

    private void moveToDestination() {
        navAgent.SetDestination(targetSeat.position);
        state.moving = true;
    }

    private void moveToRandomExit()
    {
        navAgent.SetDestination(SimulationManager.Instance().getRandomEntrance());
        state.moving = true;
    }

    private float getDelay()
    {
        float delay = 0.0f;
        float ran = 0;
        switch (state.per)
        {
            case personality.standard:
                ran = Random.Range(1, 3);
                break;
            case personality.late:
                ran = Random.Range(3, 6);
                break;
            case personality.early:
                ran = Random.Range(0, 2);
                break;
            case personality.chaotic:
                ran = Random.Range(0, 6);
                break;
            default:
                break;
        }
        delay += ran;
        delay = (delay * 60) / (float)DayTime.Instance().getTimeSpeed();
        return delay;
    }
    public void togglePause()
    {
        pause = !pause;
        navAgent.isStopped = pause;
       
    }
}
