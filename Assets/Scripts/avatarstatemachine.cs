using UnityEngine;
using System.Collections;

public class avatarstatemachine : MonoBehaviour
{
    public struct path
    {
        public Vector2[] singlepath;
        public int valid;
        public int indexinsidepath;
        public int actualX;
        public int actualY;
        public bool[,] roomchecked;
    }
    static path[] paths;
    static int itterations = 0;
    static int[] successfulpaths;
    static int lastusedpath;
    static int[,] roomfield;
    public const int maxsizepath = 200;

    static float avatarspeed = 150;

    [HideInInspector] public Vector2 avataractualposition;

    static Vector2[] finalpath;
    static int avatarwhereinpath = 0;
    static bool avatarmoving;

    [HideInInspector]
    static map_manager map_manager_local;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PathFindingInit()
    {
        int i, j, k;
        map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
        roomfield = new int[map_manager_local.mapsize, map_manager_local.mapsize];
        finalpath = new Vector2[maxsizepath];
        successfulpaths = new int[maxsizepath];

        paths = new path[maxsizepath];
        for (i = 0; i < maxsizepath; i++)
        {
            paths[i].singlepath = new Vector2[maxsizepath];
            paths[i].roomchecked = new bool[map_manager_local.mapsize, map_manager_local.mapsize];

            for (j = 0; j < map_manager_local.mapsize; j++)
                for (k = 0; k < map_manager_local.mapsize; k++)
                {
                    paths[i].roomchecked[j, k] = false;
                }
        }
    }

    public void PathFindingSetRooms(GameObject[,] originalmap)
    {
        int i, j;

        for (i = 0; i < map_manager_local.mapsize; i++)
            for (j = 0; j < map_manager_local.mapsize; j++)
            {
                if (originalmap[i, j] != null)
                {
                    roomfield[i, j] = 1;
                    //Debug.Log("Found map_piece: " + originalmap[i,j].name);
                }
                else
                {
                    roomfield[i, j] = 0;
                }
            }
    }

