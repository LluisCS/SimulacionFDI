using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DataManager))] [RequireComponent(typeof(LayoutManager))]
public class SimulationManager : MonoBehaviour
{
    private static SimulationManager instance;
    public static SimulationManager Instance() {
        if (instance == null)
            Debug.LogError("Simulation Manager not initiated");
        return instance;
    }
    private DataManager dataManager;
    private LayoutManager layoutManager;
    public SubjectSchedule schedule;
    private float timer = 0;
    public float cooldown = 5;
    public bool waitForDestination = true;

    public SubjectSchedule subjectSchedule;
    public GameObject agent; 
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        dataManager = GetComponent<DataManager>();
        layoutManager = GetComponent<LayoutManager>();
        subjectSchedule = dataManager.generateSchedule(layoutManager);
    }

    
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = cooldown;
            foreach (Transform item in agent.transform)
            {
                NavMeshAgent agentComp = (NavMeshAgent)item.GetComponent("NavMeshAgent");
                dGoToRandom(agentComp);
            }

        }

        weekDay day = DayTime.Instance().WeekDay();
        int hour = DayTime.Instance().Hour();
        int minute = DayTime.Instance().Minute();

        foreach (Subject item in schedule.days[(int)day])
        {
            //incomplete
        }
    }

    void dGoToRandom(NavMeshAgent agentComp)
    {
        int rnd = Random.Range(0, layoutManager.FDI.Count);
        //rnd = 0;
        int rnd2 = Random.Range(0, layoutManager.FDI[rnd].rooms.Count);
        //rnd2 = 0;
        int rnd3 = Random.Range(0, layoutManager.FDI[rnd].rooms[rnd2].seats.Count);

        if (waitForDestination && agentComp.remainingDistance > 0.5)
            return;
        agentComp.SetDestination(layoutManager.FDI[rnd].rooms[rnd2].seats[rnd3].position);
        //Debug.Log(FDI[rnd].rooms[rnd2].seats[rnd3].position.ToString());
    }
}
