using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex 
{
    public GameObject go;
    public Vector3 location;
    public List<Room> rooms;
    public bool innerWall;
    public bool corner;
    public bool tJunct;
    public bool outerWall;
    public GridSpace gridSpace;
    public Vertex(GameObject go, Vector3 loc, GridSpace grid = new GridSpace())
    {
        this.go = go;
        this.location = loc;
        rooms = new List<Room>();
        innerWall = false;
        outerWall = false;
        corner = false;
        tJunct = false;
        gridSpace = grid;
    }
}
