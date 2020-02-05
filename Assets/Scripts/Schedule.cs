using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum weekDay { Monday, Tuesday, Wednesday, Thursday, Friday }

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

public class Schedule
{
    public List<subject>[] days = new List<subject>[5];
    public string presentSubject;
       
}