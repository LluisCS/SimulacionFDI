using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAlarm : MonoBehaviour
{
    public bool active = false;
    private double timer = 0;
    public double delay = 1f;

    void Update()
    {
        if (active && timer < Time.time)
        {
            timer = Time.time + delay;
            foreach (Transform t in SimulationManager.Instance().dataManager.agentParent.transform)
            {
                if (Vector3.Distance(t.position, transform.position) < 5)
                {
                    Agent ag = t.transform.GetComponent<Agent>();
                    if (ag.state.sim != simulation.fire && ag.initedDay)
                        ag.ChangeSimulation(simulation.fire);
                }
            }
        }
    }

    public void ToggleAllActive() {
        foreach (Transform child in transform.parent.transform)
        {
            FireAlarm fa = child.GetComponent<FireAlarm>();
            if (fa != null)
                fa.active = !fa.active;
        }
    }
}
