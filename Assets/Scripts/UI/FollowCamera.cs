using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    private Transform target = null;

    public float smoothSpeed = 1.25f;
    public Vector3 offset, positionOffset;
    private bool active = false;

    void LateUpdate()
    {
        if (!active || target == null)
            return;
       
        Vector3 desiredPosition = target.position + (target.forward * offset.x) + (target.up * offset.y) + (target.right * offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target.position + positionOffset);
    }

    public bool Select(out Agent ag)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ag = null;
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            ag = objectHit.GetComponent<Agent>();
            if (ag != null)
            {
                target = objectHit;
                active = true;
                Debug.Log(ag.name);
            }
        }
        return active;
    }

    public void Deselect()
    {
        target = null;
        active = false;
    }

    public bool ChangeTargetSimulation(simulation sim)
    {
        if(target != null)
        {
            if (sim == simulation.infection)
                target.GetComponent<Agent>().startInfection();
            else
                target.GetComponent<Agent>().ChangeSimulation(sim);
            
            return true;
        }
        return false;
    }
}