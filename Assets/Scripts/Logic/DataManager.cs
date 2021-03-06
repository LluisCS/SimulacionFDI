﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public GameObject agentPrefab;
    [HideInInspector]
    //public GameObject studentParent, teacherParent;
    public GameObject agentParent;
    public bool logs = false;
    //private SubjectData[] subjectsData;
    //private TeacherData[] teachersData;
    //private StudentData[] studentsData;

    public SubjectSchedule GenerateSchedule(LayoutManager LManager) {
        SubjectSchedule schedule = new SubjectSchedule();
        SubjectData[]  subjectsData = Resources.LoadAll<SubjectData>("Data/Subjects");
        TeacherData[] teachersData = Resources.LoadAll<TeacherData>("Data/Teachers");
        StudentData[] studentsData = Resources.LoadAll<StudentData>("Data/Students");

        if (logs) Debug.Log(subjectsData.Length);
        if (logs) Debug.Log(studentsData.Length);
        if (logs) Debug.Log(teachersData.Length);

        foreach (var sub in subjectsData)
        {
            SubjectInfo info = new SubjectInfo(sub.name);
            foreach (var hour in sub.classes)
            {
                Subject s = new Subject(hour.day, hour.startHour, hour.startMinute, hour.durationHours, hour.durationMinutes, LManager.getRoom(hour.roomName),info);
                schedule.days[(int)hour.day].Add(s);
                info.hours.Add(s);
            }
            schedule.subjectInfos.Add(sub.name, info);
            if (logs) Debug.Log(sub.name + " loaded.");

        }

        agentParent = new GameObject("agentParent");
        foreach (var teacher in teachersData)
        {
            GameObject agentObj = Instantiate(agentPrefab);
            int modelNum = Random.Range(0, 2);
            agentObj.transform.GetChild(modelNum).gameObject.SetActive(false);
            Destroy(agentObj.transform.GetChild((modelNum + 1) % 2).gameObject);
            agentObj.transform.SetParent(agentParent.transform);
            Agent agentComp = agentObj.GetComponent<Agent>();
            agentComp.state.type = agentType.teacher;
            agentComp.name = teacher.name;
            agentComp.state.per = teacher.per;
            if (agentComp == null)
                Debug.LogError("Agent prefab missing agent component.");
            else
            {
                foreach (var s in teacher.classNames)
                {
                    SubjectInfo tmp;
                    if (schedule.subjectInfos.TryGetValue(s, out tmp))
                    {
                        tmp.teachers.Add(agentComp);
                        agentComp.subjects.Add(s, tmp);
                    }
                    else
                        Debug.LogWarning("Subject with name " + s + " not found.");
            }
                foreach (var a in teacher.facultyActivities)
                {
                    agentComp.activities.Add(a);
                }
                if (teacher.autoLunchActivity)
                    GenerateLunchActivities(agentComp);
            }
            if (logs) Debug.Log(teacher.name + " loaded.");
        }
        //studentParent = new GameObject("studentParent");
        foreach (var student in studentsData)
        {
            GameObject agentObj = Instantiate(agentPrefab);
            int modelNum = Random.Range(0, 2);
            agentObj.transform.GetChild(modelNum).gameObject.SetActive(false);
            Destroy(agentObj.transform.GetChild((modelNum + 1) % 2).gameObject);
            agentObj.transform.SetParent(agentParent.transform);
            Agent agentComp = agentObj.GetComponent<Agent>();
            agentComp.state.per = student.per;
            agentComp.name = student.name;
            if (agentComp == null)
                Debug.LogError("Agent prefab missing agent component.");
            else
            {
                foreach (var s in student.classNames)
                {
                    SubjectInfo tmp;
                    if (schedule.subjectInfos.TryGetValue(s, out tmp))
                    {
                        tmp.students.Add(agentComp);
                        agentComp.subjects.Add(s, tmp);
                    }
                    else
                        Debug.LogWarning("Subject with name " + s + " not found.");
                }
                foreach (var a in student.facultyActivities)
                {
                    agentComp.activities.Add(a);
                }
                if (student.autoLunchActivity)
                    GenerateLunchActivities(agentComp);
            }
            if (logs) Debug.Log(student.name + " loaded.");
        }
        return schedule;
    }
    
    private void GenerateLunchActivities(Agent ag)
    {
        for (int i = 0; i < 5; i++)
        {
            uint morningEnd = 0;
            uint afternoonStart = 99999999;
            foreach (var act in ag.activities)
            {
                if (i == (int)act.day)
                {
                    uint tmp = (act.startHour * 60) + act.startMinute;
                    if (tmp > 14 * 60 && tmp < afternoonStart)
                        afternoonStart = tmp;
                    else
                    {
                        tmp += (act.durationHours * 60) + act.durationMinutes;
                        if (tmp < 13 * 60 && tmp > morningEnd)
                            morningEnd = tmp;
                    }
                }
            }
            foreach (var sInfo in ag.subjects)
            {
                foreach(Subject s in sInfo.Value.hours)
                {
                    if (i == (int)s.day)
                    {
                        uint tmp = (s.startHour * 60) + s.startMinute;
                        if (tmp > 14 * 60 && tmp < afternoonStart)
                            afternoonStart = tmp;
                        else
                        {
                            tmp = (s.endHour * 60) + s.endMinute;
                            if (tmp < 13 * 60 && tmp > morningEnd)
                                morningEnd = tmp;
                        }
                    }
                }
            }
            if (morningEnd > 0 && afternoonStart < 9999999)
            {
                activity a;
                a.day = (weekDay)i;
                a.name = "Lunch";
                a.roomName = "Cafeteria";
                a.startHour = 13;
                a.durationHours = 0;
                a.durationMinutes = (uint)Random.Range(20, 35);
                if (afternoonStart > 30 + (14 * 60))
                    a.startMinute = (uint)Random.Range(29, 59);
                else
                    a.startMinute = (uint)Random.Range(5, 35);
                ag.activities.Add(a);
            }
        }
    }
    void generateRandomStudents() { }
    void generateRandomTeachers() { }
    //void generateRandomSubjects() { }
}
