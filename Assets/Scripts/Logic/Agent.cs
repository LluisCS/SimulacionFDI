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
    private Transform followTarget = null;

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
        SimulationUpdate();
            
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
                    MoveToRandomExit();
                }
            }
        }

    }

    public void SubjectUpdate(string n, subjectState subState, Room room)
    {
        if (!subjects.ContainsKey(n) || state.sim == simulation.fire || state.sim == simulation.aliens)
            return;
        
        switch (subState)
        {
            case subjectState.start:
                if (state.action != agentAction.work)
                {
                    SetUp();
                    ResetTarget();
                    currentSubject = n;
                    targetRoom = room;
                    if (state.type == agentType.teacher)
                        targetSeat = room.getTeacherSeat();
                    else
                        targetSeat = room.getFirstFreeSeat();
                    targetSeat.occupied = true;
                    state.action = agentAction.enter;
                    Invoke("MoveToDestination", GetDelay());
                }
                break;
            case subjectState.active:
                if(currentSubject != n  || state.action != agentAction.work)
                {
                    SetUp();
                    ResetTarget();
                    currentSubject = n;
                    targetRoom = room;
                    if (state.type == agentType.teacher)
                        targetSeat = room.getTeacherSeat();
                    else
                        targetSeat = room.getFirstFreeSeat();
                    targetSeat.occupied = true;
                    state.action = agentAction.enter;
                    Invoke("MoveToDestination", GetDelay());
                }
                break;
            case subjectState.end:
                if(currentSubject == n)
                {
                    remainingSubjects--;
                    Room r = targetRoom;
                    ResetTarget();
                    state.action = agentAction.exit;
                    targetSeat = r.floor.getClosestSeat(transform.position);
                    if (targetSeat == null)
                        MoveToRandomExit();
                    else
                    {
                        targetSeat.occupied = true;
                        Invoke("MoveToDestination", GetDelay());
                    }
                }
                break;
            case subjectState.inactive:
                if (currentSubject == n)
                {
                    ResetTarget();
                }
                break;
            default:
                break;
        }
    }

    private void ActivityUpdate() {
        if (!state.pendingActivity)
            return;
        if(activityIndex != -1)
        {
            activity a = activities[activityIndex];
            if (state.action == agentAction.work && DayTime.Instance().activityEnded(a))
            {
                Room r = targetRoom;
                ResetTarget();
                state.action = agentAction.exit;
                targetSeat = r.floor.getClosestSeat(transform.position);
                if (targetSeat == null)
                    MoveToRandomExit();
                else
                {
                    targetSeat.occupied = true;
                    Invoke("MoveToDestination", GetDelay());
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
                        ResetTarget();
                        activityIndex = activities.IndexOf(a);
                        targetRoom = SimulationManager.Instance().GetRoom(a.roomName);
                        if (state.type == agentType.teacher)
                            targetSeat = targetRoom.getTeacherSeat();
                        else
                            targetSeat = targetRoom.getFirstFreeSeat();
                        targetSeat.occupied = true;
                        SetUp();
                        Invoke("MoveToDestination", GetDelay());
                    }
                }
            }
        }
    }


    private void ResetTarget()
    {
        targetRoom = null;
        if(targetSeat != null)
            targetSeat.occupied = false;
        targetSeat = null;
        currentSubject = "None";
        activityIndex = -1;
        navAgent.isStopped = true;
        navAgent.ResetPath();
    }

    private void EndDay()
    {
        gameObject.SetActive(false);
        state.action = agentAction.inactive;
    }

    public void StartDay()
    {
        ResetTarget();
        EndDay();
        remainingSubjects = 0;
        weekDay day = DayTime.Instance().WeekDay();
        state.pendingActivity = activities.Count > 0;
        ChangeSimulation(simulation.regular);
        gameObject.SetActive(true);
        transform.position = Vector3.zero;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void AddSubjectCount()
    {
        remainingSubjects++;
    }

    private void MoveToDestination() {
        navAgent.SetDestination(targetSeat.position);
        state.moving = true;
    }

    private void MoveToRandomExit()
    {
        navAgent.SetDestination(SimulationManager.Instance().GetRandomEntrance());
        state.moving = true;
    }

    private float GetDelay()
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
    public void TogglePause()
    {
        pause = !pause;
        navAgent.isStopped = pause;
    }
    
    public void ChangeSimulation(simulation s) {
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
                MoveToRandomExit();
                break;
            case simulation.special:
                break;
            case simulation.zombie:
                material.color = Color.black;
                state.action = agentAction.work;
                break;
            default:
                break;
        }
        state.sim = s;
        UpdateSpeed(SimulationManager.Instance().GetAgentSpeed());
    }

    private void SimulationUpdate()
    {
        switch (state.sim)
        {
            case simulation.regular:
                ActivityUpdate();
                break;
            case simulation.virus:
                ActivityUpdate();
                if (timer < Time.time)
                {
                    timer = Time.time + delay;
                    foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
                    {
                        if (Vector3.Distance(t.position, transform.position) > 5)
                            continue;
                        Agent ag = t.transform.GetComponent<Agent>();
                        if (ag.state.sim != simulation.virus)
                            ag.ChangeSimulation(simulation.virus);
                    }
                }
                break;
            case simulation.fire:
                if (timer < Time.time)
                {
                    timer = Time.time + delay;
                   
                    foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
                    {
                        if (Vector3.Distance(t.position, transform.position) > 8)
                            continue;
                        Agent ag = t.transform.GetComponent<Agent>();
                        if (ag.state.sim != simulation.fire && ag.state.sim != simulation.zombie)
                            ag.ChangeSimulation(simulation.fire);
                    }
                }
                break;
            case simulation.special:
                break;
            case simulation.zombie:
                if (timer < Time.time)
                {
                    timer = Time.time + delay;
                    float minDist = 1000;
                    Transform newTarget = null;
                    
                    foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
                    {
                        float dist = Vector3.Distance(t.transform.position, transform.position);
                        if (dist > 8)
                            continue;

                        Agent ag = t.transform.GetComponent<Agent>();

                        if (ag.state.sim == simulation.zombie)
                            continue;
                       
                        if (ag.state.sim != simulation.fire)
                            ag.ChangeSimulation(simulation.fire);
                        
                        if (dist < 1)
                            ag.ChangeSimulation(simulation.zombie);
                        else if (dist < minDist)
                        {
                            minDist = dist;
                            newTarget = ag.transform;
                        }
                    }
                    if (followTarget == null || Vector3.Distance(followTarget.position, transform.position) > minDist || followTarget.GetComponent<Agent>().state.sim == simulation.zombie) 
                        followTarget = newTarget;

                    if (followTarget != null)
                    {
                        navAgent.SetDestination(followTarget.position);
                        state.moving = true;
                    }
                    else
                    {
                        MoveToRandomExit();
                        timer += 1;
                    }
                }
                break;
            default:
                ActivityUpdate();
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Exit>() != null)
        {
            if(state.action == agentAction.inactive)
                EndDay();
        }
    }

    public void UpdateSpeed(float speed)
    {
        if (state.sim == simulation.zombie)
            speed *= 1.2f;
        navAgent.speed = speed;
    }

    private void SetUp()
    {
        if (state.action == agentAction.inactive)
        {
            state.action = agentAction.enter;
            gameObject.transform.position = SimulationManager.Instance().GetRandomEntrance();
            navAgent.speed = SimulationManager.Instance().GetAgentSpeed();
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
