using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Seat
{
    public bool occupied;
    public Vector3 position;
}
public struct Room
{
    public Vector3 entrance;
    public List<Seat> seats;
    public List<Seat> teacherSeats;
    public string name;
}
public struct Floor
{
    public List<Room> rooms;
    public string name;
}

public class LayoutManager : MonoBehaviour
{
    public GameObject layoutObject;
    public List<Floor> FDI;
    public bool logs = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (logs) Debug.Log("Start logic layout");
        FDI = new List<Floor>();
        foreach (Transform floor in layoutObject.transform)
        {
            Floor tmpFloor;
            tmpFloor.rooms = new List<Room>();
            tmpFloor.name = floor.name;
            if (logs) Debug.Log("Adding floor");
            foreach (Transform room in floor.transform)
            {
                Room tmpRoom;
                tmpRoom.seats = new List<Seat>();
                tmpRoom.teacherSeats = new List<Seat>();
                tmpRoom.entrance = room.transform.position;
                tmpRoom.name = room.name;
                if (logs) Debug.Log("Adding room");
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
        if (logs) Debug.Log("End logic layout");
    }

    public Room getRoom(string name)
    {
        foreach (Floor floor in FDI)
            foreach (Room room in floor.rooms)
                if(room.name == name)
                    return room;

        Debug.LogWarning("Room with name " + name + " not found.");
        return emptyRoom();
    }

    public Floor getRoomFloor(string name)
    {
        foreach (Floor floor in FDI)
            foreach (Room room in floor.rooms)
                if (room.name == name)
                    return floor;

        Debug.LogWarning("Floor of room with name " + name + " not found.");
        return emptyFloor();
    }

    private Room emptyRoom()
    {
        Room r;
        r.name = "NoRoom";
        r.seats = new List<Seat>(0);
        r.teacherSeats = new List<Seat>(0);
        r.entrance = Vector3.zero;
        return r;
    }

    private Floor emptyFloor()
    {
        Floor f;
        f.name = "NoFloor";
        f.rooms = new List<Room>(0);
        return f;
    }
}
