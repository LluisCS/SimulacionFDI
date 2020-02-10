﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public SubjectData[] subjectsData;
    public TeacherData[] teachersData;
    public StudentData[] studentsData;
    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    public SubjectSchedule generateSchedule(LayoutManager LManager) {
        SubjectSchedule subSchedule = new SubjectSchedule();

        foreach (var sub in subjectsData)
        {
            SubjectInfo info = new SubjectInfo();
            foreach (var hour in sub.classes)
            {
                Subject s = new Subject( hour.startHour, hour.startMinute, hour.durationHours, hour.durationMinutes,sub.name, LManager.getRoom(hour.roomName),info);
                subSchedule.days[(int)hour.day].Add(s);
            }
            subSchedule.subjectInfos.Add(sub.name, info);
        }

        foreach (var teacher in teachersData)
        {

        }
        return subSchedule;
    }

    void generateRandomStudents() { }
    void generateRandomTeachers() { }
    //void generateRandomSubjects() { }
}