using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// Randomly generates a 2D platformer level
public class LevelGenerator : MonoBehaviour
{
    public int ROOMS_NUMBER = 6;

    public List<Room> rooms = new List<Room>();
    public List<Vector3> solution = new List<Vector3>(); // All major point in the solution path
    public List<Vector3> solutionLinks = new List<Vector3>(); // All platforms in-between points composing the solution path
    public List<Vector3> platforms = new List<Vector3>(); // all random platforms
    public List<Vector3> globalPlatforms = new List<Vector3>(); // All platforms, including solutions platforms

    protected List<Vector3> roomsToVector3 = new List<Vector3>(); // Coordinates of the rooms inside a 6x5 2D array

    // Represents a room of 20x16 2D Array
    public class Room
    {
        public static Vector2 size = new Vector2(20, 16);
        
        public Vector2 position;
        
        public List<Walls> wallList = new List<Walls>();
        public List<Vector3> platformList = new List<Vector3>();

        public RoomType type;

        public Room prevRoom;
        public Room nextRoom;

        public enum Walls {down, up, left, right};
        public enum RoomType {start, end, path};

        public Room(Vector3 pos, RoomType rt, Room prvRoom, params Walls[] _wall)
        {
            position = pos;
            type = rt;

            prevRoom = prvRoom;

            foreach(Walls item in _wall)
                wallList.Add(item);
        }

    }

    // Returns a possible position next to 'targetRoom'
    public Vector3 FindPath(Room targetRoom)
    {
        List<int> possibilities = new List<int>();

        int i;

        Debug.Log("Setting possibilities int");

        for(i = 0; i < 4; i++)
            possibilities.Add(i);

        if(targetRoom.position.x == 0)
            possibilities.Remove(2);

        if(targetRoom.position.y == 0)
            possibilities.Remove(0);

        if(targetRoom.position.x == 6)
            possibilities.Remove(3);

        if(targetRoom.position.y == 5)
            possibilities.Remove(1);

        int _dir;

        Debug.Log("Random testing possibilities");


        Debug.Log("Possible positions before prevRoom check . . . ");
        foreach(int item in possibilities)
            Debug.Log(item.ToString());

        do
        {
    
            _dir = possibilities[(int)Random.Range(0, possibilities.Count)];

            // down
            if(_dir == 0)
                if(!targetRoom.wallList.Contains(Room.Walls.down) && targetRoom.position.y-1 >= 0)
                    if( !(roomsToVector3.Contains(new Vector3(targetRoom.position.x, targetRoom.position.y-1))) )
                        return new Vector3(targetRoom.position.x, targetRoom.position.y-1);            
            // up
            if(_dir == 1)
                if(!targetRoom.wallList.Contains(Room.Walls.up) && targetRoom.position.y+1 <= 5)
                    if( !(roomsToVector3.Contains(new Vector3(targetRoom.position.x, targetRoom.position.y+1))) )
                        return new Vector3(targetRoom.position.x, targetRoom.position.y+1);
            // left
            if(_dir == 2)
                if(!targetRoom.wallList.Contains(Room.Walls.left) && targetRoom.position.x-1 >= 0)
                    if(!(roomsToVector3.Contains(new Vector3(targetRoom.position.x-1, targetRoom.position.y))) )
                        return new Vector3(targetRoom.position.x-1, targetRoom.position.y);
            // right
            if(_dir == 3)
                if(!targetRoom.wallList.Contains(Room.Walls.right) && targetRoom.position.x+1 <= 6)
                    if(!(roomsToVector3.Contains(new Vector3(targetRoom.position.x+1, targetRoom.position.y))) )
                        return new Vector3(targetRoom.position.x+1, targetRoom.position.y);

            possibilities.Remove(_dir);
        } while(true);
    }

