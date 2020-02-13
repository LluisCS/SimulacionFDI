using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum weekDay { Monday, Tuesday, Wednesday, Thursday, Friday }
[System.Serializable]
public enum personality { }
public enum subjectState { start, active, end, inactive }
public enum operation { add, remove, nothing }

public class SubjectInfo
{
    public subjectState state;
    public Agent[] teachers;
    public Agent[] students;
    public SubjectInfo()
    {
        teachers = new Agent[0];
        students = new Agent[0];
        state = subjectState.inactive;
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
    public List<string> activeSubjects;
    public Dictionary<string, SubjectInfo> subjectInfos;
    public SubjectSchedule()
    {
        days = new List<Subject>[5];
        activeSubjects = new List<string>(0);
        subjectInfos = new Dictionary<string, SubjectInfo>(0);
    }
}