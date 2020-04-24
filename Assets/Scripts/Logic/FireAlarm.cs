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
        return;
        if (active && timer < Time.time)
        {
            timer = Time.time + delay;
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 12, transform.forward);
            foreach (var hit in hits)
            {
                Agent ag = hit.transform.GetComponent<Agent>();
                if (ag != null && ag.state.sim != simulation.fire)
                    ag.ChangeSimulation(simulation.fire);
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