    // Creates starting room, (ROOMS_NUMBER-1) rooms, adding them walls
    public void InitRooms()
    {
        int i;
        int n_room = 0;

        List<int> possibilities = new List<int>();

        possibilities.Add(1);
        possibilities.Add(3);

        int randomPick = Random.Range(0,possibilities.Count);
        randomPick = possibilities[randomPick];

        Room oldRoom;
        Vector3 newRoomPos;

        List<Room.Walls> newRoomWalls = new List<Room.Walls>();

        rooms.Add(new Room(new Vector3(0, 0), Room.RoomType.start, null, Room.Walls.down, Room.Walls.left, (Room.Walls)randomPick));
        roomsToVector3.Add(new Vector3(0,0));
        ++n_room;

        // Creating the rest of the rooms
        while(n_room != ROOMS_NUMBER)
        {
            n_room++;

            Debug.Log(string.Format("Creating room {0}/{1} . . .", n_room, ROOMS_NUMBER));

            oldRoom = rooms[rooms.Count-1];
            Debug.Log("Done");
            Debug.Log(string.Format("Getting old room ({0}, {1}) -- [{2}, {3}]", oldRoom.position.x, oldRoom.position.y, oldRoom.position.x*20, oldRoom.position.y*16));


            Debug.Log("Getting path");
            do
            {
                newRoomPos = FindPath(oldRoom);
            } while(roomsToVector3.Contains(newRoomPos));
            
            Debug.Log("Done");

            Debug.Log("Clearing");

            possibilities.Clear();
            newRoomWalls.Clear();

            for(i = 0; i < 4; i++)
                possibilities.Add(i);

            Debug.Log("Getting Walls");
            while(newRoomWalls.Count != 2)
            {
                randomPick = Random.Range(0, possibilities.Count);

                if(possibilities.Count == 0)
                    randomPick = 0;

                if(newRoomPos.x+1 == oldRoom.position.x) // if we're on the LEFT of the previous room, ommiting the right wall
                {
                    possibilities.Remove(3);
                }
                if(newRoomPos.x-1 == oldRoom.position.x) // if we're on the RIGHT of the previous room, omitting the left wall
                {
                    possibilities.Remove(2);
                }
                if(newRoomPos.y+1 == oldRoom.position.y) // if we're UNDER the previous room, omitting TOP wall
                {
                    possibilities.Remove(1);
                }
                if(newRoomPos.y-1 == oldRoom.position.y) // if we're on TOP of the previous room, omitting DOWN wall
                {
                    possibilities.Remove(0);
                }

                // HACK: Works for max 5 rooms ;)
                // Depending on the room's position, some walls are obligatory
                if(newRoomPos.x == 0)
                    if(possibilities.Contains(2))
                    {
                        possibilities.Remove(2);
                        newRoomWalls.Add(Room.Walls.left);
                    }
                if(newRoomPos.x == 6)
                    if(possibilities.Contains(6))
                    {   
                        possibilities.Remove(1);
                        newRoomWalls.Add(Room.Walls.up);
                    }
                if(newRoomPos.y == 0)
                    if(possibilities.Contains(0))
                    {   
                        possibilities.Remove(0);
                        newRoomWalls.Add(Room.Walls.down);
                    }
                if(newRoomPos.y == 5)
                    if(possibilities.Contains(3))
                    {
                        possibilities.Remove(3);
                        newRoomWalls.Add(Room.Walls.right);
                    }     

                foreach(int item in possibilities)
                    Debug.Log(item.ToString());

                randomPick = possibilities[Random.Range(0, possibilities.Count)];


                // if our wall list does NOT contain '(Room.Walls)randomPick'
                if(!newRoomWalls.Contains((Room.Walls)randomPick))
                {
                    switch(randomPick)
                    {
                        case 0:
                            newRoomWalls.Add(Room.Walls.down);
                            break;
                        case 1:
                            newRoomWalls.Add(Room.Walls.up);
                            break;
                        case 2:
                            newRoomWalls.Add(Room.Walls.left);
                            break;
                        case 3:
                            newRoomWalls.Add(Room.Walls.right);
                            break;
                    }
                }

                // removing possibility for second check
                if(newRoomWalls.Count != 0)
                    possibilities.Remove(randomPick);
                else
                {
                    Debug.LogError("ERROR PICKING A WALL !");
                    return;
                }
            }

            Debug.Log(string.Format("New room added ! - ({0}, {1}) -- [{2}, {3}]", newRoomPos.x, newRoomPos.y, newRoomPos.x*20, newRoomPos.x*16));

            // Adding new room to our rooms layout
            rooms.Add(new Room(newRoomPos, Room.RoomType.path, oldRoom, newRoomWalls[0], newRoomWalls[1]));   
            roomsToVector3.Add(newRoomPos);
            
            oldRoom.nextRoom = rooms[rooms.Count-1];
        }

        // Adding a closing wall to our last room
        possibilities.Clear();

        for(i = 0; i < 4; i++)
            possibilities.Add(i);

        Room lastRoom = rooms[rooms.Count-1];

        // removing existing walls from possibilities
        possibilities.Remove((int)lastRoom.wallList[0]);
        possibilities.Remove((int)lastRoom.wallList[1]);

        if(lastRoom.prevRoom.position.x-1 == lastRoom.position.x) // if previous room is on our RIGHT, excluding RIGHT wall
        {
           if(possibilities.Contains(3))
                possibilities.Remove(3);
        }

        else if(lastRoom.prevRoom.position.x+1 == lastRoom.position.x) // if previous room is on our LEFT, excluding LEFT wall
        {
            if(possibilities.Contains(2))
                possibilities.Remove(2);
        }

        else if(lastRoom.prevRoom.position.y-1 == lastRoom.position.y) // if previous room is on TOP, excluding TOP wall
        {
            if(possibilities.Contains(1))
                possibilities.Remove(1);
        }

        else if(lastRoom.prevRoom.position.y+1 == lastRoom.position.y) // if previous room is UNDER, excluding DOWN wall
        { 
           if(possibilities.Contains(0))
                possibilities.Remove(0);
        }

        // Adding our last wall

        Debug.Log("Adding last wall . . .");

        lastRoom.wallList.Add((Room.Walls)possibilities[0]);

        Debug.Log("All rooms OK !");
    }    

