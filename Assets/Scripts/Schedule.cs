using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum weekDay { Monday, Tuesday, Wednesday, Thursday, Friday }
[System.Serializable]
public enum personality { }
public enum subjectState { start, active, end, inactive, cancel }

public class SubjectInfo
{
    public subjectState state;
    public List<Room> rooms;
    public GameObject[] teachers;
    public GameObject[] students;
    public SubjectInfo()
    {
        teachers = new GameObject[0];
        students = new GameObject[0];
        rooms = new List<Room>(0);
        state = subjectState.inactive;
    }
}

public class Subject
{
    public uint startHour, startMinute, endHour, endMinute;
    public string name;
    public SubjectInfo info;
    public Subject(uint sHour, uint sMinute, uint dHour, uint dMinute, string n, SubjectInfo inf)
    {
        startHour = sHour;
        startMinute = sMinute;
        endHour = sHour + dHour;
        endMinute = sMinute + dMinute;
        name = n;
        info = inf;
        while (endMinute > 59)
        {
            endMinute -= 60;
            endHour++;
        }
    }
}

[System.Serializable]
public struct classHour
{
    public string roomName;
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
    public List<SubjectInfo> activeSubjects;
    public Dictionary<string, SubjectInfo> subjectInfos;
    public SubjectSchedule()
    {
        days = new List<Subject>[5];
        activeSubjects = new List<SubjectInfo>(0);
        subjectInfos = new Dictionary<string, SubjectInfo>(0);
    }
}