    public void SetCharacter(float x, float y)
    {
        int i;
        Vector3 pos;
        pos = Vector3.zero;
        pos.x = (x - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
        pos.y = (y - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
        pos.z = map_manager_local.floorZ;

        transform.localPosition = pos;

        for (i = 0; i < maxsizepath; i++)
        {
            finalpath[i] = Vector2.zero;
        }

        finalpath[0].x = x;
        finalpath[0].y = y;

        avataractualposition.x = x;
        avataractualposition.y = y;

        avatarwhereinpath = 0;
        avatarmoving = false;

        Debug.Log("avatar on place - Erased path: " + x + "," + y);
    }

    public void Actualize(float timestep)
    {
        Vector3 _deltavector = Vector3.zero;
        Vector3 _norm = Vector3.zero;
        Vector3 _avatarpos = Vector3.zero;
        Vector3 _wheretogo = Vector3.zero;

        if (avatarmoving) //avatar is moving
        {
            _avatarpos = transform.localPosition;

            _wheretogo.x = (finalpath[avatarwhereinpath + 1].x - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
            _wheretogo.y = (finalpath[avatarwhereinpath + 1].y - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
            _wheretogo.z = map_manager_local.floorZ;

            _deltavector = _wheretogo - transform.localPosition;
            _norm = _deltavector.normalized * avatarspeed * timestep;

            if (_deltavector.magnitude < _norm.magnitude || _deltavector.magnitude == 0)
            {
                _norm = _deltavector;
                avatarmoving = false;
                avatarwhereinpath = avatarwhereinpath + 1;
                avataractualposition = finalpath[avatarwhereinpath];


            }

            _avatarpos = _avatarpos + _norm;
            transform.localPosition = _avatarpos;

        }
        //else //avatar is on place
        if (finalpath[avatarwhereinpath + 1] != Vector2.zero)
        {
            avatarmoving = true;
            avataractualposition = finalpath[avatarwhereinpath];

            //Debug.Log("avatar position on path [" + avatarwhereinpath + "]: " + finalpath[avatarwhereinpath] + " ARRAY:" + finalpath[0] + ", " + finalpath[1] + ", " + finalpath[2] + ", " + finalpath[3] + ", " + finalpath[4]);
        }         
    }

    public void FindPath(int whereX, int whereY)
    {
        int i, j, k;
        Vector2[] oldpath;
        int shortest = 0;

        oldpath = new Vector2[maxsizepath];
        
        

        for (i = 0; i < maxsizepath; i++)
        {
            oldpath[i] = finalpath[i];
            successfulpaths[i] = -1;
            paths[i].valid = 0;
            paths[i].indexinsidepath = 0;
            paths[i].actualX = 0;
            paths[i].actualY = 0;
            for (j = 0; j < maxsizepath; j++)
            {
                paths[i].singlepath[j] = Vector2.zero;


            }
        }
        for (i = 0; i < maxsizepath; i++)
            for (j = 0; j < map_manager_local.mapsize; j++)
                for (k = 0; k < map_manager_local.mapsize; k++)
                {
                    paths[i].roomchecked[j, k] = false;
                }

        paths[0].actualX = (int)avataractualposition.x;
        paths[0].actualY = (int)avataractualposition.y;
        paths[0].roomchecked[paths[0].actualX, paths[0].actualY] = true;
        paths[0].singlepath[0].x = paths[0].actualX;
        paths[0].singlepath[0].y = paths[0].actualY;
        lastusedpath = 0;
        itterations = 0;

        CheckDirection(0, whereX, whereY); //START PATHFINDING RECURSE


        //FIND VALID PATHS
        j = 0;
        for (i = 0; i < maxsizepath; i++)
        {
            if (paths[i].valid == 1)
            {
                successfulpaths[j] = i;
                j = j + 1;
                //Debug.Log("FOUND SUCCESSFUL PATH NO:" + i + ", LENGTH:" + paths[i].indexinsidepath);
            }
        }
        //Debug.Log("AMOUNT OF SUCCESSFUL PATHS:" + j);

        //sort
        shortest = successfulpaths[0];
        if (j > 1)
        {
            for (i = 1; i < j; i++)
            {
                if (paths[successfulpaths[i]].indexinsidepath < paths[shortest].indexinsidepath)
                {
                    shortest = successfulpaths[i];
                }
            }
        }
        //sort

        if (successfulpaths[0] != -1)
        {
            //Debug.Log("Path Length:" + paths[shortest].indexinsidepath);
            //Debug.Log("START[" + avatarwhereinpath + "]:" + avataractualposition);
            //Debug.Log("OLD PATH NOW [" + avatarwhereinpath + "]: " + finalpath[avatarwhereinpath]);
            //Debug.Log("OLD PATH NEXT [" + (avatarwhereinpath + 1) + "]: " + finalpath[avatarwhereinpath + 1]);

            //for (i = 0; i < 7; i++) Debug.Log("ORIGINAL PATH [" + i + "]: " + finalpath[i]);

            //for (i = 0; i < 3; i++) Debug.Log("MOVING TO PATH ID: " + shortest + " [" + i + "]: " + paths[shortest].singlepath[i]);

            //Debug.Log("OLD PATH [" + avatarwhereinpath + "]: " + oldpath[avatarwhereinpath] + " ARRAY:" + oldpath[0] + ", " + oldpath[1] + ", " + oldpath[2] + ", " + oldpath[3] + ", " + oldpath[4]);
            //Debug.Log("NEW PATH ARRAY:" + paths[shortest].singlepath[0] + ", " + paths[shortest].singlepath[1] + ", " + paths[shortest].singlepath[2] + ", " + paths[shortest].singlepath[3] + ", " + paths[shortest].singlepath[4]);

            if (paths[shortest].singlepath[1] != oldpath[avatarwhereinpath+1]) // TESTING!!! BUG!!! BAD ARRAY INDEX
            {
                Debug.Log("DIFFERENT DIRECTION PATH");
                avatarwhereinpath = 0;

                for (i = 0; i < maxsizepath; i++) finalpath[i] = paths[shortest].singlepath[i];

                finalpath[0] = avataractualposition;
                for (i = 0; i <= paths[shortest].indexinsidepath; i++)
                {
                    finalpath[i + 1] = paths[shortest].singlepath[i];
                    
                    //Debug.Log("CLEANING THE PATH ID: " + shortest + " [" + i + "]: " + paths[shortest].singlepath[i]);

                }
            }
            else
            {
                Debug.Log("SAME DIRECTION PATH");
                avatarwhereinpath = 0;
                for (i = 0; i < maxsizepath; i++) finalpath[i] = paths[shortest].singlepath[i];
            }

        }


    }

    static void CheckDirection(int pathindex, int whereX, int whereY)
    {
        int first = 0;
        int ways = 0;
        Vector2 firstdelta = Vector2.zero;
        Vector2 deltatest = Vector2.zero;

        if (paths[pathindex].actualX == whereX && paths[pathindex].actualY == whereY)
        {
            paths[pathindex].valid = 1;
            //Debug.Log("STARTING ON GOAL POSITION - PATH INDEX: " + pathindex);
            paths[pathindex].indexinsidepath = paths[pathindex].indexinsidepath + 1;
            paths[pathindex].singlepath[paths[pathindex].indexinsidepath].x = paths[pathindex].actualX;
            paths[pathindex].singlepath[paths[pathindex].indexinsidepath].y = paths[pathindex].actualY;
        }

        while (paths[pathindex].valid == 0 && pathindex < maxsizepath && itterations < 50)
        {
            itterations = itterations + 1;
            ways = 0;
            if (roomfield[paths[pathindex].actualX + 1, paths[pathindex].actualY] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX + 1, paths[pathindex].actualY]) ways = ways + 1;
            if (roomfield[paths[pathindex].actualX - 1, paths[pathindex].actualY] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX - 1, paths[pathindex].actualY]) ways = ways + 1;
            if (roomfield[paths[pathindex].actualX, paths[pathindex].actualY + 1] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY + 1]) ways = ways + 1;
            if (roomfield[paths[pathindex].actualX, paths[pathindex].actualY - 1] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY - 1]) ways = ways + 1;

            if (ways > 1) //crossing
            {

                first = 0;
                firstdelta = Vector2.zero;

                if (roomfield[paths[pathindex].actualX + 1, paths[pathindex].actualY] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX + 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {
                    deltatest = Crossing(pathindex, whereX, whereY, 1, 0, first);
                    firstdelta = deltatest;
                    first = first + 1;
                }
                if (roomfield[paths[pathindex].actualX - 1, paths[pathindex].actualY] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX - 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {

                    deltatest = Crossing(pathindex, whereX, whereY, -1, 0, first);
                    if (deltatest != Vector2.zero) firstdelta = deltatest;
                    first = first + 1;
                }
                if (roomfield[paths[pathindex].actualX, paths[pathindex].actualY + 1] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY + 1] && paths[pathindex].valid == 0)
                {
                    deltatest = Crossing(pathindex, whereX, whereY, 0, 1, first);
                    if (deltatest != Vector2.zero) firstdelta = deltatest;
                    first = first + 1;
                }
                if (roomfield[paths[pathindex].actualX, paths[pathindex].actualY - 1] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY - 1] && paths[pathindex].valid == 0)
                {
                    deltatest = Crossing(pathindex, whereX, whereY, 0, -1, first);
                    if (deltatest != Vector2.zero) firstdelta = deltatest;
                    first = first + 1;
                }

                paths[pathindex].roomchecked[paths[pathindex].actualX + (int)firstdelta.x, paths[pathindex].actualY + (int)firstdelta.y] = true;
                paths[pathindex].indexinsidepath = paths[pathindex].indexinsidepath + 1;
                paths[pathindex].singlepath[paths[pathindex].indexinsidepath].x = paths[pathindex].actualX + (int)firstdelta.x;
                paths[pathindex].singlepath[paths[pathindex].indexinsidepath].y = paths[pathindex].actualY + (int)firstdelta.y;

                paths[pathindex].actualX = (int)paths[pathindex].actualX + (int)firstdelta.x;
                paths[pathindex].actualY = (int)paths[pathindex].actualY + (int)firstdelta.y;

                //Debug.Log("From Crossing! [" + pathindex + "] XY:" + paths[pathindex].actualX + ", " + paths[pathindex].actualY + ", INDEX[" + paths[pathindex].indexinsidepath + "]");
            }
            else if (ways == 1) //straight way
            {
                first = 0;

                if (roomfield[paths[pathindex].actualX + 1, paths[pathindex].actualY] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX + 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {
                    firstdelta = Crossing(pathindex, whereX, whereY, 1, 0, first);
                }
                else if (roomfield[paths[pathindex].actualX - 1, paths[pathindex].actualY] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX - 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {
                    firstdelta = Crossing(pathindex, whereX, whereY, -1, 0, first);
                }
                else if (roomfield[paths[pathindex].actualX, paths[pathindex].actualY + 1] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY + 1] && paths[pathindex].valid == 0)
                {
                    firstdelta = Crossing(pathindex, whereX, whereY, 0, 1, first);
                }
                else if (roomfield[paths[pathindex].actualX, paths[pathindex].actualY - 1] != 0 && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY - 1] && paths[pathindex].valid == 0)
                {
                    firstdelta = Crossing(pathindex, whereX, whereY, 0, -1, first);
                }

                paths[pathindex].roomchecked[paths[pathindex].actualX + (int)firstdelta.x, paths[pathindex].actualY + (int)firstdelta.y] = true;
                paths[pathindex].indexinsidepath = paths[pathindex].indexinsidepath + 1;
                paths[pathindex].singlepath[paths[pathindex].indexinsidepath].x = paths[pathindex].actualX + (int)firstdelta.x;
                paths[pathindex].singlepath[paths[pathindex].indexinsidepath].y = paths[pathindex].actualY + (int)firstdelta.y;

                paths[pathindex].actualX = (int)paths[pathindex].actualX + (int)firstdelta.x;
                paths[pathindex].actualY = (int)paths[pathindex].actualY + (int)firstdelta.y;

                //Debug.Log("Straight Way! [" + pathindex + "] XY:" + paths[pathindex].actualX + ", " + paths[pathindex].actualY + ", INDEX[" + paths[pathindex].indexinsidepath + "]");
            }
            else //dead end
            {
                //Debug.Log("Dead End! [" + pathindex + "] XY:" + paths[pathindex].actualX + ", " + paths[pathindex].actualY + ", INDEX[" + paths[pathindex].indexinsidepath + "]");
                paths[pathindex].valid = 2;
            }
        }

        if (paths[pathindex].valid == 1)
        {
            //Debug.Log("FOUND Goal! [" + pathindex + "] XY:" + paths[pathindex].actualX + ", " + paths[pathindex].actualY + ", INDEX[" + paths[pathindex].indexinsidepath + "]");
        }
    }

