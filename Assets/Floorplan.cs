using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Floorplan
{
    public List<Room> roomList;
    public GridSpace[,] gridSpaces;
    public int maxX;
    public int maxZ;
    public int minX;
    public int minZ;

    public Floorplan(GridSpace[,] grid, Room parentRoom, List<Room> rooms)
    {
        if(rooms == null)
        {
            roomList = new List<Room>();
        }
        else
        {

            roomList = rooms;
        }
        gridSpaces = grid;
        if(parentRoom.hash != 0)
        {
            maxX = parentRoom.GetMaxX();
            maxZ = parentRoom.GetMaxZ();
            minX = parentRoom.GetMinX();
            minZ = parentRoom.GetMinZ();
        }
    }
    public Floorplan(GridSpace[,] grid, int maxX, int maxZ, List<Room> rooms = null)
    {
        if (rooms == null)
        {
            roomList = new List<Room>();
        }
        else
        {

            roomList = rooms;
        }
        gridSpaces = grid;

            this.maxX = maxX;
            this.maxZ = maxZ;
        minX = 0;
            minZ = 0;
        
    }

}
