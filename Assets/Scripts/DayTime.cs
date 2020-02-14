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
    public double simulationSpeed = 1.0f;
    public int initialHour = 7;
    public bool logs = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        hours = initialHour;
    }

    void Update()
    {
        seconds += Time.deltaTime * timeSpeed;
        while(seconds >= 60){
            minutes++;
            seconds -= 60;
        }
        while(minutes >= 60)
        {
            minutes -= 60;
            hours++;
            if (!logs) Debug.Log(hours);
        }
        if (hours > 23)
        {
            hours -= 24;
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
}