    static Vector2 Crossing(int pathindex, int whereX, int whereY, int deltaX, int deltaY, int first)
    {
        Vector2 back = Vector2.zero;
        int i;

        if (first > 0)
        {
            lastusedpath = lastusedpath + 1;
            for (i = 0; i < paths[pathindex].indexinsidepath + 1; i++)
            {
                paths[lastusedpath].singlepath[i] = paths[pathindex].singlepath[i];
                paths[lastusedpath].roomchecked[(int) paths[pathindex].singlepath[i].x, (int) paths[pathindex].singlepath[i].y] = true;
            }

            paths[lastusedpath].indexinsidepath = paths[pathindex].indexinsidepath + 1;
            paths[lastusedpath].actualX = paths[pathindex].actualX;
            paths[lastusedpath].actualY = paths[pathindex].actualY;

            pathindex = lastusedpath;

            paths[pathindex].actualX = paths[pathindex].actualX + deltaX;
            paths[pathindex].actualY = paths[pathindex].actualY + deltaY;
            paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY] = true;

            paths[pathindex].singlepath[paths[pathindex].indexinsidepath].x = paths[pathindex].actualX;
            paths[pathindex].singlepath[paths[pathindex].indexinsidepath].y = paths[pathindex].actualY;

            if (paths[pathindex].actualX == whereX && paths[pathindex].actualY == whereY)
            {
                paths[pathindex].valid = 1;
            }
            else
            {
                //Debug.Log("New Way! [" + pathindex + "] XY:" + paths[pathindex].actualX + ", " + paths[pathindex].actualY + ", INDEX[" + paths[pathindex].indexinsidepath + "]");
                CheckDirection(pathindex, whereX, whereY);
            }
            return Vector2.zero; //!!!!!!!

        }
        else
        {

            //Debug.Log("Index:" + paths[pathindex].indexinsidepath);
            if (paths[pathindex].actualX + deltaX == whereX && paths[pathindex].actualY + deltaY == whereY)
            {
                paths[pathindex].valid = 1;
            }

            back.x = deltaX;
            back.y = deltaY;
            return back;

        }
        //Debug.Log("next check:" + paths[pathindex].actualX + ", " + paths[pathindex].actualY + ", itterations: " + itterations + " Goal:" + whereX + ", " + whereY);
    }

}