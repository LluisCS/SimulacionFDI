using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat
{
    public bool occupied;
    public Vector3 position;
    public Seat(Vector3 pos)
    {
        occupied = false;
        position = pos;
    }
}
