using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum agentAction { inactive, enter, work, exit, relax}
public enum agentType { student, teacher, other }
public enum simulation { regular, virus, fire, special, zombie, aliens}
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
    private double timer = 0;
    public double delay = 0.8f;
    private Material material;

    void Awake()
    {
        state.moving = false;
        state.pendingActivity = true;
        state.sim = simulation.regular;
        state.type = agentType.student;
        state.action = agentAction.inactive;
        state.per = personality.standard;

        subjects = new Dictionary<string, SubjectInfo>(0);
        activities = new List<activity>(0);
        navAgent = GetComponent<NavMeshAgent>();
        material = GetComponent<Renderer>().material;
    }

    void FixedUpdate()
    {
        if (pause) return;
        //if (state.sim != simulation.regular)
        simulationUpdate();
            
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
            }
        }

    }

    public void subjectUpdate(string n, subjectState subState, Room room)
    {
        if (!subjects.ContainsKey(n) || state.sim == simulation.fire || state.sim == simulation.aliens)
            return;
        
        switch (subState)
        {
            case subjectState.start:
                if (state.action != agentAction.work)
                {
                    spawn();
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
                    spawn();
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
                        if (state.action == agentAction.inactive)
                        {
                            gameObject.transform.position = SimulationManager.Instance().getRandomEntrance();
                            navAgent.speed = SimulationManager.Instance().getAgentSpeed();
                        }
                        state.action = agentAction.enter;
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
        changeSimulation(simulation.regular);
        gameObject.SetActive(true);
        GetComponent<MeshRenderer>().enabled = false;
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
    
    public void changeSimulation(simulation s) {
        switch (s)
        {
            case simulation.regular:
                material.color = Color.blue;
                break;
            case simulation.virus:
                material.color = Color.green;
                break;
            case simulation.fire:
                material.color = Color.red;
                state.action = agentAction.inactive;
                moveToRandomExit();
                break;
            case simulation.special:
                break;
            default:
                break;
        }
        state.sim = s;
    }

    private void simulationUpdate()
    {
        switch (state.sim)
        {
            case simulation.regular:
                activityUpdate();
                break;
            case simulation.virus:
                activityUpdate();
                if (timer < Time.deltaTime)
                {
                    timer = Time.deltaTime + delay;
                    RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, transform.forward);
                    foreach (var hit in hits)
                    {
                        Agent ag = hit.transform.GetComponent<Agent>();
                        if (ag != null && ag.state.sim != simulation.virus)
                            ag.changeSimulation(simulation.virus);
                    }
                }
                break;
            case simulation.fire:
                if (timer < Time.deltaTime)
                {
                    timer = Time.deltaTime + delay;
                    RaycastHit[] hits = Physics.SphereCastAll(transform.position, 7, transform.forward);
                    foreach (var hit in hits)
                    {
                        Agent ag = hit.transform.GetComponent<Agent>();
                        if (ag != null && ag.state.sim != simulation.fire)
                            ag.changeSimulation(simulation.fire);
                    }
                }
                break;
            case simulation.special:
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Exit>() != null)
        {
            if(state.action == agentAction.inactive)
                endDay();
        }
    }

    private void spawn()
    {
        if (state.action == agentAction.inactive)
        {
            state.action = agentAction.enter;
            gameObject.transform.position = SimulationManager.Instance().getRandomEntrance();
            navAgent.speed = SimulationManager.Instance().getAgentSpeed();
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
