using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
    public List<Room> rooms;
    public List<Seat> hall;
    public string name;
    public Floor(string n)
    {
        rooms = new List<Room>(0);
        hall = new List<Seat>(0);
        name = n;
    }

    public Seat getClosestSeat(Vector3 pos)
    {
        Seat bestSeat = hall[0];
        float bestDist = 9999;
        foreach (Seat seat in hall)
        {
            if (!seat.occupied)
            {
                Vector2 v = new Vector2(pos.x - seat.position.x, pos.z - seat.position.z);
                if(v.sqrMagnitude < bestDist)
                {
                    bestDist = v.sqrMagnitude;
                    bestSeat = seat;
                }
            }
        }
        if (bestSeat.occupied)
            return null;
        else
            return bestSeat;
    }
}