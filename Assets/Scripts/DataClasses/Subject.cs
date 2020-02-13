﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                if (h * 60 + min > endHour * 60 + endMinute + 5)
                {
                    op = operation.remove;
                    info.state = subjectState.inactive;
                    messageAgents();
                }
                break;
            case subjectState.inactive:
                if (h * 60 + min > startHour * 60 + startMinute - 5 && h * 60 + min < endHour * 60 + endMinute)
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