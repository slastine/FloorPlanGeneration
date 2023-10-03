using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct Room
{


    public List<GridSpace> gridSpaces;
    public int desiredX;
    public int desiredZ;
    public int maxX;
    public int maxZ;
    public List<Room> childRooms;
    public Vector3 startingPoint;
    public int size;
    public int hash;
    public Color color;
    public Room(int hash, int desiredX, int desiredZ, Color color)
    {
        gridSpaces = new List<GridSpace>();
        this.desiredX = desiredX;
        this.desiredZ = desiredZ;
        maxX = desiredX * 2;
        maxZ = desiredZ * 2;
        childRooms = new List<Room>();
        startingPoint = Vector3.zero;
        size = 1;
        this.hash = hash;
        this.color = color;
    }

    public int GetAspectRatio()
    {
        return GetCurrentX() / GetCurrentZ();
    }

    public int GetDesiredAspectRatio()
    {
        return desiredX / desiredZ;
    }

    public int GetCurrentX()
    {
        if(gridSpaces.Count == 0)
        {
            return 0;
        }
        return GetMaxX() - GetMinX();
    }

    public int GetCurrentZ()
    {
        if (gridSpaces.Count == 0)
        {
            return 0;
        }
        
        return GetMaxZ() - GetMinZ();
    }

    public int GetMinX()
    {
        if (gridSpaces.Count == 0)
        {
            return 0;
        }
        int minX = (int)gridSpaces.Min(x => x.gridLocation.x);
        return minX;
    }
    public int GetMinZ()
    {
        if (gridSpaces.Count == 0)
        {
            return 0;
        }
        int minZ = (int)gridSpaces.Min(x => x.gridLocation.z);
        return minZ;
    }

    public int GetMaxX()
    {
        if (gridSpaces.Count == 0)
        {
            return 0;
        }
        int minX = (int)gridSpaces.Max(x => x.gridLocation.x);
        return minX;
    }

    public int GetMaxZ()
    {
        if (gridSpaces.Count == 0)
        {
            return 0;
        }
        int minZ = (int)gridSpaces.Max(x => x.gridLocation.z);
        return minZ;
    }

    public int GetLongestXRow()
    {
        List<int> xSizes = new List<int>();
        for(int x = this.GetMinX(); x <= this.GetMaxX(); x++)
        {
            xSizes.Add((from GridSpace gridSpace in gridSpaces where gridSpace.gridLocation.x == x select gridSpace).Count());
        }
        Debug.Log("X list: " + xSizes.ToString());
        return xSizes.GroupBy(value => value)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .First();
    }

    public int GetRowLengthAtX(int x)
    {
        return ((from GridSpace gridSpace in gridSpaces where gridSpace.gridLocation.x == x select gridSpace).Count());
    }

    public int GetLongestZRow()
    {
        List<int> zSizes = new List<int>();
        for (int z = this.GetMinZ(); z <= this.GetMaxZ(); z++)
        {
            zSizes.Add((from GridSpace gridSpace in gridSpaces where gridSpace.gridLocation.z == z select gridSpace).Count());
        }
        Debug.Log("Z list: " + zSizes.ToString());
        return zSizes.GroupBy(value => value)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .First();
    }

    public int GetRowLengthAtZ(int z)
    {
        return ((from GridSpace gridSpace in gridSpaces where gridSpace.gridLocation.z == z select gridSpace).Count());
    }
}
