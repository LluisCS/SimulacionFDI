using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum agentAction { inactive, enter, work, exit, relax}
public enum agentType { student, teacher, other }
public enum simulation { regular, infection, fire, special, zombie, aliens}
[System.Serializable]
public enum personality { standard, late, early, chaotic }

//struct with the core information required to know the state of an agent
public struct agentState
{
    public bool moving, pendingActivity;
    public agentAction action;
    public agentType type;
    public simulation sim;
    public personality per;
}

/*Component that controls the IA of every characters of the simulation.
 Has specific methods and data structures to manage a schedule of subjects and other activities in the faculty.
 Depends on the Unity navMeshAgent component for movement around the building(pathfinding).
*/
[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    public Dictionary<string, SubjectInfo> subjects;//Name of every subject of this agent with a reference to its information
    public string currentSubject = "None";

    public List<activity> activities;
    public agentState state;
    public double delay = 0.8f;

    private NavMeshAgent navAgent;//NavMeshAgent component reference
    private Seat targetSeat = null;
    private Room targetRoom = null;
    private int remainingSubjects = 0, activityIndex = -1;
    private bool pause = false;
    public bool initedDay = false;
    private double timer = 0;
    public bool canInfect = false;
    
    private Material material;
    private Transform followTarget = null;

    //called on scene start
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

    //called every loop
    void FixedUpdate()
    {
        if (pause) return;
        //if (state.sim != simulation.regular)
        SimulationUpdate();
            
        //check destination reached
        if (state.moving)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance < navAgent.stoppingDistance)
            {
                state.moving = false;
                //navAgent.ResetPath();
                if (state.action == agentAction.enter)
                    state.action = agentAction.work;
                else if (state.action == agentAction.exit)
                    state.action = agentAction.relax;

                if (targetRoom != null)
                    transform.LookAt(targetRoom.entrance);

            }
        }
        //check no more pending tasks
        else
        {
            if(remainingSubjects <= 0 && !state.pendingActivity)
            {
                if(state.action == agentAction.relax)
                {
                    ResetTarget();
                    state.action = agentAction.inactive;
                    MoveToRandomExit();
                }
            }
        }

    }

    //Updates agent state and actions according to the state of its subjects
    public void SubjectUpdate(string n, subjectState subState, Room room)
    {
        if (!subjects.ContainsKey(n) || state.sim == simulation.fire || state.sim == simulation.zombie)
            return;
        
        switch (subState)
        {
            case subjectState.start:
                if (state.action != agentAction.work)
                {
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
                if( currentSubject == "None" || (state.action != agentAction.work && state.action != agentAction.enter) )
                {
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
                    Debug.Log("hehe");
                }
                break;
            case subjectState.end:
                if(currentSubject == n)
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
                break;
            case subjectState.inactive:
                remainingSubjects--;
                if (currentSubject == n)
                {
                    ResetTarget();
                }
                break;
            default:
                break;
        }
    }

    //Updates agent state and actions according to the activities
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
                        state.action = agentAction.enter;
                        Invoke("MoveToDestination", GetDelay());
                    }
                }
            }
        }
    }

    //Cleans all paths and remaining tasks
    private void ResetTarget()
    {
        targetRoom = null;
        if(targetSeat != null)
            targetSeat.occupied = false;
        targetSeat = null;
        currentSubject = "None";
        activityIndex = -1;
        //navAgent.isStopped = true;
        //navAgent.ResetPath();
    }
    //Process end of day
    public void EndDay()
    {
        if (initedDay)
            LogSystem.Instance().Log(name + " abandoned the faculty ", logType.exit);

        gameObject.SetActive(false);
        canInfect = false;
        initedDay = false;
        ChangeSimulation(simulation.regular);
        ResetTarget();
    }
    //Process start of day
    public void StartDay()
    {
        ResetTarget();
        state.action = agentAction.inactive;
        remainingSubjects = 0;
        weekDay day = DayTime.Instance().WeekDay();
        state.pendingActivity = activities.Count > 0;
        gameObject.SetActive(true);
        transform.position = Vector3.zero;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        
    }

    public void AddSubjectCount()
    {
        remainingSubjects++;
    }
    
    //Initiates the character movement to a previously designated target
    private void MoveToDestination() {
        if (targetSeat == null || state.sim == simulation.fire || state.sim == simulation.zombie) return;
        SetUp();
        navAgent.SetDestination(targetSeat.position);
        state.moving = true;
        if (canInfect && !SimulationManager.Instance().infectionInfo.safe)
        {
            GameObject ent = Instantiate(SimulationManager.Instance().infectionInfo.prefab);
            ent.transform.position = transform.position;
            ent.transform.SetParent(SimulationManager.Instance().infectionInfo.parentObject.transform);
        }

    }

    //Initiates the character movements to a randomly chosen exit of the building
    private void MoveToRandomExit()
    {
        if (!gameObject.activeSelf) return;
        navAgent.SetDestination(SimulationManager.Instance().GetRandomEntrance());
        state.moving = true;
    }

    //Obtains a random delay for agent actions or movements depending on its personality
    private float GetDelay()
    {
        float ran = 0;
        switch (state.per)
        {
            case personality.standard:
                ran = Random.Range(1, 4);
                break;
            case personality.late:
                ran = Random.Range(3, 6);
                break;
            case personality.early:
                ran = Random.Range(0, 3);
                break;
            case personality.chaotic:
                ran = Random.Range(0, 7);
                break;
            default:
                break;
        }
        float delay = (ran * 60.0f) / (float)DayTime.Instance().getTimeSpeed();
        return delay;
    }
    public void TogglePause()
    {
        pause = !pause;
        navAgent.isStopped = pause;
    }
    //Changes agent state with a different simulation event
    public void ChangeSimulation(simulation s) {
        switch (s)
        {
            case simulation.regular:
                if (state.type == agentType.teacher)
                    material.color = Color.yellow;
                else
                    material.color = Color.blue;
                break;
            case simulation.infection:
                LogSystem.Instance().Log(name + " got infected", logType.disease);
                material.color = Color.green;
                break;
            case simulation.fire:
                if(initedDay)
                    LogSystem.Instance().Log(name + " started evacuating", logType.fire);
                material.color = Color.red;
                state.action = agentAction.inactive;
                ResetTarget();
                MoveToRandomExit();
                break;
            case simulation.special:
                break;
            case simulation.zombie:
                if(initedDay)
                    LogSystem.Instance().Log(name + " became a zombie", logType.zombie);
                material.color = Color.black;
                ResetTarget();
                state.action = agentAction.work;
                break;
            default:
                break;
        }
        state.sim = s;
        UpdateSpeed(SimulationManager.Instance().GetAgentSpeed());
    }
    //Main agent loop, depending on its state, gathers information on the environment and processes a response
    private void SimulationUpdate()
    {
        switch (state.sim)
        {
            case simulation.regular:
                ActivityUpdate();
                break;
            case simulation.infection:
                ActivityUpdate();
                if (canInfect && initedDay && timer < Time.time)
                {
                    timer = Time.time + (SimulationManager.Instance().infectionInfo.getInfectRatio() * 60)/ DayTime.Instance().timeSpeed;
                    foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
                    {
                        if (Vector3.Distance(t.position, transform.position) < SimulationManager.Instance().infectionInfo.getInfectRange())
                        {
                            if (Random.Range(0, 1.0f) < SimulationManager.Instance().infectionInfo.getInfectChance())
                            {
                                Agent ag = t.transform.GetComponent<Agent>();
                                if (ag.state.sim != simulation.infection)
                                    ag.ChangeSimulation(simulation.infection);
                            }
                        }
                    }
                }
                break;
            case simulation.fire:
                if (timer < Time.time && initedDay)
                {
                    timer = Time.time + delay;
                   
                    foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
                    {
                        if (Vector3.Distance(t.position, transform.position) > 7)
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
                if (timer < Time.time && initedDay)
                {
                    timer = Time.time + delay;
                    float minDist = 1000;
                    Transform newTarget = null;
                    
                    foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
                    {
                        float dist = Vector3.Distance(t.transform.position, transform.position);
                        if (dist > 6)
                            continue;

                        Agent ag = t.transform.GetComponent<Agent>();

                        if (ag.state.sim == simulation.zombie || !ag.initedDay)
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

    private void OnTriggerStay(Collider other) {
        if(other.GetComponent<Exit>() != null)
        {
            if(state.action == agentAction.inactive)
            {
                EndDay();
            }
        }
    }

    public void UpdateSpeed(float speed)
    {
        if (state.sim == simulation.zombie)
            speed *= 1.2f;
        navAgent.speed = speed;
    }

    //Prepares the agent to enter the scenario
    private void SetUp()
    {
        if (!initedDay)
        {
            LogSystem.Instance().Log(name + " entered the building ", logType.enter);
            gameObject.transform.position = SimulationManager.Instance().GetRandomEntrance();
            navAgent.speed = SimulationManager.Instance().GetAgentSpeed();
            GetComponent<MeshRenderer>().enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
            initedDay = true;
        }
    }

    public void startInfection()
    {
        ChangeSimulation(simulation.infection);
        canInfect = true;
        material.color = Color.magenta;
    }
}
