﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector3 entrance;
    public List<Seat> seats;
    public List<Seat> teacherSeats;
    public bool occupied;
    public string name;
    public Room(string n, Vector3 en)
    {
        seats = new List<Seat>();
        teacherSeats = new List<Seat>();
        entrance = en;
        name = n;
        occupied = false;
    }
    public Seat getFirstFreeSeat()
    {
        foreach (var seat in seats)
        {
            if (!seat.occupied)
                return seat;
        }
        Debug.LogWarning("No empty seat found in room " + name);
        return null;
    }
    //public Seat getRandomFreeSeat() { }
}