using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionSimulationInfo: MonoBehaviour
{
    
    public double infectRatio = 5, infectChance = 0.2, infectRange = 5;
    public double safeInfRatio = 10, safeInfChance = 0.05, safeInfRange = 3;
    public GameObject prefab;
    [HideInInspector]
    public GameObject parentObject;
    public bool safe = false;

    void Start()
    {
        parentObject = new GameObject("InfectionEntities");
    }
    
    public double getInfectRatio()
    {
        if (safe)
            return safeInfRatio;
        else
            return infectRatio;
    }

    public double getInfectChance()
    {
        if (safe)
            return safeInfChance;
        else
            return infectChance;
    }

    public double getInfectRange()
    {
        if (safe)
            return safeInfRange;
        else
            return infectRange;
    }

    public void toggleProtocol()
    {
        safe = !safe;
        if(safe)
            LogSystem.Instance().Log("Safe infection protocol ACTIVE");
        else
            LogSystem.Instance().Log("Safe infection protocol INACTIVE");
    }
}
