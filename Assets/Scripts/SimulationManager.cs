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
    public bool logs = false;
    private weekDay day = weekDay.Monday;

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
                schedule.activeSubjects.Add(s.info.name);
                s.room.occupied = true;
                if (logs) Debug.Log(s.info.name + " has started.");
            }
            else if(op== operation.remove)
            {
                if (logs) Debug.Log(s.info.name + " has ended.");
                schedule.activeSubjects.Remove(s.info.name);
                s.room.occupied = false;
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
}
