﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum logType { disease, zombie, fire, enter, exit, none};

public class LogSystem : MonoBehaviour
{
    //public bool active = true;
    private static int logNumber = 0;
    static private LogSystem instance;
    private List<string> logs;
    private int nDisease, nZombie, nFire, nCharacters;
    private StreamWriter logFile;

    static public LogSystem Instance()
    {
        if (instance == null)
            Debug.LogError("LogSystem not initiated");
        return instance;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        InvokeRepeating("WriteLogs", 1.0f, 1.5f);
        logs = new List<string>(0);
    }

    public void StartDay()
    {
        if (logFile != null)
        {
            WriteLogs();
            logFile.Close();
        }
        logNumber++;
        nDisease = nZombie = nFire = nCharacters = 0;
        logFile = new StreamWriter(Application.dataPath + "/../logFile" + logNumber + ".txt");
        logFile.WriteLine("This is a log file generated by the FDI simulation app which recaps relevant information from events happening in the building. Current time is " + DayTime.Instance().Hour().ToString("D2") + ":" + DayTime.Instance().Minute().ToString("D2") + " " + DayTime.Instance().WeekDay().ToString().ToUpper());
    }

    public void Log(string text, logType type = logType.none)
    {
        string s = (text + " at " + DayTime.Instance().Hour().ToString("D2") + ":" + DayTime.Instance().Minute().ToString("D2") );
        switch (type)
        {
            case logType.disease:
                nDisease++;
                s = s + "   " + nDisease + " character/s infected by the disease.";
                break;
            case logType.zombie:
                nZombie++;
                s = s + "   " + nZombie + " character/s transormed into a zombie.";
                break;
            case logType.fire:
                nFire++;
                s = s + "   " + nFire + " character/s started evacuating.";
                break;
            case logType.enter:
                nCharacters++;
                s = s + "   " + nCharacters + " character/s in the building.";
                break;
            case logType.exit:
                nCharacters--;
                s = s + "   " + nCharacters + " character/s in the building.";
                break;
            case logType.none:
                break;
            default:
                break;
        }
        logs.Add(s);
    }

    public void WriteLogs()
    {
        foreach (string log in logs)
            logFile.WriteLine(log);

        logFile.Flush();
        logs.Clear();
    }

    void OnDestroy()
    {
        WriteLogs();
        logFile.Close();
    }
}
