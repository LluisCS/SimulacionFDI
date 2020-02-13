using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
    public List<Room> rooms;
    public string name;
    public Floor(string n)
    {
        rooms = new List<Room>();
        name = n;
    }
}