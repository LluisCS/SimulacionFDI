using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectiousRemains : MonoBehaviour
{
    public static double deathTimer = 80, infectTimer = 25, infectChance = 0.1, infectRange = 1;
    private double currentDeathTimer = 0;
    private double timer = 0;
    
    void Start()
    {
        currentDeathTimer = deathTimer;
    }

    void Update()
    {
        if (timer < Time.time)
        {
            timer = Time.time + (infectTimer * 60)/ DayTime.Instance().timeSpeed;
            foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
            {
                if (Vector3.Distance(t.position, transform.position) < infectRange)
                {
                    if (Random.Range(0, 1.0f) < infectChance)
                    {
                        Agent ag = t.transform.GetComponent<Agent>();
                        if (ag.state.sim != simulation.infection && ag.initedDay)
                            ag.ChangeSimulation(simulation.infection);
                    }
                }
            }
            currentDeathTimer -= infectTimer;
            if (currentDeathTimer < 0)
                Destroy(gameObject);
        }
    }
}
