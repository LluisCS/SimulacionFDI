using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    private Transform target = null;

    public float smoothSpeed = 1.25f;
    public Vector3 offset;
    private bool active = false;

    void LateUpdate()
    {
        if (!active || target == null)
            return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

    public bool select(out Agent ag)
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

    public void deselect()
    {
        target = null;
        active = false;
    }

    public void changeTargetSimulation(simulation sim)
    {
        if(target != null)
        {
            target.GetComponent<Agent>().changeSimulation(sim);
        }
    }
}