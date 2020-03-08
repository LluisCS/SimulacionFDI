using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public bool logs = false;
    private weekDay day = weekDay.Monday;
    public SubjectUI subjectUI;
    public Text speedText;
    public float initialAgentSpeed = 2.0f, initialTimeSpeed = 60.0f;
    public float speedInrement = 0.5f, maxSpeed = 10.0f, minSpeed = 0.5f;
    private float speedMultiplier = 1.0f;
    private bool pause = false;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        dataManager = GetComponent<DataManager>();
        layoutManager = GetComponent<LayoutManager>();
        schedule = dataManager.generateSchedule(layoutManager);

        startDay(DayTime.Instance().WeekDay());
        updateSpeed();
    }

    
    void Update()
    {
        if (day != DayTime.Instance().WeekDay())
            startDay(DayTime.Instance().WeekDay());


        day = DayTime.Instance().WeekDay();
        uint hour = (uint)DayTime.Instance().Hour();
        uint minute = (uint)DayTime.Instance().Minute();
        //Debug.Log((int)day);
        foreach (Subject s in schedule.days[(int)day])
        {
            //if (logs) Debug.Log(s.name + " exists");

            operation op = s.update(hour, minute);
            if (op == operation.add)
            {
                if (logs) Debug.Log(s.info.name + " has started.");
                schedule.activeSubjects.Add(s);
                s.room.occupied = true;
                subjectUI.updateUI(schedule.activeSubjects);
            }
            else if(op== operation.remove)
            {
                if (logs) Debug.Log(s.info.name + " has ended.");
                schedule.activeSubjects.Remove(s);
                s.room.occupied = false;
                subjectUI.updateUI(schedule.activeSubjects);
            }
        }
    }

    private void startDay(weekDay day)
    {
        foreach (Transform student in dataManager.studentParent.transform)
        {
            Agent ag = student.GetComponent<Agent>();
            ag.startDay();
        }
        foreach (Transform teacher in dataManager.teacherParent.transform)
        {
            Agent ag = teacher.GetComponent<Agent>();
            ag.startDay();
        }
        schedule.activeSubjects.Clear();
        foreach (Subject s in schedule.days[(int)day])
        {
            foreach (var student in s.info.students)
            {
                student.addSubjectCount();
            }
            foreach (var teacher in s.info.teachers)
            {
                teacher.addSubjectCount();
            }
        }
    }

    public Vector3 getRandomEntrance()
    {
        return layoutManager.getRandomEntrance();
    }

    public Room GetRoom(string name)
    {
        return layoutManager.getRoom(name);
    }

    public void changeSpeed(bool decrease)
    {
        if (decrease)
        {
            if (speedMultiplier - speedInrement < minSpeed)
                speedMultiplier = minSpeed;
            else
                speedMultiplier -= speedInrement;
        }
        else
        {
            if (speedMultiplier + speedInrement > maxSpeed)
                speedMultiplier = maxSpeed;
            else
                speedMultiplier += speedInrement;
        }
        updateSpeed();
    }

    private void updateSpeed()
    {
        DayTime.Instance().setTimeSpeed(initialTimeSpeed * speedMultiplier);

        foreach (NavMeshAgent ag in dataManager.studentParent.GetComponentsInChildren<NavMeshAgent>())
            ag.speed = speedMultiplier * initialAgentSpeed;
        foreach (NavMeshAgent ag in dataManager.teacherParent.GetComponentsInChildren<NavMeshAgent>())
            ag.speed = speedMultiplier * initialAgentSpeed;
        speedText.text = "x " + speedMultiplier.ToString("F1");
    }

    public void togglePause()
    {
        pause = !pause;
        foreach (Agent ag in dataManager.studentParent.GetComponentsInChildren<Agent>())
            ag.togglePause();
        foreach (Agent ag in dataManager.teacherParent.GetComponentsInChildren<Agent>())
            ag.togglePause();

    }
}
