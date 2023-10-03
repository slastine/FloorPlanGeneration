using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridSpace
{
    public Vector3 gridLocation;
    public GameObject go;
    public bool outerWall;
    public bool corner;
    public bool door;
    public Room room;
    public bool innerWall;
    public Vector3 topLeftCorner;
    public Vector3 topRightCorner;
    public Vector3 bottomLeftCorner;
    public Vector3 bottomRightCorner;
    public Vertex topLeft;
    public Vertex topRight;
    public Vertex bottomLeft;
    public Vertex bottomRight;

    public GridSpace(Vector3 gridLocation, GameObject go, bool outerWall, bool corner, bool door, Room room = new Room(), bool innerWall = false)
    {
        this.gridLocation = gridLocation;
        this.go = go;
        this.outerWall = outerWall;
        this.corner = corner;
        this.door = door;
        this.room = room;
        this.innerWall = innerWall;
        //topRightCorner = go.GetComponent<MeshFilter>().mesh.vertices[1];
        topRightCorner = Vector3.zero;
        topRightCorner += new Vector3(5f, 0, 5f); 
        topRightCorner = go.transform.TransformPoint(topRightCorner);
        topLeftCorner = Vector3.zero;
        topLeftCorner += new Vector3(-5f, 0, 5f);
        topLeftCorner = go.transform.TransformPoint(topLeftCorner);
        bottomRightCorner = Vector3.zero;
        bottomRightCorner += new Vector3(5f, 0, -5f);
        bottomRightCorner = go.transform.TransformPoint(bottomRightCorner);
        bottomLeftCorner = Vector3.zero;
        bottomLeftCorner += new Vector3(-5f, 0, -5f);
        bottomLeftCorner = go.transform.TransformPoint(bottomLeftCorner);
        topLeft = null;
        topRight = null;
        bottomLeft = null;
        bottomRight = null;
    }
}
