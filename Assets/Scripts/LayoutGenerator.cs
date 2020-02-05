using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public struct Seat
{
    public bool occupied;
    public Vector3 position;
}
public struct Room
{
    public Vector3 entrance;
    public List<Seat> seats;
    public string name;
}
public struct Floor
{
    public List<Room> rooms;
    public string name;
}

public class LayoutGenerator : MonoBehaviour
{
    public GameObject layoutObject;
    public List<Floor> FDI;
    private float timer = 0;
    public float cooldown = 5;
    public bool waitForDestination = true;

    public GameObject agent; //tmp
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start logic layout");
        FDI = new List<Floor>();
        foreach (Transform floor in layoutObject.transform)
        {
            Floor tmpFloor;
            tmpFloor.rooms = new List<Room>();
            tmpFloor.name = floor.name;
            Debug.Log("Adding floor");
            foreach (Transform room in floor.transform)
            {
                Room tmpRoom;
                tmpRoom.seats = new List<Seat>();
                tmpRoom.entrance = room.transform.position;
                tmpRoom.name = room.name;
                foreach (Transform seat in room.transform)
                {
                    Seat tmpSeat;
                    tmpSeat.position = seat.transform.position;
                    tmpSeat.occupied = false;
                    tmpRoom.seats.Add(tmpSeat);
                }
                tmpFloor.rooms.Add(tmpRoom);
            }
            FDI.Add(tmpFloor);
        }
        Debug.Log("End logic layout");
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0) {
            timer = cooldown;
            foreach (Transform item in agent.transform) {
                NavMeshAgent agentComp = (NavMeshAgent)item.GetComponent("NavMeshAgent");
                dGoToRandom(agentComp);
            }
           
        }
    }

    void dGoToRandom(NavMeshAgent agentComp)
    {
        int rnd = Random.Range(0, FDI.Count);
        //rnd = 0;
        int rnd2 = Random.Range(0, FDI[rnd].rooms.Count);
        //rnd2 = 0;
        int rnd3 = Random.Range(0, FDI[rnd].rooms[rnd2].seats.Count);
        
        if (waitForDestination && agentComp.remainingDistance > 0.5)
            return;
        agentComp.SetDestination(FDI[rnd].rooms[rnd2].seats[rnd3].position);
        //Debug.Log(FDI[rnd].rooms[rnd2].seats[rnd3].position.ToString());
    }
}
