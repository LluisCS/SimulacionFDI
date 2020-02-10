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

public class Subject
{
    public uint startHour, startMinute, endHour, endMinute;
    public string name;
    public SubjectInfo info;
    public Room room;
    public Subject(uint sHour, uint sMinute, uint dHour, uint dMinute, string n, Room r, SubjectInfo inf)
    {
        startHour = sHour;
        startMinute = sMinute;
        endHour = sHour + dHour;
        endMinute = sMinute + dMinute;
        name = n;
        info = inf;
        room = r;
        while (endMinute > 59)
        {
            endMinute -= 60;
            endHour++;
        }
    }

    private void messageAgents()
    {
        foreach (var student in info.students)
        {
            student.subjectUpdate(name, info.state, room);
        }

        foreach (var teacher in info.teachers)
        {
            teacher.subjectUpdate(name, info.state, room);
        }
    }

    public operation update(uint h, uint min)
    {
        operation op = operation.nothing;
        switch (info.state)
        {
            case subjectState.start:
                if (h * 60 + min > startHour * 60 + startMinute)
                {
                    info.state = subjectState.active;
                    messageAgents();
                }
                break;
            case subjectState.active:
                if (h * 60 + min > endHour * 60 + endMinute)
                {
                    info.state = subjectState.end;
                    messageAgents();
                }
                break;
            case subjectState.end:
                if (h * 60 + min > endHour * 60 + endMinute+5)
                {
                    op = operation.remove;
                    info.state = subjectState.inactive;
                    messageAgents();
                }
                break;
            case subjectState.inactive:
                if(h*60 + min > startHour*60 +startMinute-5 && h*60 + min < endHour*60 + endMinute )
                {
                    op = operation.add;
                    info.state = subjectState.start;
                    messageAgents();
                }
                break;
            default:
                break;
        }

        return op;
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