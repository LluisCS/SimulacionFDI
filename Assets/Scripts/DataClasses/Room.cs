using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Floor floor;
    public Vector3 entrance;
    public List<Seat> seats;
    public List<Seat> teacherSeats;
    public bool occupied;
    public string name;
    public Room(string n, Vector3 en, Floor f)
    {
        seats = new List<Seat>();
        teacherSeats = new List<Seat>();
        floor = f;
        entrance = en;
        name = n;
        occupied = false;
    }
    public Seat getFirstFreeSeat()
    {
        if (SimulationManager.Instance().infectionInfo.safe)
        {
            for (int i = 0; i < seats.Count; i++)
            {
                if (seats[i].occupied)
                    i++;
                else
                    return seats[i];
            }   
        }
        else
        {
            foreach (var seat in seats)
            {
                if (!seat.occupied)
                    return seat;
            }
        }
        Debug.LogWarning("No empty seat found in room " + name);
        return null;
    }
    public Seat getTeacherSeat()
    {
        if (name == "Cafeteria")
            return getFirstFreeSeat();

        foreach (var seat in teacherSeats)
        {
            if (!seat.occupied)
                return seat;
        }
        Debug.LogWarning("No empty seat found in room " + name);
        return null;
    }
    //public Seat getRandomFreeSeat() { }
}