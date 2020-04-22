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
        if (active && timer < Time.deltaTime)
        {
            timer = Time.deltaTime + delay;
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 12, transform.forward);
            foreach (var hit in hits)
            {
                Agent ag = hit.transform.GetComponent<Agent>();
                if (ag != null && ag.state.sim != simulation.fire)
                    ag.changeSimulation(simulation.fire);
            }
        }
    }

    public void toggleAllActive() {
        foreach (Transform child in transform.parent.transform)
        {
            FireAlarm fa = child.GetComponent<FireAlarm>();
            if (fa != null)
                fa.active = !fa.active;
        }
    }
}
