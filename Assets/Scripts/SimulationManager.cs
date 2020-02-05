using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(DataManager))]
public class SimulationManager : MonoBehaviour
{
    private static SimulationManager instance;
    public static SimulationManager Instance() {
        if (instance == null)
            Debug.LogError("Simulation Manager not initiated");
        return instance;
    }
    private DataManager dataManager;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        dataManager = GetComponent<DataManager>();
    }

    
    void Update()
    {
        
    }
}
