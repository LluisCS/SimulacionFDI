using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject
{
    public uint startHour, startMinute, endHour, endMinute;
    public SubjectInfo info;
    public Room room;
    public weekDay day;
    public Subject(weekDay d, uint sHour, uint sMinute, uint dHour, uint dMinute, Room r, SubjectInfo inf)
    {
        day = d;
        startHour = sHour;
        startMinute = sMinute;
        endHour = sHour + dHour;
        endMinute = sMinute + dMinute;
        info = inf;
        room = r;
        while (endMinute > 59)
        {
            endMinute -= 60;
            endHour++;
        }
    }

    private void MessageAgents()
    {
        foreach (var student in info.students)
        {
            student.SubjectUpdate(info.name, info.state, room);
        }

        foreach (var teacher in info.teachers)
        {
            teacher.SubjectUpdate(info.name, info.state, room);
        }
    }

    public operation Update(uint h, uint min)
    {
        operation op = operation.nothing;
        switch (info.state)
        {
            case subjectState.start:
                if ((h * 60) + min > (startHour * 60) + startMinute)
                {
                    info.state = subjectState.active;
                    MessageAgents();
                    LogSystem.Instance().Log(info.name + " has just started");
                    
                }
                break;
            case subjectState.active:
                if ((h * 60) + min > (endHour * 60) + endMinute - 5)
                {
                    info.state = subjectState.end;
                    MessageAgents();
                    LogSystem.Instance().Log(info.name + " is ending"); 
                }
                break;
            case subjectState.end:
                if ((h * 60) + min > (endHour * 60) + endMinute + 5)
                {
                    op = operation.remove;
                    info.state = subjectState.inactive;
                    MessageAgents();
                    
                }
                break;
            case subjectState.inactive:
                if ((h * 60) + min > (startHour * 60) + startMinute - 8 && (h * 60) + min < (endHour * 60) + endMinute)
                {
                    op = operation.add;
                    info.state = subjectState.start;
                    MessageAgents();
                    LogSystem.Instance().Log(info.name + " is a about to begin in " + room.name);
                }
                break;
            default:
                break;
        }

        return op;
    }
}