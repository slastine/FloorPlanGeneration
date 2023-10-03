using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Generation : MonoBehaviour
{
    public Vector3[,] gridSquares;
    public int xSize;
    public int zSize;
    public GridSpace[,] gridSpaces;
    public GameObject[,] grid;
    public GameObject plane;
    List<Vertex> vertices;
    public GameObject parent;
    public Material vertexMat;

    private void Start()
    {
        GenerateFloorPlan();
    }
    [ContextMenu("Generate Floor Plan")]
    public  void GenerateFloorPlan()
    {
        float timeSinceStart = Time.realtimeSinceStartup;
        Debug.ClearDeveloperConsole();
        parent = new GameObject();
        Floorplan plan = MakeTestFloorplan();
        
        BeginGeneration(plan);
        Debug.Log("Finish generate");
        Debug.Log("Generated in: " + (Time.realtimeSinceStartup - timeSinceStart));
        
        //vertices = new List<Vertex>();
        //SpawnVertices(plan);
    }

    private void Update()
    {
        for (int x = 0; x <= xSize; x++)
        {
            for (int z = 0; z <= zSize; z++)
            {
                //Debug.Log(gridSpaces[x, z].room.hash);
                if(gridSpaces[x, z].go.GetComponent<MeshRenderer>().material.color != Color.black)
                {
                    gridSpaces[x, z].go.GetComponent<MeshRenderer>().material.color = gridSpaces[x, z].room.color;
                }
               

            }
        }
    }


    public GridSpace[,] ConvertToArray(List<GridSpace> grid, Room room)
    {
        GridSpace[,] convertedGrid = new GridSpace[room.GetCurrentX() + 1, room.GetCurrentZ() + 1];
        //Debug.Log("Current X: " + room.GetCurrentX());
       // Debug.Log("Current Z: " + room.GetCurrentZ());
       // Debug.Log("Max X: " + room.GetMaxX());
        //Debug.Log("Min X: " + room.GetMinX());
       // Debug.Log("Max Z: " + room.GetMaxZ());
       // Debug.Log("Min Z: " + room.GetMinZ());
        for (int x = room.GetMinX(); x <= room.GetMaxX(); x++)
        {
            for(int z = room.GetMinZ(); z <= room.GetMaxZ(); z++)
            {
                if (room.gridSpaces.Where(r => r.gridLocation.x == x && r.gridLocation.z == z).Any())
                {
                    
                    GridSpace space = room.gridSpaces.Where(r => r.gridLocation.x == x && r.gridLocation.z == z).First();
                    space.room.hash = 0;
                    //Debug.Log("X is " + (int)space.gridLocation.x + " Z is " + (int)space.gridLocation.z);
                    convertedGrid[(int)space.gridLocation.x - room.GetMinX(), (int)space.gridLocation.z - room.GetMinZ()] = space;
                    //space.go.GetComponent<MeshRenderer>().material.color = Color.black;
                }
                    
            }
        }
        return convertedGrid;
    }
    public bool IsTooClose(Room room, GridSpace startingPlace, Floorplan plan)
    {
        for (int x = (int)startingPlace.gridLocation.x - room.desiredX; x < (int)startingPlace.gridLocation.x + room.desiredX + 1; x++)
        {
            for (int z = (int)startingPlace.gridLocation.z - room.desiredZ; z < (int)startingPlace.gridLocation.z + room.desiredZ + 1; z++)
            {
                if (x <= plan.maxX && z <= plan.maxZ && x >= plan.minX && z >= plan.minZ)
                {
                    if (gridSpaces[x, z].room.hash != 0)
                    {


                        Debug.Log("Is too close");
                        return false;
                    }


                }
                else return false;
                
            }
        }
        return true;
    }
    public Vertex ClosestVertex(Vector3 loc)
    {
        List<Vertex> vertexList = new List<Vertex>();
        vertexList.AddRange(from Vertex vertex in vertices where Vector3.Distance(loc, vertex.location) < .01f select vertex);
        vertexList.OrderBy(x => Vector3.Distance(loc, x.location));
        if (vertexList.Count <= 0) return null;
        else
        {
            return vertexList.First();
        }
        
    }
    public void SpawnVertices(Floorplan plan)
    {
        //vertices = new Vertex[(xSize * 2), (zSize * 2)];
        for(int x = plan.minX; x <= plan.maxX; x++)
        {
            for (int z = plan.minZ; z <= plan.maxZ; z++)
            {
                GridSpace checkSpace = gridSpaces[x, z];
                if(ClosestVertex(checkSpace.topLeftCorner) == null)
                {
                    Vertex newVertex = new Vertex(null, checkSpace.topLeftCorner, checkSpace);
                    if (z == zSize)
                    {
                        //Debug.Log("Upper wall");
                        newVertex.outerWall = true;
                    }
                    if (x == 0)
                    {
                        //Debug.Log("Left wall");
                        newVertex.outerWall = true;
                    }
                    newVertex.rooms.Add(checkSpace.room);
                    vertices.Add(newVertex);
                }
                else 
                {
                    ClosestVertex(checkSpace.topLeftCorner).rooms.Add(checkSpace.room);
                }
                if (ClosestVertex(checkSpace.topRightCorner) == null)
                {
                    Vertex newVertex = new Vertex(null, checkSpace.topRightCorner, checkSpace);
                    if (z == zSize)
                    {
                        //Debug.Log("Upper wall");
                        newVertex.outerWall = true;
                    }
                    if (x == xSize)
                    {
                        //Debug.Log("Right wall");
                        newVertex.outerWall = true;
                    }
                    newVertex.rooms.Add(checkSpace.room);
                    vertices.Add(newVertex);
                }
                else 
                {
                    ClosestVertex(checkSpace.topRightCorner).rooms.Add(checkSpace.room);
                }
                if (ClosestVertex(checkSpace.bottomLeftCorner) == null)
                {

                    Vertex newVertex = new Vertex(null, checkSpace.bottomLeftCorner, checkSpace);
                    if (x == 0)
                    {
                        //Debug.Log("Left wall");
                        newVertex.outerWall = true;
                    }
                    if (z == 0)
                    {
                        //Debug.Log("Bottom wall");
                        newVertex.outerWall = true;
                    }
                    newVertex.rooms.Add(checkSpace.room);
                        vertices.Add(newVertex);
                    
                    
                }
                else 
                {
                    ClosestVertex(checkSpace.bottomLeftCorner).rooms.Add(checkSpace.room);
                }
                if (ClosestVertex(checkSpace.bottomRightCorner) == null)
                {

                    Vertex newVertex = new Vertex(null, checkSpace.bottomRightCorner, checkSpace);
                    if (x == xSize)
                    {
                        //Debug.Log("Right wall");
                        newVertex.outerWall = true;
                    }
                    if (z == 0)
                    {
                        //Debug.Log("Bottom wall");
                        newVertex.outerWall = true;
                    }
                    newVertex.rooms.Add(checkSpace.room);
                    vertices.Add(newVertex);
                    
                       
                }
                else 
                {
                    ClosestVertex(checkSpace.bottomRightCorner).rooms.Add(checkSpace.room);
                }
            }
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].rooms = vertices[i].rooms.GroupBy(p => p.color)
                              .Select(g => g.First())
                              .ToList();
            
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = vertices[i].location;
                go.transform.localScale *= .2f;
                go.transform.parent = parent.transform;
                
                if (vertices[i].rooms.Count > 1 || vertices[i].outerWall)
                {
                    go.GetComponent<MeshRenderer>().material = vertexMat;
                }
                if (vertices[i].rooms.Count > 2)
                {
                    vertices[i].innerWall = true;
                }
                if (vertices[i].rooms.Count > 2)
                {
                    vertices[i].tJunct = true;
                }
            
            
        }




    }
    public GridSpace? IdealStartingLocation(Room room, Floorplan plan)
    {
        List<GridSpace> potentialSpaces = new List<GridSpace>();
        //Debug.Log("Ideal Starting Location");
        //Debug.Log("Upper bound X: " + plan.gridSpaces.GetUpperBound(0));
        //Debug.Log("Upper bound Z: " + plan.gridSpaces.GetUpperBound(1));
        //Debug.Log("Lower bound X: " + plan.gridSpaces.GetLowerBound(0));
        //Debug.Log("Lower bound Z: " + plan.gridSpaces.GetLowerBound(1));
        for (int x = plan.gridSpaces.GetLowerBound(0); x <= plan.gridSpaces.GetUpperBound(0); x++)
        {
            for (int z = plan.gridSpaces.GetLowerBound(1); z <= plan.gridSpaces.GetUpperBound(1); z++)
            {
                if (x <= plan.gridSpaces.GetUpperBound(0) && z <= plan.gridSpaces.GetUpperBound(1) && x >= plan.gridSpaces.GetLowerBound(0) && z >= plan.gridSpaces.GetLowerBound(1))
                {
                    GridSpace testGridSpace = gridSpaces[x, z];
                    if (!testGridSpace.outerWall && !testGridSpace.corner && (xSize - testGridSpace.gridLocation.x) >= room.desiredX / 2 && (zSize - testGridSpace.gridLocation.z) >= room.desiredZ / 2 && (testGridSpace.gridLocation.x) >= room.desiredX / 2 && (testGridSpace.gridLocation.z) >= room.desiredZ / 2)
                    {
                        if (IsTooClose(room, testGridSpace, plan))
                        {
                            potentialSpaces.Add(testGridSpace);
                        }
                    }
                }
                    

            }
        }
        if(potentialSpaces.Count > 0)
        {
            List<GridSpace> orderedSpaces = new List<GridSpace>();
            orderedSpaces.AddRange(potentialSpaces.OrderByDescending(x => DistanceFromOtherGridSpace(room, plan, x)));
            orderedSpaces = orderedSpaces.OrderBy(x => DistanceFromOuterWall(x, plan)).ToList();
            return orderedSpaces[0];
        }
        return null;
    }

    public GridSpace? IdealStartingLocation(Room room, Floorplan plan, Room parentRoom)
    {
        List<GridSpace> potentialSpaces = new List<GridSpace>();
        //Debug.Log("Ideal Starting Location");
        //Debug.Log("Max X: " + parentRoom.GetMaxX());
        //Debug.Log("Min X: " + parentRoom.GetMinX());
        //Debug.Log("Max Z: " + parentRoom.GetMaxZ());
        //Debug.Log("Min Z: " + parentRoom.GetMinZ());
        for (int x = parentRoom.GetMinX(); x <= parentRoom.GetMaxX(); x++)
        {
            for (int z = parentRoom.GetMinZ(); z <= parentRoom.GetMaxZ(); z++)
            {
                if (parentRoom.gridSpaces.Where(r => r.gridLocation.x == x && r.gridLocation.z == z).Any())
                {
                    GridSpace testGridSpace = gridSpaces[x, z];
                    //Debug.Log("Test grid space " + testGridSpace.gridLocation);
                    if (!testGridSpace.outerWall && !testGridSpace.corner && (xSize - testGridSpace.gridLocation.x) >= room.desiredX /2 && (zSize - testGridSpace.gridLocation.z) >= room.desiredZ /2 && (testGridSpace.gridLocation.x) >= room.desiredX /2 && (testGridSpace.gridLocation.z) >= room.desiredZ /2)
                    {
                        if (IsTooClose(room, testGridSpace, plan))
                        {
                            potentialSpaces.Add(testGridSpace);
                            //Debug.Log("Potential space: " + testGridSpace.gridLocation);
                        }
                        //else
                        //{
                        //Debug.Log("Not close enough");
                        //}
                    }
                    else
                    {
                        //Debug.Log("Doesn't fit in constraints");
                    }
                }


            }
        }
        //Debug.Log("Potential spaces count: " + potentialSpaces.Count);
        if (potentialSpaces.Count > 0)
        {
            List<GridSpace> orderedSpaces = new List<GridSpace>();
            //orderedSpaces.AddRange(potentialSpaces.OrderByDescending(x => Mathf.Min(DistanceFromOtherGridSpace(room, plan, x), DistanceFromOuterWall(x, plan))));
            orderedSpaces = potentialSpaces.OrderBy(x => DistanceFromOuterWall(x, plan)).ToList();
            orderedSpaces.RemoveRange(orderedSpaces.Count / 2, orderedSpaces.Count - (orderedSpaces.Count /2));
            orderedSpaces = orderedSpaces.OrderByDescending(x => DistanceFromOtherGridSpace(room, plan, x)).ToList();
            
            return orderedSpaces[0];
        }
        return null;
    }


    public float DistanceFromOuterWall(GridSpace space, Floorplan plan)
    {
        float distanceFromTop = plan.maxX - space.gridLocation.x;
        float distanceFromBottom = plan.minX + space.gridLocation.x;
        float distanceFromLeft = plan.minZ + space.gridLocation.z;
        float distanceFromRight = plan.maxZ - space.gridLocation.z;
        return Mathf.Min(distanceFromBottom, distanceFromLeft, distanceFromRight, distanceFromTop);
    }

    public float DistanceFromOtherGridSpace(Room room, Floorplan plan, GridSpace space)
    {
        List<GridSpace> nonWhiteGridspaces = new List<GridSpace>();
        nonWhiteGridspaces.AddRange(from GridSpace gridSpace in gridSpaces where gridSpace.room.hash != 0  select gridSpace);
        List<GridSpace> orderedSpaces = new List<GridSpace>();
        orderedSpaces.AddRange(nonWhiteGridspaces.OrderBy(x => Vector3.Distance(space.gridLocation, x.gridLocation)));
        if(nonWhiteGridspaces.Count > 0)
        {
            
            return Vector3.Distance(space.gridLocation, orderedSpaces[0].gridLocation);
        }
        return 0;
    }

    public Floorplan MakeTestFloorplan()
    {
        GridSpace[,] gridSpace = new GridSpace[xSize + 1, zSize + 1];
        Floorplan plan = new Floorplan(gridSpace, xSize, zSize);
        plan.roomList.Add(new Room(GetHashCode() + 1, 5, 5, Color.red));
        plan.roomList.Add(new Room(GetHashCode() + 2, 5, 5, Color.cyan));
        plan.roomList[0].childRooms.Add(new Room(GetHashCode() + 3, 3, 3, Color.green));
        plan.roomList[0].childRooms.Add(new Room(GetHashCode() + 4, 3, 3, Color.magenta));
        plan.roomList[0].childRooms.Add(new Room(GetHashCode() + 5, 3, 3, Color.cyan));
        plan.roomList[1].childRooms.Add(new Room(GetHashCode() + 6, 3, 6, Color.gray));
        plan.roomList[1].childRooms.Add(new Room(GetHashCode() + 7, 3, 6, Color.blue));
        return plan;
    }
    public bool CanExpandAny(Floorplan plan)
    {
        for (int i = 0; i < plan.roomList.Count; i++)
        {
            if (CanExpandAll(gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z], plan.roomList[i].size, plan.roomList[i], plan))
            {
                return true;
            }

        }
        //Debug.Log("Can't expand any");
        return false;
    }
    public bool CanFillAny(Floorplan plan)
    {
        for (int i = 0; i < plan.roomList.Count; i++)
        {
            if (CanFillRectangular(plan.roomList[i], plan))
            {
                return true;
            }

        }
        return false;
    }
    public void CreateIdealStartingLocations(Floorplan plan)
    {
        int successCount = 0;
        int attemptCount = 0;
        List<GridSpace> changedSpaces = new List<GridSpace>();
        while(successCount < plan.roomList.Count && attemptCount < 10)
        {
            successCount = 0;
            attemptCount++;
            for (int i = 0; i < plan.roomList.Count; i++)
            {
                GridSpace? start = IdealStartingLocation(plan.roomList[i], plan);
                if (start != null)
                {
                    GridSpace startingSpace = (GridSpace)start;
                    gridSpaces[(int)startingSpace.gridLocation.x, (int)startingSpace.gridLocation.z].room = plan.roomList[i];
                    Room newRoom = plan.roomList[i];
                    newRoom.startingPoint = startingSpace.gridLocation;
                    changedSpaces.Add(startingSpace);
                    plan.roomList[i] = newRoom;
                    successCount++;
                    
                }

            }
            for(int i = 0; i < changedSpaces.Count; i++)
            {
                GridSpace space = gridSpaces[(int)changedSpaces[i].gridLocation.x, (int)changedSpaces[i].gridLocation.z];
                
                space.room = new Room();
                gridSpaces[(int)changedSpaces[i].gridLocation.x, (int)changedSpaces[i].gridLocation.z] = space;
            }
        }
        for (int i = 0; i < plan.roomList.Count; i++)
        {
            //rooms.Add(plan.roomList[i]);
            //Debug.Log(plan.roomList[i].hash);
            gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].room = plan.roomList[i];
            //gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].go.GetComponent<MeshRenderer>().material.color = plan.roomList[i].color;

        }
    }

    public void CreateIdealStartingLocations(Floorplan plan, Room parentRoom)
    {
        int successCount = 0;
        int attemptCount = 0;
        
        List<GridSpace> changedSpaces = new List<GridSpace>();
        while (successCount < plan.roomList.Count && attemptCount < 10)
        {
            successCount = 0;
            attemptCount++;
            for (int i = 0; i < plan.roomList.Count; i++)
            {
                GridSpace? start = IdealStartingLocation(plan.roomList[i], plan, parentRoom);
                if (start != null)
                {
                    GridSpace startingSpace = (GridSpace)start;
                    //Debug.Log("Starting space is " + startingSpace.gridLocation);
                    gridSpaces[(int)startingSpace.gridLocation.x, (int)startingSpace.gridLocation.z].room  = plan.roomList[i];
                    Room newRoom = plan.roomList[i];
                    newRoom.startingPoint = startingSpace.gridLocation;
                    newRoom.gridSpaces.Add(startingSpace);
                    changedSpaces.Add(startingSpace);
                    plan.roomList[i] = newRoom;
                    successCount++;
                }

            }
            for (int i = 0; i < changedSpaces.Count; i++)
            {
                GridSpace space = gridSpaces[(int)changedSpaces[i].gridLocation.x, (int)changedSpaces[i].gridLocation.z];

                space.room = new Room();
                gridSpaces[(int)changedSpaces[i].gridLocation.x, (int)changedSpaces[i].gridLocation.z] = space;
            }
            
        }
        //Debug.Log("Success count: " + successCount);
        for (int i = 0; i < plan.roomList.Count; i++)
        {
            //rooms.Add(plan.roomList[i]);
            gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].room = plan.roomList[i];
            //gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].go.GetComponent<MeshRenderer>().material.color = Color.black;

        }
    }

    public void BeginGeneration(Floorplan plan)
    {
        CreateGridSquares(plan);
        SpawnGridSquares(plan);
        Generate(plan);
    }

    public void Generate(Floorplan plan, Room parentRoom)
    {

        CreateIdealStartingLocations(plan, parentRoom);
        for (int i = 0; i < plan.roomList.Count; i++)
        {
            //rooms.Add(plan.roomList[i]);
            //Debug.Log(plan.roomList[i].hash);
            gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].room = plan.roomList[i];
            //gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].go.GetComponent<MeshRenderer>().material.color = Color.black;

        }

        
            List<Room> sortedRoomList = new List<Room>();
            sortedRoomList.AddRange(plan.roomList.OrderBy(x => RoomToGrow(x, plan)));
            plan.roomList = sortedRoomList;
            for (int i = 0; i < plan.roomList.Count; i++)
            {
                if (CanExpandAll(gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z], plan.roomList[i].size, plan.roomList[i], plan))
                {

                    ExpandFrom(gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z], plan.roomList[i].size, plan.roomList[i], plan);
                    Room newRoom = plan.roomList[i];
                    newRoom.size++;
                    plan.roomList[i] = newRoom;
                }

            }
        

        for (int i = 0; i < plan.roomList.Count; i++)
        {
            gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].go.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        //Continue(plan);
        StartCoroutine(Continue(plan));

    }
    public void Generate(Floorplan plan)
    {
        
        CreateIdealStartingLocations(plan);
        for (int i = 0; i < plan.roomList.Count; i++)
        {
            //rooms.Add(plan.roomList[i]);
            //Debug.Log(plan.roomList[i].hash);
            gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].room = plan.roomList[i];
            //gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z].go.GetComponent<MeshRenderer>().material.color = Color.black;

        }

        while(CanExpandAny(plan))
        {
            List<Room> sortedRoomList = new List<Room>();
            sortedRoomList.AddRange(plan.roomList.OrderBy(x => RoomToGrow(x, plan)));
            plan.roomList = sortedRoomList; 
            for (int i = 0; i < plan.roomList.Count; i++)
            {
                if (CanExpandAll(gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z], plan.roomList[i].size, plan.roomList[i], plan))
                {

                    ExpandFrom(gridSpaces[(int)plan.roomList[i].startingPoint.x, (int)plan.roomList[i].startingPoint.z], plan.roomList[i].size, plan.roomList[i], plan);
                    Room newRoom = plan.roomList[i];
                    newRoom.size++;
                    plan.roomList[i] = newRoom;
                }
               
            }
        }
      
        //Continue(plan);
        StartCoroutine(Continue(plan));
       
    }
    public IEnumerator Continue(Floorplan plan)
    {
        float startTime = Time.realtimeSinceStartup;
        List<GridSpace> whiteGridSpaces = new List<GridSpace>();
        whiteGridSpaces.AddRange(from GridSpace gridSpace in gridSpaces where gridSpace.room.hash == 0 select gridSpace);
        bool expanding = true;
        while(whiteGridSpaces.Count > 0 && expanding)
        {
            whiteGridSpaces = new List<GridSpace>();
            whiteGridSpaces.AddRange(from GridSpace gridSpace in gridSpaces where gridSpace.room.hash == 0 select gridSpace);
            int longestSpace = 0;
            float longestWallTime = Time.realtimeSinceStartup;
            /*for (int i = 0; i < plan.roomList.Count; i++)
            {
                if(FindLongestWall(plan.roomList[i], plan) > FindLongestWall(plan.roomList[longestSpace], plan))
                {
                    longestSpace = i;
                }
                
            }
            Debug.Log("Find and expand longest wall time: " + (Time.realtimeSinceStartup - longestWallTime));
           
            while (ExpandLongestWall(plan.roomList[longestSpace], true, plan))
            {
                //Debug.Log("Expand longest wall");
            }*/
            if (CanFillAny(plan))
            {
                List<Room> sortedRoomList = new List<Room>();
                sortedRoomList.AddRange(plan.roomList.OrderByDescending(x => RoomToGrow(x, plan)));
                for (int i = 0; i < sortedRoomList.Count; i++)
                {

                    Fill(sortedRoomList[i], true, plan);
                    Debug.Log("Second else");
                }
            }
            else
            {
                for (int i = 0; i < plan.roomList.Count; i++)
                {

                    while (Fill(plan.roomList[i], false, plan)) ;
                    //Debug.Log("Second else");
                }


            }
            //Debug.Log("Generating...");
            yield return new WaitForSeconds(.1f);
        }
        Debug.Log("Continue one level: " + (Time.realtimeSinceStartup - startTime));
            for (int i = 0; i < plan.roomList.Count; i++)
            {
                if (plan.roomList[i].childRooms.Count > 0)
                {
                    Floorplan newPlan = new Floorplan(ConvertToArray(plan.roomList[i].gridSpaces, plan.roomList[i]), plan.roomList[i], plan.roomList[i].childRooms);
                    UpdateGridSquares(newPlan, plan.roomList[i]);
                    //CreateIdealStartingLocations(newPlan, plan.roomList[i]);
                    Generate(newPlan, plan.roomList[i]);
                }
            }

    }

    public float RoomToGrow(Room room, Floorplan plan)
    {
        if (!CanFillRectangular(room, plan)) return -100;
        float desiredArea = room.desiredX * room.desiredZ;
        float currentArea = room.GetCurrentX() * room.GetCurrentZ();
        return desiredArea - currentArea;
    }

    public bool ExpandLongestWall(Room room, bool lShape, Floorplan plan)
    {
        List<GridSpace> topWalls = new List<GridSpace>();
        List<GridSpace> bottomWalls = new List<GridSpace>();
        List<GridSpace> leftWalls = new List<GridSpace>();
        List<GridSpace> rightWalls = new List<GridSpace>();
        for (int i = 0; i < room.gridSpaces.Count; i++)
        {
            if (room.gridSpaces[i].gridLocation.z == room.GetMaxZ())
            {
                topWalls.Add(room.gridSpaces[i]);
            }
            if (room.gridSpaces[i].gridLocation.z == room.GetMinZ())
            {
                bottomWalls.Add(room.gridSpaces[i]);
            }
            if (room.gridSpaces[i].gridLocation.x == room.GetMinX())
            {
                leftWalls.Add(room.gridSpaces[i]);
            }
            if (room.gridSpaces[i].gridLocation.x == room.GetMaxX())
            {
                rightWalls.Add(room.gridSpaces[i]);
            }
        }
        List<List<GridSpace>> walls = new List<List<GridSpace>>();
        walls.Add(topWalls);
        walls.Add(bottomWalls);
        walls.Add(rightWalls);
        walls.Add(leftWalls);


        int longest = 0;
        for (int i = 0; i < walls.Count; i++)
        {
            if (GrowthPotential(walls[i], room, plan) > GrowthPotential(walls[longest], room, plan))
            {
                longest = i;
            }


        }
        switch(longest)
        {
            case 0:
                if (CanExpandUp(room, lShape, plan))
                {
                    return ExpandUp(room, plan);
                }
                else return false;
            case 1:
                if(CanExpandDown(room, lShape, plan))
                {
                    return ExpandDown(room, plan);
                }
                else return false;
            case 2:
                if(CanExpandRight(room, lShape, plan))
                {
                    return ExpandRight(room, plan);
                }
                else return false;
            case 3:
                if(CanExpandLeft(room, lShape, plan))
                {
                    return ExpandLeft(room, plan);
                }
                else return false;
            default:
                break;
        }
        return true;
    }
    public int FindLongestWall(Room room, Floorplan plan)
    {
        List<GridSpace> topWalls = new List<GridSpace>();
        List<GridSpace> bottomWalls = new List<GridSpace>();
        List<GridSpace> leftWalls = new List<GridSpace>();
        List<GridSpace> rightWalls = new List<GridSpace>();
        //topWalls.AddRange(from GridSpace gridSpace in room.gridSpaces where gridSpace.gridLocation.z == room.GetMaxZ() select gridSpace);
        //bottomWalls.AddRange(from GridSpace gridSpace in room.gridSpaces where gridSpace.gridLocation.z == room.GetMinZ() select gridSpace);
        //rightWalls.AddRange(from GridSpace gridSpace in room.gridSpaces where gridSpace.gridLocation.x == room.GetMaxX() select gridSpace);
        //leftWalls.AddRange(from GridSpace gridSpace in room.gridSpaces where gridSpace.gridLocation.x == room.GetMinX() select gridSpace);
        for(int i = 0; i < room.gridSpaces.Count; i++)
        {
            if(room.gridSpaces[i].gridLocation.z == room.GetMaxZ())
            {
                topWalls.Add(room.gridSpaces[i]);
            }
            if (room.gridSpaces[i].gridLocation.z == room.GetMinZ())
            {
                bottomWalls.Add(room.gridSpaces[i]);
            }
            if (room.gridSpaces[i].gridLocation.x == room.GetMinX())
            {
                leftWalls.Add(room.gridSpaces[i]);
            }
            if (room.gridSpaces[i].gridLocation.x == room.GetMaxX())
            {
                rightWalls.Add(room.gridSpaces[i]);
            }
        }
        
        List<List<GridSpace>> walls = new List<List<GridSpace>>();
        walls.Add(topWalls);
        walls.Add(bottomWalls);
        walls.Add(rightWalls);
        walls.Add(leftWalls);

        
        int longest = 0;
        for(int i = 0; i < walls.Count; i++)
        {
            if(GrowthPotential(walls[i], room, plan) > GrowthPotential(walls[longest], room, plan))
            {
                longest = i;
            }
            
            
        }
        return longest;
    }
    public int GrowthPotential(List<GridSpace> wall, Room room, Floorplan plan)
    {
        int growthPotential = 0;
        for (int j = 0; j < wall.Count; j++)
        {
            if (!IsAdjacentToOtherRoom(room, wall[j], plan))
            {
                
                growthPotential++;
            }
        }
        if (!CanGrow(room)) return -1;
        return growthPotential;
    }
    public void CreateGridSquares(Floorplan plan)
    {
        gridSquares = new Vector3[plan.gridSpaces.GetLength(0), plan.gridSpaces.GetLength(1)];
        for (int z = plan.gridSpaces.GetLowerBound(1); z <= plan.gridSpaces.GetUpperBound(1); z++)
        {
            for (int x = plan.gridSpaces.GetLowerBound(0); x <= plan.gridSpaces.GetUpperBound(0); x++)
            {
                gridSquares[x, z] = new Vector3(x, 0, z);
            }
        }
    }

    public void UpdateGridSquares(Floorplan plan, Room parentRoom)
    {
        for (int z = 0; z <= zSize; z++)
        {
            for (int x  =0; x <= xSize; x++)
            {
                if (x <= parentRoom.GetMaxX() && z <= parentRoom.GetMaxZ() && x >= parentRoom.GetMinX() && z >= parentRoom.GetMinZ())
                {
                    gridSpaces[x, z] = new GridSpace(new Vector3(x, 0, z), gridSpaces[x, z].go, false, false, false, new Room(0, 0, 0, Color.white));

                }
            }
        }
    }
    public void SpawnGridSquares(Floorplan plan)
    {
        float startTime = Time.realtimeSinceStartup;
        //grid = new GameObject[xSize + 1, zSize + 1];
        gridSpaces = new GridSpace[plan.gridSpaces.GetLength(0), plan.gridSpaces.GetLength(1)];
        for (int z = plan.gridSpaces.GetLowerBound(1); z <= plan.gridSpaces.GetUpperBound(1); z++)
        {
            for (int x = plan.gridSpaces.GetLowerBound(0); x <= plan.gridSpaces.GetUpperBound(0); x++)
            {
                GameObject go;
                    go = GameObject.Instantiate(plane);
                    go.transform.position = gridSquares[x, z];
                    go.transform.parent = parent.transform;
                    go.transform.localScale *= .1f;
                    //grid[x, z] = go;
                    
                gridSpaces[x, z] = new GridSpace(new Vector3(x, 0, z), go, false, false, false, new Room(0, 0, 0, Color.white));
                
                
                //GameObject go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //go1.transform.position = gridSpaces[x, z].topLeftCorner;
                //go1.transform.localScale *= .1f;
                gridSpaces[x, z].topLeft = new Vertex(null, gridSpaces[x, z].topLeftCorner);

                //GameObject go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //go2.transform.position = gridSpaces[x, z].topRightCorner;
               /// go2.transform.localScale *= .1f;
                gridSpaces[x, z].topRight = new Vertex(null, gridSpaces[x, z].topRightCorner);

                //GameObject go3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //go3.transform.position = gridSpaces[x, z].bottomRightCorner;
                //go3.transform.localScale *= .1f;
                gridSpaces[x, z].bottomRight = new Vertex(null, gridSpaces[x, z].bottomRightCorner);

                //GameObject go4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //go4.transform.position = gridSpaces[x, z].bottomLeftCorner;
                //go4.transform.localScale *= .1f;
                gridSpaces[x, z].bottomLeft = new Vertex(null, gridSpaces[x, z].bottomLeftCorner);

            }
        }
        Debug.Log("Spawn grid squares: " + (Time.realtimeSinceStartup - startTime));
    }
    public void FindOuterWalls()
    {
        List<GridSpace> outerWallsList = new List<GridSpace>();
        outerWallsList.AddRange(from GridSpace square in gridSpaces where square.gridLocation.x == 0 || square.gridLocation.x == xSize || square.gridLocation.z == 0 || square.gridLocation.z == zSize select square);
        for (int i = 0; i < outerWallsList.Count; i++)
        {
            //outerWallsList[i].go.GetComponent<MeshRenderer>().material.color = Color.red;
            GridSpace newSpace = outerWallsList[i];
            newSpace.outerWall = true;
            outerWallsList[i] = newSpace;
            gridSpaces[(int)newSpace.gridLocation.x, (int)newSpace.gridLocation.z] = newSpace;
        }
    }
    public void FindCorners()
    {
        List<GridSpace> cornersList = new List<GridSpace>();
        cornersList.AddRange(from GridSpace square in gridSpaces where square.gridLocation.x == 0 && square.gridLocation.z == 0 select square);
        cornersList.AddRange(from GridSpace square in gridSpaces where square.gridLocation.x == 0 && square.gridLocation.z == zSize select square);
        cornersList.AddRange(from GridSpace square in gridSpaces where square.gridLocation.x == xSize && square.gridLocation.z == 0 select square);
        cornersList.AddRange(from GridSpace square in gridSpaces where square.gridLocation.x == xSize && square.gridLocation.z == zSize select square);
        for (int i = 0; i < cornersList.Count; i++)
        {
            //cornersList[i].go.GetComponent<MeshRenderer>().material.color = Color.black;
            GridSpace newSpace = cornersList[i];
            newSpace.corner = true;
            cornersList[i] = newSpace;
            gridSpaces[(int)newSpace.gridLocation.x, (int)newSpace.gridLocation.z] = newSpace;
        }
    }
    public Room ExpandFrom(GridSpace center, int size, Room room, Floorplan plan)
    {
        for (int x = (int)center.gridLocation.x - size; x < (int)center.gridLocation.x + size + 1; x++)
        {
            for (int z = (int)center.gridLocation.z - size; z < (int)center.gridLocation.z + size + 1; z++)
            {
                if (x <= plan.maxX && z <= plan.maxZ && x >= plan.minX && z >= plan.minZ)
                {
                    if (gridSpaces[x, z].room.hash == 0)
                    {
                        //gridSpaces[x, z].go.GetComponent<MeshRenderer>().material.color = room.color;
                        GridSpace newSpace = gridSpaces[x, z];
                        newSpace.room = room;
                        room.gridSpaces.Add(newSpace);
                        gridSpaces[x, z] = newSpace;
                        //Room newRoom = room;
                        //room.size = size + 1;
                        //room = newRoom;
                    }


                }
            }
        }
        //Debug.Log(room.gridSpaces.Count);
        return room;
    }
    public bool CanExpandAll(GridSpace center, int size, Room room, Floorplan plan)
    {
        List<GridSpace> expansionPoints = new List<GridSpace>();
        for (int x = (int)center.gridLocation.x - size; x < (int)center.gridLocation.x + size + 1; x++)
        {
            for (int z = (int)center.gridLocation.z - size; z < (int)center.gridLocation.z + size + 1; z++)
            {
                if (x <= plan.maxX && z <= plan.maxZ && x >= plan.minX && z >= plan.minZ)
                {
                    //Debug.Log(size);
                    if (gridSpaces[x, z].room.hash == 0)
                    {
                        expansionPoints.Add(gridSpaces[x, z]);
                    }
                    else if(gridSpaces[x, z].room.hash != room.hash)
                    {
                        //Debug.Log("Not right color");
                        return false;
                    }
                    //return false;
                }
            }
        }
        if (expansionPoints.Count > 0) return true;
        return false;
    }
    public bool ExpandUp(Room room, Floorplan plan)
    {

        int maxZ = (int)room.gridSpaces.Max(x => x.gridLocation.z);
        List<GridSpace> upSpaces = new List<GridSpace>();
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ select gridSpace);
        //upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ - 1 select gridSpace);
        bool success = false;
        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.forward;

            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {
                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                //Debug.Log("Expand up at " + checkPosition);
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {
                    //checkSpace.go.GetComponent<MeshRenderer>().material.color = room.color;
                    GridSpace newSpace = checkSpace;
                    newSpace.room = room;
                    room.gridSpaces.Add(newSpace);
                    gridSpaces[(int)checkSpace.gridLocation.x, (int)checkSpace.gridLocation.z] = newSpace;
                    success = true;
                }
            }

        }
        return success;
    }
    public bool CanExpandUp(Room room, bool all, Floorplan plan)
    {
        if (room.gridSpaces.Count == 0)
        {
            //return true;
        }
        int maxZ = (int)room.gridSpaces.Max(x => x.gridLocation.z);
        List<GridSpace> upSpaces = new List<GridSpace>();
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ select gridSpace);
        upSpaces.OrderBy(x => x.gridLocation.x);
        List<GridSpace> validExpansionSpaces = new List<GridSpace>();
        //upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ - 1 select gridSpace);

        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.forward;

            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {
                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                //Debug.Log("*");
                //Debug.Log("Trying to expand up at " + checkPosition);
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {

                    if (!all)
                    {
                        return true;

                    }

                    validExpansionSpaces.Add(checkSpace);
                    if (GridSpacesConnected(validExpansionSpaces))
                    {
                        //Debug.Log("Up all connected");
                    }

                }
                else if (all) return false;
            }

        }
        if (all)
        {
            return true;
        }
        return false;
    }
    public bool GridSpacesConnected(List<GridSpace> gridSpaces)
    {
        for(int i = 0; i < gridSpaces.Count; i++)
        {
            GridSpace thisSpace = gridSpaces[i];
            Vector3 upPosition = thisSpace.gridLocation + Vector3.forward;
            Vector3 backPosition = thisSpace.gridLocation + Vector3.back;
            Vector3 leftPosition = thisSpace.gridLocation + Vector3.left;
            Vector3 rightPosition = thisSpace.gridLocation + Vector3.right;
            if(i != 0)
            {
                if (!(gridSpaces[i - 1].gridLocation == upPosition || gridSpaces[i - 1].gridLocation == backPosition || gridSpaces[i - 1].gridLocation == leftPosition || gridSpaces[i - 1].gridLocation == rightPosition))
                {
                    //Debug.Log(thisSpace.gridLocation + " not related to " + gridSpaces[i - 1].gridLocation);
                    return false;
                }
            }
            
        }
        return true;
    }
    public bool ExpandDown(Room room, Floorplan plan)
    {
        int maxZ = (int)room.gridSpaces.Min(x => x.gridLocation.z);
        List<GridSpace> upSpaces = new List<GridSpace>();
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ select gridSpace);
        //upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ - 1 select gridSpace);
        bool success = false;
        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.back;

            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {
                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                //Debug.Log("*");
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {
                    //Debug.Log("Expand down at " + checkPosition);
                    //checkSpace.go.GetComponent<MeshRenderer>().material.color = room.color;
                    GridSpace newSpace = checkSpace;
                    newSpace.room = room;
                    room.gridSpaces.Add(newSpace);
                    gridSpaces[(int)checkSpace.gridLocation.x, (int)checkSpace.gridLocation.z] = newSpace;
                    success = true;
                }
            }

        }
        return success;
    }
    public bool CanExpandDown(Room room, bool all, Floorplan plan)
    {
        if (room.gridSpaces.Count == 0)
        {
            return true;
        }
        int maxZ = (int)room.gridSpaces.Min(x => x.gridLocation.z);
        List<GridSpace> upSpaces = new List<GridSpace>();
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ select gridSpace);
        upSpaces.OrderBy(x => x.gridLocation.x);
        List<GridSpace> validExpansionSpaces = new List<GridSpace>();
        //upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.z == maxZ - 1 select gridSpace);

        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.back;

            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {
                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                //Debug.Log("*");
                //Debug.Log("Trying to expand down at " + checkPosition);
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {
                    if (!all)
                    {
                        return true;
                    }
                    validExpansionSpaces.Add(checkSpace);
                    if (GridSpacesConnected(validExpansionSpaces))
                    {
                        //Debug.Log("Down all connected");
                    }

                }
                else if (all) return false;
            }

        }
        if (all)
        {
            return true;
        }
        return false;
    }
    public bool ExpandLeft(Room room, Floorplan plan)
    {
        int maxZ = (int)room.gridSpaces.Min(x => x.gridLocation.x);
        List<GridSpace> upSpaces = new List<GridSpace>();
        bool success = false;
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.x == maxZ select gridSpace);
        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.left;
            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {


                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                //Debug.Log(checkSpace.room.color);
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {
                    //checkSpace.go.GetComponent<MeshRenderer>().material.color = room.color;
                    GridSpace newSpace = checkSpace;
                    newSpace.room = room;
                    room.gridSpaces.Add(newSpace);
                    gridSpaces[(int)checkSpace.gridLocation.x, (int)checkSpace.gridLocation.z] = newSpace;
                    success = true;
                    //Debug.Log("Expand left at " + checkPosition);
                }
            }

        }
        return success;
    }
    public bool CanExpandLeft(Room room, bool all, Floorplan plan)
    {
        if (room.gridSpaces.Count == 0)
        {
            return true;
        }
        int maxZ = (int)room.gridSpaces.Min(x => x.gridLocation.x);
        List<GridSpace> upSpaces = new List<GridSpace>();
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.x == maxZ select gridSpace);
        upSpaces.OrderBy(x => x.gridLocation.z);
        List<GridSpace> validExpansionSpaces = new List<GridSpace>();
        //upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.x == maxZ - 1 select gridSpace);

        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.left;
            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {


                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                //Debug.Log("Trying to expand left at " + checkPosition);
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {
                    if (!all)
                    {
                        return true;
                    }

                    validExpansionSpaces.Add(checkSpace);
                    if (GridSpacesConnected(validExpansionSpaces))
                    {
                        //Debug.Log("Left all connected");
                    }
                }
                else if (all) return false;

            }

        }
        if (all)
        {
            return true;
        }
        return false;
    }
    public bool ExpandRight(Room room, Floorplan plan)
    {
        int maxZ = (int)room.gridSpaces.Max(x => x.gridLocation.x);
        List<GridSpace> upSpaces = new List<GridSpace>();
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.x == maxZ select gridSpace);
        bool success = false;
        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.right;
            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {


                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {

                    //checkSpace.go.GetComponent<MeshRenderer>().material.color = room.color;
                    GridSpace newSpace = checkSpace;
                    newSpace.room = room;
                    room.gridSpaces.Add(newSpace);
                    gridSpaces[(int)checkSpace.gridLocation.x, (int)checkSpace.gridLocation.z] = newSpace;
                    success = true;
                    //Debug.Log("Expand right at " + checkPosition);
                }
            }

        }
        return success;
    }
    public bool CanExpandRight(Room room, bool all, Floorplan plan)
    {
        if (room.gridSpaces.Count == 0)
        {
            return true;
        }
        int maxZ = (int)room.gridSpaces.Max(x => x.gridLocation.x);
        List<GridSpace> upSpaces = new List<GridSpace>();
        upSpaces.AddRange(from gridSpace in room.gridSpaces where gridSpace.gridLocation.x == maxZ select gridSpace);
        upSpaces.OrderBy(x => x.gridLocation.z);
        List<GridSpace> validExpansionSpaces = new List<GridSpace>();
        for (int i = 0; i < upSpaces.Count; i++)
        {
            Vector3 checkPosition = upSpaces[i].gridLocation + Vector3.right;
            if (checkPosition.x <= plan.maxX && checkPosition.z <= plan.maxZ && checkPosition.x >= plan.minX && checkPosition.z >= plan.minZ)
            {
                //Debug.Log("Trying to expand right at " + checkPosition);
                GridSpace checkSpace = gridSpaces[(int)checkPosition.x, (int)checkPosition.z];
                if (checkSpace.room.hash == 0 && IsAdjacentToSelf(room, checkSpace, plan))
                {
                    if (!all)
                    {
                        return true;
                    }
                    validExpansionSpaces.Add(checkSpace);
                    if (GridSpacesConnected(validExpansionSpaces))
                    {
                    }

                }
                else if (all) return false;
            }

        }
        if (all)
        {
            return true;
        }
        return false;
    }
    public bool CanGrow(Room room)
    {
        if (room.GetCurrentZ() == 0) return true;
        if (room.GetCurrentX() / room.GetCurrentZ() <= room.GetDesiredAspectRatio()) return true;
        if (room.GetCurrentX() + 1 < room.GetMaxX() && room.GetCurrentZ() + 1 < room.GetMaxZ()) return true;
        return false;
    }

    public bool CanFillRectangular(Room room, Floorplan plan)
    {
        if (CanExpandDown(room, true, plan)) return true;
        if (CanExpandLeft(room, true, plan)) return true;
        if (CanExpandRight(room, true, plan)) return true;
        if (CanExpandUp(room, true, plan)) return true;
        return false;
    } 
    public bool Fill(Room room, bool lShape, Floorplan plan)
    {
        if (!lShape && !CanGrow(room)) return false;
        bool success = false;
        if (CanExpandUp(room, lShape, plan))
        {
            if (ExpandUp(room, plan)) success = true; 
        }
        if (CanExpandDown(room, lShape, plan))
        {
            if(ExpandDown(room, plan)) success = true;
        }
        if (CanExpandRight(room, lShape, plan))
        {
            if(ExpandRight(room, plan)) success = true;
        }
        if (CanExpandLeft(room, lShape, plan))
        {
            if(ExpandLeft(room, plan)) success = true;
        }
        return success;
    }
    public bool IsAdjacentToSelf(Room room, GridSpace checkSpace, Floorplan plan)
    {
        Vector3 checkSpaceUp = checkSpace.gridLocation + Vector3.forward;
        Vector3 checkSpaceDown = checkSpace.gridLocation + Vector3.back;
        Vector3 checkSpaceRight = checkSpace.gridLocation + Vector3.right;
        Vector3 checkSpaceLeft = checkSpace.gridLocation + Vector3.left;
        if (checkSpaceUp.x <= plan.maxX && checkSpaceUp.z <= plan.maxZ && checkSpaceUp.x >= plan.minX && checkSpaceUp.z >= plan.minZ)
        {
            if (gridSpaces[(int)checkSpaceUp.x, (int)checkSpaceUp.z].room.hash == room.hash)
            {
                return true;
            }
        }
        if (checkSpaceDown.x <= plan.maxX && checkSpaceDown.z <= plan.maxZ && checkSpaceDown.x >= plan.minX && checkSpaceDown.z >= plan.minZ)
        {
            if (gridSpaces[(int)checkSpaceDown.x, (int)checkSpaceDown.z].room.hash == room.hash)
            {
                return true;
            }
        }
        if (checkSpaceRight.x <= plan.maxX && checkSpaceRight.z <= plan.maxZ && checkSpaceRight.x >= plan.minX && checkSpaceRight.z >= plan.minZ)
        {
            if (gridSpaces[(int)checkSpaceRight.x, (int)checkSpaceRight.z].room.hash == room.hash)
            {
                return true;
            }
        }
        if (checkSpaceLeft.x <= plan.maxX && checkSpaceLeft.z <= plan.maxZ && checkSpaceLeft.x >= plan.minX && checkSpaceLeft.z >= plan.minZ)
        {
            if (gridSpaces[(int)checkSpaceLeft.x, (int)checkSpaceLeft.z].room.hash == room.hash)
            {
                return true;
            }
        }
        //Debug.Log(checkSpace + " not adjacent to self");
        return false;
    }
    public bool IsAdjacentToOtherRoom(Room room, GridSpace checkSpace, Floorplan plan)
    {
        Vector3 checkSpaceUp = checkSpace.gridLocation + Vector3.forward;
        Vector3 checkSpaceDown = checkSpace.gridLocation + Vector3.back;
        Vector3 checkSpaceRight = checkSpace.gridLocation + Vector3.right;
        Vector3 checkSpaceLeft = checkSpace.gridLocation + Vector3.left;
        if (checkSpaceUp.x <= plan.gridSpaces.GetUpperBound(0) && checkSpaceUp.z <= plan.gridSpaces.GetUpperBound(1) && checkSpaceUp.x >= plan.gridSpaces.GetLowerBound(0) && checkSpaceUp.z >= plan.gridSpaces.GetLowerBound(1))
        {
            if (gridSpaces[(int)checkSpaceUp.x, (int)checkSpaceUp.z].room.hash != room.hash && gridSpaces[(int)checkSpaceUp.x, (int)checkSpaceUp.z].room.hash != 0)
            {
                return true;
            }
        }
        else return true;

        if (checkSpaceDown.x <= plan.gridSpaces.GetUpperBound(0) && checkSpaceDown.z <= plan.gridSpaces.GetUpperBound(1) && checkSpaceDown.x >= plan.gridSpaces.GetLowerBound(0) && checkSpaceDown.z >= plan.gridSpaces.GetLowerBound(1))
        {
            if (gridSpaces[(int)checkSpaceDown.x, (int)checkSpaceDown.z].room.hash != room.hash && gridSpaces[(int)checkSpaceDown.x, (int)checkSpaceDown.z].room.hash != 0)
            {
                return true;
            }
        }
        else return true;
        if (checkSpaceRight.x <= plan.gridSpaces.GetUpperBound(0) && checkSpaceRight.z <= plan.gridSpaces.GetUpperBound(1) && checkSpaceRight.x >= plan.gridSpaces.GetLowerBound(0) && checkSpaceRight.z >= plan.gridSpaces.GetLowerBound(1))
        {
            if (gridSpaces[(int)checkSpaceRight.x, (int)checkSpaceRight.z].room.hash != room.hash && gridSpaces[(int)checkSpaceRight.x, (int)checkSpaceRight.z].room.hash != 0)
            {
                return true;
            }
        }
        else return true;
        if (checkSpaceLeft.x <= plan.gridSpaces.GetUpperBound(0) && checkSpaceLeft.z <= plan.gridSpaces.GetUpperBound(1) && checkSpaceLeft.x >= plan.gridSpaces.GetLowerBound(0) && checkSpaceLeft.z >= plan.gridSpaces.GetLowerBound(1))
        {
            if (gridSpaces[(int)checkSpaceLeft.x, (int)checkSpaceLeft.z].room.hash != room.hash && gridSpaces[(int)checkSpaceLeft.x, (int)checkSpaceLeft.z].room.hash != 0)
            {
                return true;
            }
        }
        else return true;
        return false;
    }

    

    

  

}