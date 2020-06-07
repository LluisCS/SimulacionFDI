using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTime : MonoBehaviour
{
    static private DayTime instance;
    static public DayTime Instance() {
        if (instance == null)
            Debug.LogError("Day Time not initiated");
        return instance;
    }
    private int hours = 0, minutes = 0;
    private double seconds = 0;
    private weekDay day = weekDay.Monday;
    public double timeSpeed = 1.0f;
    public int initialHour = 8, initialMinute = 45, lastDayHour = 22;
    public bool logs = false;
    private bool pause = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        hours = initialHour;
        minutes = initialMinute;
    }

    void Update()
    {
        if (pause) return;
        seconds += Time.deltaTime * timeSpeed;
        while(seconds >= 60){
            minutes++;
            seconds -= 60;
        }
        while(minutes >= 60)
        {
            minutes -= 60;
            hours++;
        }
        if (hours >= lastDayHour)
        {
            hours = initialHour;
            minutes = initialMinute;
            day++;
            if ((int)day > 4)
                day = 0;
        }
        if (logs) Debug.Log("Time is: "+ day.ToString()+ " " + hours + ":" + minutes);
    }

    public int Hour()
    {
        return hours;
    }

    public int Minute()
    {
        return minutes;
    }

    public double Second()
    {
        return seconds;
    }

    public weekDay WeekDay()
    {
        return day;
    }

    public void setTimeSpeed(double tSpeed)
    {
        timeSpeed = tSpeed;
    }

    public bool activityEnded( activity a)
    {
        return hours * 60 + minutes > (a.startHour + a.durationHours) * 60 + a.startMinute + a.durationMinutes;
    }

    public bool activityStarted( activity a)
    {
        return hours * 60 + minutes > a.startHour * 60 + a.startMinute;
    }

    public double getTimeSpeed()
    {
        return timeSpeed;
    }

    public void togglePause()
    {
        pause = !pause;
    }

    public void skipDay()
    {
        day++;
        if ((int)day > 4)
            day = 0;
        hours = initialHour;
        minutes = initialMinute;
        SimulationManager.Instance().EndDay();
    }
}
