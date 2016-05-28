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
    public const int maxsizepath = 400;
	public GameObject avatarobject;
	public GameObject avatarcamera;
	public GameObject minimapobject;
	public GameObject camerafolowobject;
	public GameObject AvatarSkin;
	public Material AvatarMaterialLow;
	public Material AvatarMaterialHi;
	float avatarshift = 15;

    static float avatarspeed = 200;
    static float avatarrotationspeed = 8;

    [HideInInspector] public Vector2 avataractualposition;

    static Vector3 avatar_old_worldposition;
    [HideInInspector] public Vector3 avatar_actual_worldposition;
    static Vector2[] finalpath;
    static int avatarwhereinpath = 0;
    [HideInInspector] public bool avatarmoving;
	[HideInInspector] public bool avatardetail = false;

    static map_manager map_manager_local;
	static camera_lowfps camera_lowfps_local;

    public struct fogroomstruct
    {
        public GameObject fog;
        //public GameObject subfog;
		public int visitedstate;
		public bool enabledpath;
    }
    static fogroomstruct[,] fogfield;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		
    }

	public void SetHiMaterial (bool hi)
	{
		if (hi)
		{
			AvatarSkin.GetComponent<Renderer> ().sharedMaterial = AvatarMaterialHi;
			Debug.Log("Hi Material");
		} else
		{
			AvatarSkin.GetComponent<Renderer> ().sharedMaterial = AvatarMaterialLow;
			Debug.Log("Low Material");
		}
	}

    public void RotateAvatarByPath()
    {
        Vector3 delta;
        float oldangle;
        float finalangle = 0;

        delta = avatar_actual_worldposition - avatar_old_worldposition;
        delta = delta.normalized;

        //Debug.Log("Delta: " + delta);       

        oldangle = transform.localRotation.eulerAngles.z;

        if (delta.x == 0 && delta.y == 1) finalangle = 90;
        else if (delta.x == 0 && delta.y == -1) finalangle = 270;
        else if (delta.x == -1 && delta.y == 0) finalangle = 180;
        else if (delta.x == 1 && delta.y == 0) finalangle = 0;
        else finalangle = oldangle;

        if (Mathf.Round(oldangle) != Mathf.Round(finalangle))
        {
            if (Mathf.Abs(finalangle - oldangle) <= 180)
            {
                finalangle = (finalangle - oldangle) * Time.deltaTime * avatarrotationspeed;
            }
            else
            {
                if ((finalangle - oldangle) > 0)
                {
                    //Debug.Log("PLUS 180");
                    finalangle = -(finalangle + 360 - oldangle) * Time.deltaTime * avatarrotationspeed;
                    
                }
                else
                {
                    finalangle = (finalangle + 360 - oldangle) * Time.deltaTime * avatarrotationspeed;
                    //Debug.Log("MINUS 180");
                }

            }
        }
        else finalangle = 0;
        this.transform.Rotate(0, 0, finalangle);

    }
        

    public void PathFindingInit()
    {
        int i, j, k;
        map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
		camera_lowfps_local = GameObject.FindObjectOfType(typeof(camera_lowfps)) as camera_lowfps;
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
				roomfield[i, j] = 0;
			}

        for (i = 0; i < map_manager_local.mapsize; i++)
            for (j = 0; j < map_manager_local.mapsize; j++)
            {
                if (originalmap[i, j] != null)
                {
					if (!map_manager_local.mapfield [i, j].GetComponent<map_piece_def> ().isroom)
					{
						roomfield [i, j] = 3;
					}
					else // ROOM FOUND
					{
						//Debug.Log("Found Room_piece: " + originalmap[i,j].name);
						roomfield [i, j] = 4;
						roomfield [i-1, j] = 2;
						roomfield [i+1, j] = 2;
						roomfield [i, j-1] = 2;
						roomfield [i-1, j-1] = 1;
						roomfield [i+1, j-1] = 1;
						roomfield [i, j+1] = 2;
						roomfield [i-1, j+1] = 1;
						roomfield [i+1, j+1] = 1;
					}
                    //Debug.Log("Found map_piece: " + originalmap[i,j].name);
                }
            }
    }

	void FogUpdateCross(int x, int y, float intime, bool cross)
    {
        fogfield[x, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime/200);

		if (map_manager_local.mapfield [x, y] != null)
		{
			fogfield [x, y].enabledpath = true;
			if (fogfield [x, y].visitedstate == 0)
			{
				fogfield [x, y].visitedstate = 1;
				minimapobject.GetComponent<minimap>().MapUpdate (x, y, 1);
			}
			map_manager_local.mapfield [x, y].GetComponent<UnityEngine.UI.Button> ().interactable = true;
		}
        
		if (cross)
        {
            fogfield[x + 1, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime/100);
            fogfield[x - 1, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime/100);
            fogfield[x, y - 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime/100);
            fogfield[x, y + 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime/100);

			/*if (fogfield [x+1, y].visitedstate == 0) fogfield [x+1, y].visitedstate = 1;

			if (fogfield [x-1, y].visitedstate == 0) fogfield [x-1, y].visitedstate = 1;

			if (fogfield [x, y+1].visitedstate == 0) fogfield [x, y+1].visitedstate = 1;

			if (fogfield [x, y-1].visitedstate == 0) fogfield [x, y-1].visitedstate = 1;
*/
			if (roomfield[x+1,y]>1) fogfield [x+1, y].enabledpath = true;
			if (roomfield[x-1,y]>1) fogfield [x-1, y].enabledpath = true;
			if (roomfield[x,y+1]>1) fogfield [x, y+1].enabledpath = true;
			if (roomfield[x,y-1]>1) fogfield [x, y-1].enabledpath = true;

            if (map_manager_local.mapfield[x+1, y] != null)
            {
				if (fogfield [x+1, y].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate (x+1, y, 1);
				map_manager_local.mapfield[x+1, y].GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            if (map_manager_local.mapfield[x-1, y] != null)
            {
				if (fogfield [x-1, y].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate (x-1, y, 1);
                map_manager_local.mapfield[x-1, y].GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            if (map_manager_local.mapfield[x, y+1] != null)
            {
				if (fogfield [x, y+1].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate (x, y+1, 1);
                map_manager_local.mapfield[x, y+1].GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            if (map_manager_local.mapfield[x, y-1] != null)
            {
				if (fogfield [x, y-1].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate (x, y-1, 1);
                map_manager_local.mapfield[x, y-1].GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }
    }

    public void FogUpdate()
    {
        int x, y;
        x = (int)avataractualposition.x;
        y = (int)avataractualposition.y;

		minimapobject.GetComponent<minimap>().MapUpdate (x, y, 2);
		minimapobject.GetComponent<minimap>().MapUpdate (x, y, 3);
		fogfield [x, y].visitedstate = 2;
		fogfield [x, y].enabledpath = true;

		FogUpdateCross(x, y, 0.01f, true);

        if (roomfield[x + 1,y] > 0)
        {
			FogUpdateCross(x + 1, y, 1, true);

            if (roomfield[x + 2, y] > 0)
            {
				FogUpdateCross(x + 2, y, 2, true);

                if (roomfield[x + 3, y] > 0)
                {
					FogUpdateCross(x + 3, y, 3, false);


                }
            }

        }
        if (roomfield[x - 1, y] > 0)
        {
			FogUpdateCross(x - 1, y, 1, true);

            if (roomfield[x - 2, y] > 0)
            {
				FogUpdateCross(x - 2, y, 2, true);

                if (roomfield[x - 3, y] > 0)
                {
					FogUpdateCross(x - 3, y, 3, false);


                }
            }

        }
        if (roomfield[x, y + 1] > 0)
        {
			FogUpdateCross(x, y + 1, 1, true);

            if (roomfield[x, y + 2] > 0)
            {
				FogUpdateCross(x, y + 2, 2, true);

                if (roomfield[x, y + 3] > 0)
                {
					FogUpdateCross(x, y + 3, 3, false);


                }
            }

        }
        if (roomfield[x, y - 1] > 0)
        {
			FogUpdateCross(x, y - 1, 1, true);

            if (roomfield[x, y - 2] > 0)
            {
				FogUpdateCross(x, y - 2, 2, true);

                if (roomfield[x, y - 3] > 0)
                {
					FogUpdateCross(x, y - 3, 3, false);


                }
            }

        }

    }

    public void FogInit()
    {
        int i, j;
        Vector3 pos;
        bool found;

        fogfield = new fogroomstruct[map_manager_local.mapsize, map_manager_local.mapsize];
        for (i = 1; i < map_manager_local.mapsize-1; i++)
            for (j = 1; j < map_manager_local.mapsize-1; j++)
            {
                found = false;
				if (roomfield[i, j] != 0)
                {
					if (roomfield[i,j] > 2) map_manager_local.mapfield[i, j].GetComponent<UnityEngine.UI.Button>().interactable = false;
                    found = true;
                }
				if (roomfield[i+1, j] != 0) found = true;
				if (roomfield[i-1, j] != 0) found = true;
				if (roomfield[i, j+1] != 0) found = true;
				if (roomfield[i, j-1] != 0) found = true;

                if (found)
                {
                    fogfield[i, j].fog = Instantiate(map_manager_local.fogroomprefab, Vector3.zero, Quaternion.identity) as GameObject;
                    fogfield[i, j].fog.transform.SetParent(map_manager_local.fogcontainer.transform);
                    fogfield[i, j].fog.transform.localRotation = Quaternion.identity;
                    pos = Vector3.zero;
                    pos.x = (i - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
                    pos.y = (j - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
                    fogfield[i, j].fog.transform.localPosition = pos;
					fogfield[i, j].visitedstate = 0;
					fogfield [i, j].enabledpath = false;
                }
                
            }
    }

    public void SetCharacter(float x, float y)
    {
        int i;
        Vector3 pos;
        pos = Vector3.zero;
		pos.x = (x - map_manager_local.mapoffset) * map_manager_local.mappiecesize + avatarshift;
		pos.y = (y - map_manager_local.mapoffset) * map_manager_local.mappiecesize/* - avatarshift*/;
        pos.z = map_manager_local.floorZ;

        transform.localPosition = pos;
        avatar_old_worldposition = pos;
        avatar_actual_worldposition = pos;

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
		camera_lowfps_local.fpstime = 100;
        map_manager_local.avatarstatictime = 0;
		SetHiMaterial (false);

        
        //Debug.Log("avatar on place - Erased path: " + x + "," + y);
    }

	public void AvatarStop()
	{
		int i;

		for (i = 0; i < maxsizepath; i++)
		{
			
			if (i != avatarwhereinpath + 1)
				finalpath [i] = Vector2.zero;
		}
		finalpath [avatarwhereinpath + 2] = finalpath [avatarwhereinpath + 1];

	}

    public void Actualize(float timestep)
    {
        Vector3 _deltavector = Vector3.zero;
        Vector3 _norm = Vector3.zero;
        Vector3 _avatarpos = Vector3.zero;
        Vector3 _wheretogo = Vector3.zero;

        if (avatarmoving) //avatar is moving
        {
            avatar_old_worldposition = avatar_actual_worldposition;
            _avatarpos = avatar_old_worldposition;

			_wheretogo.x = (finalpath[avatarwhereinpath + 1].x - map_manager_local.mapoffset) * map_manager_local.mappiecesize + avatarshift;
			_wheretogo.y = (finalpath[avatarwhereinpath + 1].y - map_manager_local.mapoffset) * map_manager_local.mappiecesize/* - avatarshift*/;
            _wheretogo.z = map_manager_local.floorZ;

            _deltavector = _wheretogo - transform.localPosition;
            _norm = _deltavector.normalized * avatarspeed * timestep;

			if (_deltavector.magnitude < _norm.magnitude || _deltavector.magnitude == 0)
			{
				_norm = _deltavector;
				avatarmoving = false;
				avatarwhereinpath = avatarwhereinpath + 1;
				avataractualposition = finalpath [avatarwhereinpath];
				avatar_old_worldposition = avatar_actual_worldposition;

				FogUpdate ();

				if (finalpath [avatarwhereinpath + 1] == Vector2.zero)
				{
					map_manager_local.avatarstatictime = 0.4f;
					map_manager_local.camerafadeouttime = 0.15f;
					map_manager_local.cameraspeed = map_manager_local.cameraspeedhi;

					avatarobject.GetComponent<Animator> ().SetTrigger ("idle");
					camera_lowfps_local.fpstime = 100;

					map_manager_local.mapfield [(int)finalpath [avatarwhereinpath].x, (int)finalpath [avatarwhereinpath].y].GetComponent<map_piece_def> ().SetActiveElement (1);
				}
					

			}
            _avatarpos = _avatarpos + _norm;
            avatar_actual_worldposition = _avatarpos;
            transform.localPosition = _avatarpos;

			if (avatarmoving)
			{
				RotateAvatarByPath ();
			}
        }
        
        if (finalpath[avatarwhereinpath + 1] != Vector2.zero)
        {
            avatarmoving = true;
            avataractualposition = finalpath[avatarwhereinpath];

            //Debug.Log("avatar position on path [" + avatarwhereinpath + "]: " + finalpath[avatarwhereinpath] + " ARRAY:" + finalpath[0] + ", " + finalpath[1] + ", " + finalpath[2] + ", " + finalpath[3] + ", " + finalpath[4]);
        }
    }

	void DeactivateActiveElements()
	{
		int i, j;
		for (i = 0; i < map_manager_local.mapsize; i++)
			for (j = 0; j < map_manager_local.mapsize; j++)
			{
				if (map_manager_local.mapfield [i, j] != null) map_manager_local.mapfield [i, j].GetComponent<map_piece_def> ().SetActiveElement(0);
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
                //Debug.Log("DIFFERENT DIRECTION PATH");
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
                //Debug.Log("SAME DIRECTION PATH");
                avatarwhereinpath = 0;
                for (i = 0; i < maxsizepath; i++) finalpath[i] = paths[shortest].singlepath[i];
            }

			DeactivateActiveElements ();
			avatarobject.GetComponent<Animator> ().SetTrigger ("run");

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

		while (paths[pathindex].valid == 0 && pathindex < maxsizepath && itterations < maxsizepath)
        {
            itterations = itterations + 1;
            ways = 0;
			if (fogfield[paths[pathindex].actualX + 1, paths[pathindex].actualY].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX + 1, paths[pathindex].actualY]) ways = ways + 1;
			if (fogfield[paths[pathindex].actualX - 1, paths[pathindex].actualY].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX - 1, paths[pathindex].actualY]) ways = ways + 1;
			if (fogfield[paths[pathindex].actualX, paths[pathindex].actualY + 1].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY + 1]) ways = ways + 1;
			if (fogfield[paths[pathindex].actualX, paths[pathindex].actualY - 1].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY - 1]) ways = ways + 1;

            if (ways > 1) //crossing
            {

                first = 0;
                firstdelta = Vector2.zero;

				if (fogfield[paths[pathindex].actualX + 1, paths[pathindex].actualY].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX + 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {
                    deltatest = Crossing(pathindex, whereX, whereY, 1, 0, first);
                    firstdelta = deltatest;
                    first = first + 1;
                }
				if (fogfield[paths[pathindex].actualX - 1, paths[pathindex].actualY].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX - 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {

                    deltatest = Crossing(pathindex, whereX, whereY, -1, 0, first);
                    if (deltatest != Vector2.zero) firstdelta = deltatest;
                    first = first + 1;
                }
				if (fogfield[paths[pathindex].actualX, paths[pathindex].actualY + 1].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY + 1] && paths[pathindex].valid == 0)
                {
                    deltatest = Crossing(pathindex, whereX, whereY, 0, 1, first);
                    if (deltatest != Vector2.zero) firstdelta = deltatest;
                    first = first + 1;
                }
				if (fogfield[paths[pathindex].actualX, paths[pathindex].actualY - 1].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY - 1] && paths[pathindex].valid == 0)
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

				if (fogfield[paths[pathindex].actualX + 1, paths[pathindex].actualY].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX + 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {
                    firstdelta = Crossing(pathindex, whereX, whereY, 1, 0, first);
                }
				else if (fogfield[paths[pathindex].actualX - 1, paths[pathindex].actualY].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX - 1, paths[pathindex].actualY] && paths[pathindex].valid == 0)
                {
                    firstdelta = Crossing(pathindex, whereX, whereY, -1, 0, first);
                }
				else if (fogfield[paths[pathindex].actualX, paths[pathindex].actualY + 1].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY + 1] && paths[pathindex].valid == 0)
                {
                    firstdelta = Crossing(pathindex, whereX, whereY, 0, 1, first);
                }
				else if (fogfield[paths[pathindex].actualX, paths[pathindex].actualY - 1].enabledpath && !paths[pathindex].roomchecked[paths[pathindex].actualX, paths[pathindex].actualY - 1] && paths[pathindex].valid == 0)
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