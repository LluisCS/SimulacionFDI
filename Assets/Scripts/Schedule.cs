using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum weekDay { Monday, Tuesday, Wednesday, Thursday, Friday }
public enum subjectState { start, active, end, inactive }
public enum operation { add, remove, nothing }

public class SubjectInfo
{
    public subjectState state;
    public List<Agent> teachers;
    public List<Agent> students;
    public List<Subject> hours;
    public string name;

    public SubjectInfo(string n)
    {
        teachers = new List<Agent>(0);
        students = new List<Agent>(0);
        hours = new List<Subject>(0);
        state = subjectState.inactive;
        name = n;
    }
}

[System.Serializable]
public struct activity
{
    public string roomName, name;
    public weekDay day;

    [Range(7, 20)]
    public uint startHour;
    [Range(0, 59)]
    public uint startMinute;
    [Range(0, 6)]
    public uint durationHours;
    [Range(0, 59)]
    public uint durationMinutes;
}

public class SubjectSchedule
{
    public List<Subject>[] days;
    public List<Subject> activeSubjects;
    public Dictionary<string, SubjectInfo> subjectInfos;
    public SubjectSchedule()
    {
        days = new List<Subject>[5];
        for (int i = 0; i < days.Length; i++)
        {
            days[i] = new List<Subject>(0);
        }
        activeSubjects = new List<Subject>(0);
        subjectInfos = new Dictionary<string, SubjectInfo>(0);
    }
}