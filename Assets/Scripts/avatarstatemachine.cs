using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class avatarstatemachine : MonoBehaviour
{
    /*public struct path
    {
        public Vector2[] singlepath;
        public int valid;
        public int indexinsidepath;
        public int actualX;
        public int actualY;
        public bool[,] roomchecked;
    }*/
    //static int itterations = 0;
    static int lastusedpath;
    private IEnumerator setpathfindingcoroutine;
    //static int[,] roomfield;
    public const int maxsizepath = 5000;
	public GameObject avatarobject;
	public GameObject avatarrotationobject;
	public GameObject avatarcamera;
	public GameObject minimapobject;
	public GameObject camerafolowobject;
	public GameObject AvatarSkin;
	public Material AvatarMaterialLow;
	public Material AvatarMaterialHi;
    [HideInInspector] public float avatarshift = 20;

    static float avatarspeed = 150;
    static float avatarrotationspeed = 12;
	static float curveoffset = 20;

    [HideInInspector] public Vector2 avataractualposition;
    [HideInInspector] public Vector2 avatarafterstaticposition;

    static Vector3 avatar_old_worldposition;
	static float avatar_old_rotationx = 0;
    [HideInInspector] public Vector3 avatar_actual_worldposition;
    [HideInInspector] public Vector2[] finalpath;
    [HideInInspector] public int avatarwhereinpath = 0;
    [HideInInspector] public bool avatarmoving;
	[HideInInspector] public bool avatardetail = false;
    [HideInInspector] public bool battlefoundinfog = false;

    static map_manager map_manager_local;
    static character_definitions character_definitions_local;
    static camera_lowfps camera_lowfps_local;

    public struct fogroomstruct
    {
        public GameObject fog;
        //public GameObject subfog;
		public int visitedstate;
		public bool enabledpath;
    }
    static fogroomstruct[,] fogfield;

    //-----pathfinding------------
    public class Node
    {
        public List<Node> neighbours;
        public int x;
        public int y;

        public Node()
        {
            neighbours = new List<Node>();
        }

        public float DistanceTo(Node n)
        {
            if (n == null)
            {
                Debug.LogError("WTF?");
            }

            return Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(n.x, n.y)
                );
        }
    }
    Node[,] graph;

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
			//Debug.Log("Hi Material");
		} else
		{
			AvatarSkin.GetComponent<Renderer> ().sharedMaterial = AvatarMaterialLow;
			//Debug.Log("Low Material");
		}
	}

	public void RotateAvatarToWaypoint(Vector3 _wheretogo, float strength)
    {
       	Vector3 wheretogodirection;
		//Vector3 delta;
		Vector3 finalrotation;
		Vector3 cross;
		//float oldangle;
        float finalangle = 0;
		float angleincrement;
	

		wheretogodirection = _wheretogo - avatar_actual_worldposition;

		finalangle = Vector3.Angle(Vector3.right,wheretogodirection);

		cross = Vector3.Cross(Vector3.right,wheretogodirection);
		if (cross.z > 0)
		{
			finalangle = 360 - finalangle;
		}

		//finalrotation = avatarrotationobject.transform.localEulerAngles;

		angleincrement = finalangle - avatar_old_rotationx;
		if (angleincrement >180) angleincrement = -(360 - angleincrement);
		else if (angleincrement <-180) angleincrement = 360 + angleincrement;

		if (strength != -1)
		{
			finalrotation.x = avatar_old_rotationx + angleincrement * Time.deltaTime * avatarrotationspeed * strength;
		}
		else
		{
			finalrotation.x = avatar_old_rotationx + angleincrement;
		}

		//finalrotation.x = finalangle;
		finalrotation.y = 90;
		finalrotation.z = 270;

		//Debug.Log("OriginalRotation: " + avatar_old_rotationx + ", FinalRotation:" + finalangle + " Increment: " + angleincrement);

		avatarrotationobject.transform.localEulerAngles=finalrotation;

		avatar_old_rotationx = finalrotation.x;
		if (avatar_old_rotationx < 0)
			avatar_old_rotationx = avatar_old_rotationx + 360;
		if (avatar_old_rotationx > 360)
			avatar_old_rotationx = avatar_old_rotationx - 360;

    }
        

    public void PathFindingInit()
    {
        int i, j, k;

        map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
        character_definitions_local = GameObject.FindObjectOfType(typeof(character_definitions)) as character_definitions;
        camera_lowfps_local = GameObject.FindObjectOfType(typeof(camera_lowfps)) as camera_lowfps;
        GeneratePathfindingGraph();


        finalpath = new Vector2[maxsizepath];
        /*successfulpaths = new int[maxsizepath];

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
        }*/
    }

  
    public Vector2 ClosestRoom(Vector2 mousepos)
    {
        int i;
        int j;
        float minDistance;
        float testedDistance;
        Vector2 vectorDelta;
        Vector2 foundRoom;
        Vector2 testedRoom;
        foundRoom = Vector2.zero;
        minDistance = 5;
        mousepos.x = mousepos.x + 0.10f;
        for (i = 0; i < map_manager_local.mapsize; i++)
            for (j = 0; j < map_manager_local.mapsize; j++)
            {
                if (fogfield[i, j].enabledpath)
                {
                    testedRoom.x = i;
                    testedRoom.y = j;
                    vectorDelta = mousepos - testedRoom;
                    testedDistance = vectorDelta.magnitude;
                    if (testedDistance<minDistance && testedDistance<2)
                    {
                        foundRoom = testedRoom;
                        minDistance = testedDistance;
                    }
                }
            }
        //Debug.Log("ClosestRoom: " + foundRoom);
        return foundRoom;
        
    }

    bool isVisitedRoomsAround(int x, int y)
    {
        if ((map_manager_local.dungeonmap.maptiles[x, y + 1].isroom) && (fogfield[x, y + 1].visitedstate == 0)) return true;
        if ((map_manager_local.dungeonmap.maptiles[x, y -1].isroom) && (fogfield[x, y - 1].visitedstate == 0)) return true;
        if ((map_manager_local.dungeonmap.maptiles[x + 1, y].isroom) && (fogfield[x + 1, y].visitedstate == 0)) return true;
        if ((map_manager_local.dungeonmap.maptiles[x - 1, y].isroom) && (fogfield[x - 1, y].visitedstate == 0)) return true;

        return false;
    }

	void FogUpdateCross(int x, int y, float intime, bool cross)
    {
        if (character_definitions_local.CheckBattle(x, y) >= 0)
        {
            battlefoundinfog = true;
        }
        else if (battlefoundinfog == false)
        {

            fogfield[x, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 200);

            if (map_manager_local.dungeonmap.maptiles[x, y].isroom)
            {
                fogfield[x, y].enabledpath = true;
                if (fogfield[x, y].visitedstate == 0)
                {
                    fogfield[x, y].visitedstate = 1;
                    minimapobject.GetComponent<minimap>().MapUpdate(x, y, 1);
                }
                //map_manager_local.mapfield[x, y].GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            
            if (cross)
            {
                if (character_definitions_local.CheckBattle(x + 1, y) >= 0 || character_definitions_local.CheckBattle(x - 1, y) >= 0 || character_definitions_local.CheckBattle(x, y - 1) >= 0 || character_definitions_local.CheckBattle(x, y + 1) >= 0)
                {
                    battlefoundinfog = true;
                }

                if ((map_manager_local.dungeonmap.maptiles[x + 2, y].isroom) && (fogfield[x + 2, y].visitedstate != 0) || !isVisitedRoomsAround(x + 2, y) || (map_manager_local.dungeonmap.maptiles[x + 1, y].isroom))
                {
                    fogfield[x + 1, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 100);
                    if (map_manager_local.dungeonmap.maptiles[x + 1, y].isroom) fogfield[x + 1, y].enabledpath = true;
                }

                if ((map_manager_local.dungeonmap.maptiles[x - 2, y].isroom) && (fogfield[x - 2, y].visitedstate != 0) || !isVisitedRoomsAround(x - 2, y) || (map_manager_local.dungeonmap.maptiles[x - 1, y].isroom))
                {
                    fogfield[x - 1, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 100);
                    if (map_manager_local.dungeonmap.maptiles[x - 1, y].isroom) fogfield[x - 1, y].enabledpath = true;
                }

                if ((map_manager_local.dungeonmap.maptiles[x, y - 2].isroom) && (fogfield[x, y - 2].visitedstate != 0) || !isVisitedRoomsAround(x, y - 2) || (map_manager_local.dungeonmap.maptiles[x, y - 1].isroom))
                {
                    fogfield[x, y - 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 100);
                    if (map_manager_local.dungeonmap.maptiles[x, y - 1].isroom) fogfield[x, y - 1].enabledpath = true;
                }

                if ((map_manager_local.dungeonmap.maptiles[x, y + 2].isroom) && (fogfield[x, y + 2].visitedstate != 0) || !isVisitedRoomsAround(x, y + 2) || (map_manager_local.dungeonmap.maptiles[x, y + 1].isroom))
                {
                    fogfield[x, y + 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 100);
                    if (map_manager_local.dungeonmap.maptiles[x, y + 1].isroom) fogfield[x, y + 1].enabledpath = true;
                }

                //empty space fog
                
                /*if (!isVisitedRoomsAround(x + 2, y))
                {
                    fogfield[x + 2, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 25);
                }
                if (!isVisitedRoomsAround(x - 2, y))
                {
                    fogfield[x - 2, y].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 25);
                }
                if (!isVisitedRoomsAround(x, y + 2))
                {
                    fogfield[x, y + 2].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 25);
                }
                if (!isVisitedRoomsAround(x, y - 2))
                {
                    fogfield[x, y - 2].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 25);
                }
                
                
                if (!isVisitedRoomsAround(x - 1, y + 1))
                {
                    fogfield[x - 1, y + 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 50);
                }
                if (!isVisitedRoomsAround(x + 1, y + 1))
                {
                    fogfield[x + 1, y + 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 50);
                }
                if (!isVisitedRoomsAround(x + 1, y - 1))
                {
                    fogfield[x + 1, y - 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 50);
                }
                if (!isVisitedRoomsAround(x - 1, y - 1))
                {
                    fogfield[x - 1, y - 1].fog.GetComponent<fog_controller>().SetRoomAlpha(intime / 50);
                }
                */


                //enable minimap
                if (map_manager_local.dungeonmap.maptiles[x + 1, y].isroom)
                {                    
                        if (fogfield[x + 1, y].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate(x + 1, y, 1);
                }
                if (map_manager_local.dungeonmap.maptiles[x - 1, y].isroom)
                {                    
                        if (fogfield[x - 1, y].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate(x - 1, y, 1);
                }
                if (map_manager_local.dungeonmap.maptiles[x, y + 1].isroom)
                {                   
                        if (fogfield[x, y + 1].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate(x, y + 1, 1);
                }
                if (map_manager_local.dungeonmap.maptiles[x, y - 1].isroom)
                {                   
                        if (fogfield[x, y - 1].visitedstate == 0) minimapobject.GetComponent<minimap>().MapUpdate(x, y - 1, 1);                    
                }
            }
        }
    }

    public void FogUpdate()
    {
        int x, y;
        x = (int)avataractualposition.x;
        y = (int)avataractualposition.y;

        minimapobject.GetComponent<minimap>().MapUpdate(x, y, 2);
        minimapobject.GetComponent<minimap>().MapUpdate(x, y, 3);
        fogfield[x, y].visitedstate = 2;
        fogfield[x, y].enabledpath = true;


        battlefoundinfog = false;

        FogUpdateCross(x, y, 0.01f, true);

        if (character_definitions_local.CheckBattle(x, y) < 0) battlefoundinfog = false;
        if (!battlefoundinfog)
        {
            if (map_manager_local.dungeonmap.maptiles[x + 1, y].isroom)
            {
                FogUpdateCross(x + 1, y, 1, true);

                if (map_manager_local.dungeonmap.maptiles[x + 2, y].isroom)
                {
                    FogUpdateCross(x + 2, y, 2, true);

                    if (map_manager_local.dungeonmap.maptiles[x + 3, y].isroom)
                    {
                        FogUpdateCross(x + 3, y, 3, false);


                    }
                }

            }
            battlefoundinfog = false;

            if (map_manager_local.dungeonmap.maptiles[x - 1, y].isroom)
            {
                FogUpdateCross(x - 1, y, 1, true);

                if (map_manager_local.dungeonmap.maptiles[x - 2, y].isroom)
                {
                    FogUpdateCross(x - 2, y, 2, true);

                    if (map_manager_local.dungeonmap.maptiles[x - 3, y].isroom)
                    {
                        FogUpdateCross(x - 3, y, 3, false);


                    }
                }

            }
            battlefoundinfog = false;
            if (map_manager_local.dungeonmap.maptiles[x, y + 1].isroom)
            {
                FogUpdateCross(x, y + 1, 1, true);

                if (map_manager_local.dungeonmap.maptiles[x, y + 2].isroom)
                {
                    FogUpdateCross(x, y + 2, 2, true);

                    if (map_manager_local.dungeonmap.maptiles[x, y + 3].isroom)
                    {
                        FogUpdateCross(x, y + 3, 3, false);


                    }
                }

            }
            battlefoundinfog = false;
            if (map_manager_local.dungeonmap.maptiles[x, y - 1].isroom)
            {
                FogUpdateCross(x, y - 1, 1, true);

                if (map_manager_local.dungeonmap.maptiles[x, y - 2].isroom)
                {
                    FogUpdateCross(x, y - 2, 2, true);

                    if (map_manager_local.dungeonmap.maptiles[x, y - 3].isroom)
                    {
                        FogUpdateCross(x, y - 3, 3, false);


                    }
                }
            }
        }
    }

    public void FogInit(bool firsttime)
    {
        int i, j;
        Vector3 pos;
        bool found;

        if (firsttime) fogfield = new fogroomstruct[map_manager_local.mapsize, map_manager_local.mapsize];

        for (i = 1; i < map_manager_local.mapsize-1; i++)
            for (j = 1; j < map_manager_local.mapsize-1; j++)
            {
                found = false;
				if (map_manager_local.dungeonmap.maptiles[i, j].isroom)
                {
					//if (roomfield[i,j] > 2) map_manager_local.mapfield[i, j].GetComponent<UnityEngine.UI.Button>().interactable = false;
                    found = true;
                }
				if (map_manager_local.dungeonmap.maptiles[i + 1, j].isroom) found = true;
				if (map_manager_local.dungeonmap.maptiles[i - 1, j].isroom) found = true;
				if (map_manager_local.dungeonmap.maptiles[i, j + 1].isroom) found = true;
				if (map_manager_local.dungeonmap.maptiles[i, j - 1].isroom) found = true;

                if (map_manager_local.dungeonmap.maptiles[i + 1, j + 1].isroom) found = true;
                if (map_manager_local.dungeonmap.maptiles[i - 1, j + 1].isroom) found = true;
                if (map_manager_local.dungeonmap.maptiles[i + 1, j - 1].isroom) found = true;
                if (map_manager_local.dungeonmap.maptiles[i - 1, j - 1].isroom) found = true;

                if (fogfield[i,j].fog!=null)
                {
                    Destroy(fogfield[i, j].fog);
                }
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

                    
                    //-------------Test--------------------------
                    /*fogfield[i, j].enabledpath = true;
                    if (fogfield[i, j].visitedstate == 0)
                    {
                        fogfield[i, j].visitedstate = 1;
                        minimapobject.GetComponent<minimap>().MapUpdate(i, j, 1);
                    }*/
                    //-------------------------------------------
                    
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

	public void AvatarStop()// ADD ACTIVE ELEMENT!!!!!!!
    {
		int i;
		if (finalpath [avatarwhereinpath] != Vector2.zero)
		{
			avatarafterstaticposition = finalpath [avatarwhereinpath + 1];
            //map_manager_local.mapfield [(int)finalpath [avatarwhereinpath].x, (int)finalpath [avatarwhereinpath].y].GetComponent<map_piece_def> ().SetActiveElement (1);//SET ACTIVE ELEMENT!!!!!!!
        }
        if (finalpath [avatarwhereinpath+1] != Vector2.zero)
		{
			//map_manager_local.mapfield [(int)finalpath [avatarwhereinpath+1].x, (int)finalpath [avatarwhereinpath+1].y].GetComponent<map_piece_def> ().SetActiveElement (1); //SET ACTIVE ELEMENT!!!!!!!
		}


		for (i = 0; i < maxsizepath; i++)
		{
			finalpath [i] = Vector2.zero;
		}


		avatarmoving = false;
		avatarobject.GetComponent<Animator> ().SetTrigger ("idle");
		camera_lowfps_local.fpstime = 100;
	}

    public void Actualize(float timestep)
    {
        Vector3 _deltavector = Vector3.zero;
        Vector3 _norm = Vector3.zero;
        Vector3 _avatarpos = Vector3.zero;
        Vector3 _wheretogo = Vector3.zero;
		Vector3 _wheretogo0 = Vector3.zero;
		Vector3 _wheretogo2 = Vector3.zero;
		Vector2 path1;
		Vector2 path2;
		float waypointdistance1;
		float waypointdistance2;
        int battleindex;

        if (avatarmoving) //avatar is moving
        {
            avatar_old_worldposition = avatar_actual_worldposition;
            _avatarpos = avatar_old_worldposition;

			_wheretogo.x = (finalpath[avatarwhereinpath + 1].x - map_manager_local.mapoffset) * map_manager_local.mappiecesize + avatarshift;
			_wheretogo.y = (finalpath[avatarwhereinpath + 1].y - map_manager_local.mapoffset) * map_manager_local.mappiecesize/* - avatarshift*/;
            _wheretogo.z = map_manager_local.floorZ;

			path1 = finalpath [avatarwhereinpath + 1] - finalpath [avatarwhereinpath];
            
			if (finalpath [avatarwhereinpath + 2] != Vector2.zero)
            {
				path2 = finalpath [avatarwhereinpath + 2] - finalpath [avatarwhereinpath + 1];

				_wheretogo0.x = (finalpath [avatarwhereinpath].x - map_manager_local.mapoffset) * map_manager_local.mappiecesize + avatarshift;
				_wheretogo0.y = (finalpath [avatarwhereinpath].y - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
				_wheretogo0.z = map_manager_local.floorZ;

				_wheretogo2.x = (finalpath [avatarwhereinpath + 2].x - map_manager_local.mapoffset) * map_manager_local.mappiecesize + avatarshift;
				_wheretogo2.y = (finalpath [avatarwhereinpath + 2].y - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
				_wheretogo2.z = map_manager_local.floorZ;

				_deltavector = transform.localPosition - _wheretogo0;
				waypointdistance1 = _deltavector.magnitude;
				_deltavector = _wheretogo2 - transform.localPosition;
				waypointdistance2 = _deltavector.magnitude;

				if (path1 != path2 && waypointdistance1 > map_manager_local.mappiecesize * 0.3f) {
					_wheretogo.x = _wheretogo.x - path1.x * curveoffset * 1.5f + path2.x * curveoffset; 
					_wheretogo.y = _wheretogo.y - path1.y * curveoffset * 1.5f + path2.y * curveoffset; 
                    
					//Debug.Log ("waypoint1:" + finalpath [avatarwhereinpath] + " waypoint2:" + finalpath [avatarwhereinpath + 1] + " waypoint3:" + finalpath [avatarwhereinpath + 2]);
					//Debug.Log ("path1:" + path1 + " path2:" + path2);
					//Debug.Log ("offset:" + _wheretogo);
				}
			}
			else
			{
				_deltavector = _wheretogo - transform.localPosition;
				waypointdistance2 = _deltavector.magnitude;
				if (waypointdistance2 < map_manager_local.mappiecesize * 0.9f && waypointdistance2 > map_manager_local.mappiecesize * 0.7f) {
					_wheretogo.x = _wheretogo.x - path1.x * map_manager_local.mappiecesize * 0.6f; 
					_wheretogo.y = _wheretogo.y - path1.y * map_manager_local.mappiecesize * 0.6f; 
				}
			}
			//Debug.Log("offset:" + _wheretogo);


            _deltavector = _wheretogo - transform.localPosition;
            _norm = _deltavector.normalized * avatarspeed * timestep;

			if (_deltavector.magnitude < _norm.magnitude || _deltavector.magnitude == 0) // on checkpoint 
			{
				_norm = _deltavector;
				avatarmoving = false;
				avatarwhereinpath = avatarwhereinpath + 1;
				avataractualposition = finalpath [avatarwhereinpath];
				avatar_old_worldposition = avatar_actual_worldposition;

                FogUpdate();

                //Debug.Log("Checking Battle: " + (int)avataractualposition.x + ", " + (int)avataractualposition.y);
                battleindex = character_definitions_local.CheckBattle((int)avataractualposition.x, (int)avataractualposition.y);
                if (battleindex>=0 && avatarwhereinpath>=1)//!!!
                {
                    //battle
                    Debug.Log("battle, path index: " + avatarwhereinpath);
                    map_manager_local.GUIBattlePopup.GetComponent<gui_battle_popup>().ShowGUI(true, battleindex);

                }

                

				if (finalpath [avatarwhereinpath + 1] == Vector2.zero) //on position
				{
					map_manager_local.avatarstatictime = 0.4f;
					map_manager_local.camerafadeouttime = 0.15f;
					map_manager_local.cameraspeed = map_manager_local.cameraspeedhi;

					avatarobject.GetComponent<Animator> ().SetTrigger ("idle");
					camera_lowfps_local.fpstime = 100;

                    //SET ACTIVE ELEMENT!!!!!!!
                    //map_manager_local.mapfield [(int)finalpath [avatarwhereinpath].x, (int)finalpath [avatarwhereinpath].y].GetComponent<map_piece_def> ().SetActiveElement (1);//SET ACTIVE ELEMENT!!!!!!!
                }


            }
            _avatarpos = _avatarpos + _norm;
            avatar_actual_worldposition = _avatarpos;
				
            transform.localPosition = _avatarpos;

			if (avatarmoving)
			{
				RotateAvatarToWaypoint (_wheretogo,1);
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
                //SET ACTIVE ELEMENT!!!!!!!
                //if (map_manager_local.mapfield [i, j] != null) map_manager_local.mapfield [i, j].GetComponent<map_piece_def> ().SetActiveElement(0);
			}
	}
		

    public void FindPath(int whereX, int whereY)
    {
        int i, j, k;
        //Vector2[] oldpath;
		Vector2 whereclick;
        int shortest = 0;

        /*
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
        }*/
        /*
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
        */
        //CheckDirection(0, whereX, whereY); //START PATHFINDING RECURSE

        setpathfindingcoroutine = GeneratePathTo(whereX, whereY);
        StartCoroutine(setpathfindingcoroutine);
        //GeneratePathTo(whereX, whereY);
        /*

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
        */
        //for (i = 0; i < maxsizepath; i++) finalpath[i] = computedpath[i];
        /*
        whereclick.x = whereX;
		whereclick.y = whereY;
		if (
            ((!avatarmoving && whereclick != avataractualposition) ||
             (avatarmoving && whereclick==finalpath[avatarwhereinpath+1] && character_definitions_local.CheckBattle((int)finalpath[avatarwhereinpath + 1].x, (int)finalpath[avatarwhereinpath + 1].y) < 0) ||
             (avatarmoving && whereclick != finalpath[avatarwhereinpath + 1])
            )
           )
        {
            if (computedpath[1] != oldpath[avatarwhereinpath+1]) // TESTING!!! BUG!!! BAD ARRAY INDEX
            {
                //Debug.Log("DIFFERENT DIRECTION PATH");
                avatarwhereinpath = 0;

                for (i = 0; i < maxsizepath; i++) finalpath[i] = computedpath[i];

                finalpath[0] = avataractualposition;
                for (i = 0; i < maxsizepath-1; i++)
                {
                    finalpath[i + 1] = computedpath[i];
                    
                    //Debug.Log("CLEANING THE PATH ID: " + shortest + " [" + i + "]: " + paths[shortest].singlepath[i]);

                }
            }
            else
            {
                //Debug.Log("SAME DIRECTION PATH");
                avatarwhereinpath = 0;
                for (i = 0; i < maxsizepath; i++) finalpath[i] = computedpath[i];
            }


			if (avatarafterstaticposition==finalpath[2] && finalpath[2]!=Vector2.zero) 
			{
				avatarwhereinpath = 1;
				//Debug.Log("Append path");  
			}
            
			
        }
        */

    }

    
    //----------pathfinding------------------------
    void GeneratePathfindingGraph()
    {
        // Initialize the array
        graph = new Node[map_manager_local.mapsize, map_manager_local.mapsize];

        // Initialize a Node for each spot in the array
        for (int x = 0; x < map_manager_local.mapsize; x++)
        {
            for (int y = 0; y < map_manager_local.mapsize; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        // Now that all the nodes exist, calculate their neighbours
        for (int x = 0; x < map_manager_local.mapsize; x++)
        {
            for (int y = 0; y < map_manager_local.mapsize; y++)
            {
                // We have a 4-way connected map
                // This also works with 6-way hexes and 8-way tiles and n-way variable areas (like EU4)

                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (x < map_manager_local.mapsize - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y]);
                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                if (y < map_manager_local.mapsize - 1)
                    graph[x, y].neighbours.Add(graph[x, y + 1]);

                //diagonal
                /*if (x > 0 && y > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                if (x < map_manager_local.mapsize - 1 && y > 0)
                    graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
                if (x > 0 && y < map_manager_local.mapsize - 1)
                    graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                if (x < map_manager_local.mapsize - 1 && y < map_manager_local.mapsize - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y + 1]);*/
            }
        }
    }

    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY, int fx, int fy)
    {
        if (UnitCanEnterTile(targetX, targetY, fx, fy) == false)
            return Mathf.Infinity;

        float cost = 1;

        /*
        if (sourceX != targetX && sourceY != targetY)
        {
            // We are moving diagonally!  Fudge the cost for tie-breaking
            // Purely a cosmetic thing!
            cost += 0.25f;
        }*/

        return cost;

    }

    public bool UnitCanEnterTile(int x, int y, int fx, int fy)
    {

        // We could test the unit's walk/hover/fly type against various
        // terrain flags here to see if they are allowed to enter the tile.

        if (fx != x || fy != y)
        {
            if (character_definitions_local.CheckBattle(x, y) >= 0)
            {
                return false;
            }
        }
        return fogfield[x, y].enabledpath;
        
    }

    IEnumerator GeneratePathTo(int x, int y)
    {
        // Clear out our unit's old path.

        int fx;
        int fy;

        fx = x;
        fy = y;

        Vector2 delta;

        if (UnitCanEnterTile(x, y, fx, fy) == false)
        {
            // We probably clicked on a mountain or something, so just quit out.
            //StopCoroutine(setpathfindingcoroutine);
            yield break;
        }
        

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        // Setup the "Q" -- the list of nodes we haven't checked yet.
        List<Node> unvisited = new List<Node>();

        Node source;

        delta.x = avatar_actual_worldposition.x + map_manager_local.mapoffset * map_manager_local.mappiecesize -20;
        delta.y = avatar_actual_worldposition.y + map_manager_local.mapoffset * map_manager_local.mappiecesize;
        Debug.Log("Avatar position:" + delta);
        Debug.Log("Avatar Path:" + finalpath[avatarwhereinpath]*map_manager_local.mappiecesize);
        delta =  delta - (finalpath[avatarwhereinpath + 1] * map_manager_local.mappiecesize);
        Debug.Log("Delta:" + delta.magnitude);
        if (!avatarmoving || delta.magnitude > 50)
        {
            source = graph[(int)avataractualposition.x, (int)avataractualposition.y];
        }
        else
        {
            source = graph[(int)finalpath[avatarwhereinpath + 1].x, (int)finalpath[avatarwhereinpath + 1].y];
        }
        Node target = graph[
                            x,
                            y
                            ];

        dist[source] = 0;
        prev[source] = null;

        // Initialize everything to have INFINITY distance, since
        // we don't know any better right now. Also, it's possible
        // that some nodes CAN'T be reached from the source,
        // which would make INFINITY a reasonable value
        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);
        }

        while (unvisited.Count > 0)
        {
            // "u" is going to be the unvisited node with the smallest distance.
            Node u = null;

            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;  // Exit the while loop!
            }

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y, fx, fy);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                    yield return null;
                }
            }
        }

        // If we get there, the either we found the shortest route
        // to our target, or there is no route at ALL to our target.

        if (prev[target] == null)
        {
            // No route between our target and the source
            //StopCoroutine(setpathfindingcoroutine);
            yield break;
        }

        List<Node> currentPath = new List<Node>();

        Node curr = target;

        // Step through the "prev" chain and add it to our path
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
            yield return null;
        }

        // Right now, currentPath describes a route from out target to our source
        // So we need to invert it!

        currentPath.Reverse();

        int i;

        for (i = 0; i < maxsizepath; i++)
        {
            finalpath[i] = Vector2.zero;
        }
        for (i = 0; i < currentPath.Count; i++)
        {
            finalpath[i].x = currentPath[i].x;
            finalpath[i].y = currentPath[i].y;
        }

        DeactivateActiveElements();
        avatarwhereinpath = 0;
        if (!avatarmoving)
        {
            avatarobject.GetComponent<Animator>().SetTrigger("run");
        }
    Debug.Log("Path length" + currentPath.Count);

    /*computedpath[currentPath.Count].x = x;
    computedpath[currentPath.Count].y = y;*/
    /*Gizmos.color = Color.blue;
    Gizmos.DrawLine(transform.position, target.position);*/
}

}