    // Creates a feasible path from room to room
    // Starts at a random point in 'currentRoom' and find its way to another random point in 'nextRoom'
    // until end is reached
    public void InitPath()
    {
        int randomPick;
        
        // Used for the keypoints setting/finding
        Vector3 randPoint1, randPoint2, _pointBuffer;
        Vector3 _prevBuffer = new Vector3(-1, -1);

        // Used for the link-platforms between keypoints
        Vector3 nextKeyPoint;
        int index = 0;

        // Generating the path's key points, from room to room
        foreach(Room item in rooms)
        {
            if(item == rooms[rooms.Count-1])
                return;

            do
            {
                randPoint1 = new Vector3( (float)Mathf.Floor(Random.Range(0, Room.size.x)), (float)Mathf.Floor(Random.Range(0, Room.size.y)) );
                randPoint2 = new Vector3( (float)Mathf.Floor(Random.Range(0, Room.size.x)), (float)Mathf.Floor(Random.Range(0, Room.size.y)) );
            } while(randPoint1 == randPoint2 || randPoint1.x == randPoint2.x || randPoint1.y == randPoint2.y);



            randPoint1.x = (float)Mathf.Floor(randPoint1.x);
            randPoint1.y = (float)Mathf.Floor(randPoint1.y);

            solutionLinks.Add(randPoint1);
            solutionLinks.Add(randPoint2);

            _pointBuffer = randPoint1;

            Debug.Log("randPoint1 = (" + randPoint1.x + ", " + randPoint1.y + ")");
            Debug.Log("randPoint2 = (" + randPoint2.x + ", " + randPoint2.y + ")");

            while (_pointBuffer != randPoint2)
            {
                randomPick = Random.Range(0,2);

                if(_prevBuffer != new Vector3(-1, -1))
                    _pointBuffer = _prevBuffer; // starting at the former destination point

                if (randomPick == 0) // going on Y axis
                {
                    if (Mathf.Abs((_pointBuffer.y - randPoint2.y)) < (float)Mathf.Floor(_pointBuffer.y+5))
                        _pointBuffer.y = (float)Mathf.Floor(randPoint2.y);
                    else
                        _pointBuffer.y = (float)Mathf.Floor( Random.Range(_pointBuffer.y+1,_pointBuffer.y+5) );
                }
                else
                {
                    _pointBuffer.x = (float)Mathf.Floor(randPoint2.x);
                }

                _prevBuffer = _pointBuffer;
                item.platformList.Add(_pointBuffer);
                solution.Add(_pointBuffer); // keeping part of solution path
            }

        }

        // Generating platforms between all key points
        foreach(Room roomItem in rooms)
        {
            foreach(Vector3 keyPoint in roomItem.platformList)
            {
                nextKeyPoint = (roomItem.platformList[index]);

                while(keyPoint.x != nextKeyPoint.x)
                {
                    if(keyPoint.x < nextKeyPoint.x)
                        solutionLinks.Add(new Vector3(keyPoint.x+1, keyPoint.y));
                    else if(keyPoint.x > nextKeyPoint.x)
                        solutionLinks.Add(new Vector3(keyPoint.x-1, keyPoint.y));
                }

                while(keyPoint.y != nextKeyPoint.y)
                {
                    if(keyPoint.y < nextKeyPoint.y)
                        solutionLinks.Add(new Vector3(keyPoint.x, keyPoint.y+1));
                    else if(keyPoint.y > nextKeyPoint.y)
                        solutionLinks.Add(new Vector3(keyPoint.x, keyPoint.y-1));
                }
            }
        }
    }

    // Generates all platform, hiding the solution path
    public void GeneratePlatforms()
    {
        // Adding solution path to platforms positions        
        int x, y;
        int randomPick;

        Vector3 _pointBuffer = new Vector3();

        
        foreach(Room roomItem in rooms)
        {
            for(x = 0; x < Room.size.x+1; x++)
                for(y = 0; y < Room.size.y+1; y++)
                {
                    _pointBuffer.x = x;
                    _pointBuffer.y = y;

                    randomPick = Random.Range(0,2);

                    //FIXME: Chaotic !

                    switch(randomPick)
                    {
                        case 0:
                            roomItem.platformList.Add(_pointBuffer);
                            platforms.Add(_pointBuffer);
                            break;
                        default:
                            break;
                    }
                }
        }
    }

    public void Init()
    {
        Debug.LogWarning("INITIALIZATING ROOMS");
        InitRooms();

        Debug.LogWarning("INITIALIZATING SOLUTION PATH");
        InitPath();

        Debug.LogWarning("GENERATING PLATFORMS");
        GeneratePlatforms();
        
    }

}