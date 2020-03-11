using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    private Transform target = null;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private bool active = false;

    void FixedUpdate()
    {
        if (!active || target == null)
            return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

    public bool select()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            Agent ag = objectHit.GetComponent<Agent>();
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
}