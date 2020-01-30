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
    public double timeSpeed = 1.0f;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Update()
    {
        seconds += Time.deltaTime * timeSpeed;
        if(seconds >= 60){
            minutes++;
            seconds -= 60;
        }
        if(minutes >= 60)
        {
            minutes -= 60;
            hours++;
        }
        if (hours > 23)
            hours = 0;
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
}
