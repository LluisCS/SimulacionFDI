using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public GameObject layoutObject;
    public GameObject entranceObject;
    public List<Floor> FDI;
    public bool logs = false;
    
    void Awake()
    {
        if (logs) Debug.Log("Start logic layout");
        FDI = new List<Floor>();
        foreach (Transform floor in layoutObject.transform)
        {
            Floor tmpFloor = new Floor(floor.name);
            if (logs) Debug.Log("Adding floor");
            foreach (Transform room in floor.transform)
            {
                if(room.name == "Hall")
                {
                    foreach (Transform seat in room.transform)
                    {
                        Seat tmpSeat = new Seat(seat.transform.position);
                        tmpFloor.hall.Add(tmpSeat);
                    }
                }
                else { 
                    Room tmpRoom = new Room(room.name, room.transform.position, tmpFloor);
                    if (logs) Debug.Log("Adding room");
                    foreach (Transform seat in room.transform)
                    {
                        Seat tmpSeat = new Seat(seat.transform.position);
                        tmpRoom.seats.Add(tmpSeat);
                    }
                    tmpFloor.rooms.Add(tmpRoom);
                }
            }
            FDI.Add(tmpFloor);
        }
        if (logs) Debug.Log("End logic layout");
    }

    public Room getRoom(string name)
    {
        foreach (Floor floor in FDI)
            foreach (Room room in floor.rooms)
                if(room.name == name)
                    return room;

        Debug.LogWarning("Room with name " + name + " not found.");
        return null;
    }

    //public Floor getRoomFloor(string name)
    //{
    //    foreach (Floor floor in FDI)
    //        foreach (Room room in floor.rooms)
    //            if (room.name == name)
    //                return floor;

    //    Debug.LogWarning("Floor of room with name " + name + " not found.");
    //    return null;
    //}

    public Vector3 getRandomEntrance()
    {
        int rnd = Random.Range(0, entranceObject.transform.childCount-1);
        return entranceObject.transform.GetChild(rnd).position;
    }
}
