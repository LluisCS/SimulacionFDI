using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    public Camera camera;
    public NavMeshAgent agent;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
                agent.isStopped = false;
            }
        }

        if (agent.remainingDistance < 0.1)
            agent.isStopped = true;
    }
}
