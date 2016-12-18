using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class map_manager : MonoBehaviour
{
    [System.Serializable]
    public struct wallquadrant
    {
        public GameObject wallobject;
        public int walltypeNWCW;
    }
    public wallquadrant[] wallquadrantsdef;

    [System.Serializable]
    public struct floorquadrant
    {
        public GameObject tileobject;
        public int tiletypeNWCW;
    }
    public floorquadrant[] floorquadrantsdef;

    public struct quadrantobj
    {
        public GameObject quadrantobject;
    }

    [System.Serializable]
    public struct quadrant
    {
        public int quadrantID;
    }

    [System.Serializable]
    public struct maptile
    {
        public bool isroom;
        public quadrant[] quadrantsfield;
    }
    public struct maptileobj
    {
        public quadrantobj[] quadrantsfield;
    }

    [System.Serializable]
    public struct mapdef
    {
        public maptile[,] mapwalls;
    }

    maptileobj[,] mapwallobjects;
    quadrantobj[,] mapfloorobjects;
    public mapdef dungeonmap;

    //public GameObject[,] mapfield;
    public int mapsize = 30;
    [HideInInspector] public minimap minimaplocal;
	static avatarstatemachine avatarobject_local;
    [HideInInspector] public character_definitions character_definitions_local;
    public GameObject maptapvalid;
    public GameObject mapmover;
    public GameObject mapcamera;
	public GameObject charactercamera;
    public GameObject tapcamera;
	public GameObject GUIChestOpenedPopup;
	public GameObject GUIDungeonMovement;
    public GameObject GUIBattlePopup;
    public GameObject GUIEditor;


    [HideInInspector] public float mappiecesize = 100;
    public float mapoffset = 15;
    [HideInInspector] public float floorZ = -19;

    [HideInInspector] public float cameraspeedlow = 1.5f;
    [HideInInspector] public float cameraspeedhi = 5.0f;
    [HideInInspector] public float cameraspeed = 1.5f;
    [HideInInspector] public float camerafadeouttime = 1.0f;
    [HideInInspector] public float avatarstatictime;
    [HideInInspector] public float scrollmultiplier;
    [HideInInspector] public Vector3 scrollpreviousframe;

    Ray ray;
	[HideInInspector] public bool canscrollmanually;
    Vector3 oldmousepos;
	[HideInInspector] public Vector3 newmousepos;
	[HideInInspector] public Vector3 initialmousepos;
    [HideInInspector] public Vector3 worldmousepos;
    [HideInInspector] public float _clickdistance = 10;
    Vector3 lastdelta;
    float lastdeltatime;
    public GameObject mouse3d;
    public GameObject mouse3droomindicator;
    GameObject mymouse3d;
    GameObject mymouse3droomindicator;
    Vector2 mouseposmap;
    float clickrepeater;
    float clickrepeatertime = 0.2f;
    bool leftclick;
    bool rightclick;
    bool altclick;
    Plane floorcollision;
    Camera tapcameracomponent;
    Camera charactercameracomponent;

    public GameObject fogcontainer;
    public GameObject fogroomprefab;

    public GameObject fogofwarobject;

    public GameObject mapcontainer;

    public int gamemode; //editor = 10, dungeon = 1

    //---GLOBAL DEFINITIONS------------------------------------------------------
    public GameObject DiceObject;
	public GameObject PrefabPanelDicesToRollObject;

	[HideInInspector] public Vector3[] DiceNumberRotation;

	//---END OF GLOBAL DEFINITIONS------------------------------------------------------
    public void SaveMap()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string name;
        FileStream file;

        name = "Assets/Resources/maps/level1.map";

        if (!File.Exists(name))
        {
            file = File.Create(name);
        }
        else
        {
            file = File.Open(name, FileMode.Open);
        }

        bf = new BinaryFormatter();
 
        bf.Serialize(file, dungeonmap);
        file.Close();
    }

    public void LoadMap()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string name;
        FileStream file;

        name = "Assets/Resources/maps/level1.map";

        if (File.Exists(name))
        {


            file = File.Open(name,FileMode.Open);


            bf = new BinaryFormatter();

            DestroyMap(false);

            dungeonmap = (mapdef)bf.Deserialize(file);
            file.Close();
            FillMapArt();
        }
        else
        {
            Debug.Log("Error!!!");
        }
    }

    // Use this for initialization
    void Start ()
    {
        int i,j,k;
        Vector3 colpos;

		//------------------------DICES ROTATION-------------------------------
		DiceNumberRotation = new Vector3[6];

		for (i = 0; i < 6; i++)
		{
			DiceNumberRotation[i] = Vector3.zero;
		}
		DiceNumberRotation [1].x = 90;
		DiceNumberRotation [2].y = 90;
		DiceNumberRotation [2].x = 90;
		DiceNumberRotation [3].y = 270;
		DiceNumberRotation [4].x = 90;
		DiceNumberRotation [4].y = 180;
		DiceNumberRotation [5].y = 180;

		//----------------------------------------------------------------------

        dungeonmap.mapwalls = new maptile[mapsize, mapsize];
        mapwallobjects = new maptileobj[mapsize, mapsize];


		avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;

        character_definitions_local = GameObject.FindObjectOfType(typeof(character_definitions)) as character_definitions;

        map_piece_def[] myItems = FindObjectsOfType(typeof(map_piece_def)) as map_piece_def[];

        foreach (map_piece_def item in myItems) //delete scene data
        {

            Destroy(item.gameObject);
        }

        DestroyMap(true);

        minimaplocal = GameObject.FindObjectOfType(typeof(minimap)) as minimap;

        // systems initiation----------------------------------------------------------
        minimaplocal.MapInit(true);
        avatarobject_local.PathFindingInit();
		avatarobject_local.FogInit(true);
		GUIChestOpenedPopup.GetComponent<gui_chest_unlocked_popup>().InitGuiChestSystem ();

        character_definitions_local.character_definitions_init (true);

        //mymouse3d = Instantiate(mouse3d, Vector3.zero, Quaternion.identity) as GameObject;
        //mymouse3d.transform.SetParent(mapcontainer.transform);
        mymouse3droomindicator = Instantiate(mouse3droomindicator, Vector3.zero, Quaternion.identity) as GameObject;
        mymouse3droomindicator.transform.SetParent(mapcontainer.transform);
        //Cursor.visible = false;
        //--------------------------------------------------------------------------------
        // main hero init-----------------------------------------------------

        gamemode = 10;
        if (gamemode == 1)
        {
            LoadMap();
        }

        avatarobject_local.SetCharacter(26, 23); //start position
        // battles -----------------------------------------------------
        character_definitions_local.AddBattle(27, 22, 0, 1);
        character_definitions_local.AddBattle(25, 25, 1, 1);

        //--------------------------------------------------------------
        clickrepeater = 0;
        leftclick = false;
        rightclick = false;
        altclick = false;

        canscrollmanually = true;
        avatarstatictime = 0.4f;
        camerafadeouttime = 0.3f;
        cameraspeed = cameraspeedhi;
        scrollmultiplier = 1;
        scrollpreviousframe = Vector3.zero;

        
        colpos = Vector3.zero;
        colpos.y = -floorZ;
        floorcollision = new Plane(Vector3.up, colpos);
        tapcameracomponent = tapcamera.GetComponent<Camera>();
        charactercameracomponent = charactercamera.GetComponent<Camera>();
        oldmousepos = Vector3.zero;
        newmousepos = Vector3.zero;

        SwitchGameMode(gamemode);


    }
    
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 m3d;
        Vector2 mpos;
        float rayDistance;

        if (gamemode==1)//dungeon
        {
            avatarobject_local.Actualize(Time.deltaTime);
        }
		    

		oldmousepos = newmousepos;

		if (canscrollmanually)
		{
			

			mpos = Input.mousePosition;

            ray = charactercameracomponent.ScreenPointToRay(new Vector3(mpos.x, mpos.y, 0));
            if (floorcollision.Raycast(ray, out rayDistance))
            {
                m3d = ray.GetPoint(rayDistance);
                worldmousepos = m3d;
                mouseposmap.x = worldmousepos.x / mappiecesize + mapoffset;
                mouseposmap.y = worldmousepos.z / mappiecesize + mapoffset;

                m3d.x = Mathf.Round(mouseposmap.x - mapoffset) * mappiecesize;
                m3d.z = Mathf.Round(mouseposmap.y - mapoffset) * mappiecesize;
                m3d.y = 0;
                mymouse3droomindicator.transform.position = m3d;
            }

            ray = tapcameracomponent.ScreenPointToRay (new Vector3 (mpos.x, mpos.y, 0));

			if (floorcollision.Raycast (ray, out rayDistance)) {
				m3d = ray.GetPoint (rayDistance);
				
				newmousepos = m3d;
			}
            if (Input.GetMouseButtonDown (0)) {
                leftclick = true; 
               
            }
			if (Input.GetMouseButtonUp (0)) {
                clickrepeater = 0;
                leftclick = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                altclick = true;
                Debug.Log("alt pressed");
            }
            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                altclick = false;
                Debug.Log("alt released");
            }

            if (leftclick)
            {
                if (clickrepeater > clickrepeatertime)
                {
                    clickrepeater = 0;
                }
                if (clickrepeater == 0) MouseClicked();
                clickrepeater = clickrepeater + Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(1))
            {
                rightclick = true;
                lastdeltatime = 0;
                oldmousepos = newmousepos;
                initialmousepos = newmousepos;
            }
            if (Input.GetMouseButtonUp(1))
            {
                rightclick = false;
              
                //Debug.Log ("released button ");
                lastdeltatime = 1;
                //MouseClicked();
            }



        }
		ScrollMap ();
    }

    void ScrollMap()
    {
        Vector3 actual;
        Vector3 newpos;
		Vector3 follow;
        float cuttedscrollmultiplier;
		Vector3 deltamousevector;

        scrollmultiplier = scrollmultiplier + Time.deltaTime * 3;
        if (scrollmultiplier > 1) scrollmultiplier = 1;

        //Debug.Log("Scroll multi: " + scrollmultiplier);

        if (rightclick)
        {
            lastdelta = newmousepos - oldmousepos;
            mapmover.transform.position = mapmover.transform.position + lastdelta;
            mapcamera.transform.position = -mapmover.transform.position;
        
            //Debug.Log("Scroll delta: " + lastdelta);
            

        }
        else if (lastdeltatime > 0)
        {
            mapmover.transform.position = mapmover.transform.position + lastdelta * lastdeltatime;
            mapcamera.transform.position = -mapmover.transform.position;
            lastdeltatime = lastdeltatime * 0.8f;
        }

        if (avatarstatictime > 0) avatarstatictime = avatarstatictime - Time.deltaTime * camerafadeouttime;

        if (avatarstatictime > 0)
        {
            actual = -mapmover.transform.localPosition;
           
            //Debug.Log("Scroll delta: " + (delta.magnitude / Time.deltaTime));
            //Debug.Log("Time delta: " + Time.deltaTime);


			deltamousevector = initialmousepos - newmousepos;
			if (deltamousevector.magnitude>=_clickdistance && rightclick)
            {
                scrollmultiplier = -6.0f;
            }
            if (scrollmultiplier > 1) scrollmultiplier = 1;

            cuttedscrollmultiplier = scrollmultiplier;
            if (cuttedscrollmultiplier < 0) cuttedscrollmultiplier = 0;

            


			follow.x = avatarobject_local.camerafolowobject.transform.position.x;
			follow.y = avatarobject_local.camerafolowobject.transform.position.z;
			follow.z = avatarobject_local.camerafolowobject.transform.position.y;

			newpos = actual - follow;
			newpos = -actual + newpos * Time.deltaTime * cameraspeed * avatarstatictime * cuttedscrollmultiplier;
            newpos.z = 0;
            
            

            mapmover.transform.localPosition = newpos;
            mapcamera.transform.localPosition = -newpos;
            scrollpreviousframe = -newpos;
        }
  
    }

    

    void MouseClicked()
    {
        GameObject tapindicator;
        Vector3 pos;
        //Vector3 deltamousevector;
        Vector2 posmap;

        //deltamousevector = initialmousepos - newmousepos;
        if (/*deltamousevector.magnitude < _clickdistance && */!EventSystem.current.IsPointerOverGameObject())
        {
            if (gamemode == 1)//dungeon
            {
                posmap = avatarobject_local.ClosestRoom(mouseposmap);
                if (posmap != Vector2.zero)
                {
                    lastdeltatime = 0;
                    lastdelta = Vector3.zero;
                    avatarobject_local.FindPath((int)posmap.x, (int)posmap.y);

                    /*
                    tapindicator = Instantiate(maptapvalid, Vector3.zero, Quaternion.identity) as GameObject;
                    tapindicator.transform.SetParent(this.transform);
                    tapindicator.transform.localScale = Vector3.one;
                    tapindicator.transform.localRotation = Quaternion.identity;
                    pos = Vector3.zero;
                    pos.x = (posmap.x - mapoffset) * mappiecesize;
                    pos.y = (posmap.y - mapoffset) * mappiecesize;
                    //pos.z = floorZ;
                    tapindicator.transform.localPosition = pos;
                    */

                    avatarstatictime = 1.5f;
                    camerafadeouttime = 0.05f;
                    cameraspeed = cameraspeedlow;
                    scrollmultiplier = 1;
                }
            }
            else if (gamemode == 10)
            {
                posmap.x = Mathf.Round(mouseposmap.x);
                posmap.y = Mathf.Round(mouseposmap.y);
                if (posmap.x < mapsize-5 && posmap.x > 5 && posmap.y < mapsize - 5 && posmap.y > 5)
                EditTile((int)posmap.x, (int)posmap.y);
            }
        }
    }

	public void TriggerScrolling(bool scrolling)
	{
		//Debug.Log("Scrolling: " + scrolling);
		canscrollmanually = scrolling;
        rightclick = false;
		if (scrolling)
		{
			//Time.timeScale = 1.0F;
		}
		else
		{
			//Time.timeScale = 0.2F;
			avatarobject_local.AvatarStop();
		}
	}
    public void SwitchGameMode(int mode)
    {
        if (mode == 1)
        {
            mymouse3droomindicator.SetActive(false);
            fogofwarobject.SetActive(true);
            GUIDungeonMovement.GetComponent<Animator>().SetTrigger("PanelShow");
            GUIEditor.GetComponent<Animator>().SetTrigger("PanelHide");
            minimaplocal.MapInit(false);
            avatarobject_local.FogInit(false);
            avatarobject_local.FogUpdate((int)avatarobject_local.avataractualposition.x, (int)avatarobject_local.avataractualposition.y);
        }
        else if (mode ==10)
        {
            mymouse3droomindicator.SetActive(true);
            fogofwarobject.SetActive(false);
            GUIDungeonMovement.GetComponent<Animator>().SetTrigger("PanelHide");
            GUIEditor.GetComponent<Animator>().SetTrigger("PanelShow");
        }
        gamemode = mode;
        
    }

    void EditTile(int x, int y)
    {
        if (dungeonmap.mapwalls[x, y].isroom)
        {
            if (altclick)
            {
                DeleteTile(x, y);
            }
            else
            {
                DeleteTile(x, y);
                CreateTile(x, y, 1);
            }
            
        }
        else
        {
            if (!altclick)
            {
                CreateTile(x, y, 1);
            }
        }
            
    }
    void SetQuadrantID(int x, int y, int quadrant)
    {
        int id;
        if (quadrant == 0)
        {
            id = 10;
            if (dungeonmap.mapwalls[x - 1, y].isroom)
                id = id + 100;
            if (dungeonmap.mapwalls[x - 1, y - 1].isroom && dungeonmap.mapwalls[x - 1, y].isroom && dungeonmap.mapwalls[x, y - 1].isroom)
                id = id + 1000;
            if (dungeonmap.mapwalls[x , y - 1].isroom)
                id = id + 1;

            dungeonmap.mapwalls[x, y].quadrantsfield[0].quadrantID = id; 
        }
        else if (quadrant == 1)
        {
            id = 1;
            if (dungeonmap.mapwalls[x - 1, y].isroom)
                id = id + 1000;
            if (dungeonmap.mapwalls[x - 1, y + 1].isroom && dungeonmap.mapwalls[x - 1, y].isroom && dungeonmap.mapwalls[x, y + 1].isroom)
                id = id + 100;
            if (dungeonmap.mapwalls[x, y + 1].isroom)
                id = id + 10;

            dungeonmap.mapwalls[x, y].quadrantsfield[1].quadrantID = id;
        }
        else if (quadrant == 2)
        {
            id = 1000;
            if (dungeonmap.mapwalls[x, y + 1].isroom)
                id = id + 100;
            if (dungeonmap.mapwalls[x + 1, y + 1].isroom && dungeonmap.mapwalls[x, y + 1].isroom && dungeonmap.mapwalls[x + 1, y].isroom)
                id = id + 10;
            if (dungeonmap.mapwalls[x + 1, y].isroom)
                id = id + 1;

            dungeonmap.mapwalls[x, y].quadrantsfield[2].quadrantID = id;
        }
        else
        {
            id = 100;
            if (dungeonmap.mapwalls[x + 1, y].isroom)
                id = id + 10;
            if (dungeonmap.mapwalls[x + 1, y - 1].isroom && dungeonmap.mapwalls[x + 1, y].isroom && dungeonmap.mapwalls[x, y - 1].isroom)
                id = id + 1;
            if (dungeonmap.mapwalls[x, y - 1].isroom)
                id = id + 1000;

            dungeonmap.mapwalls[x, y].quadrantsfield[3].quadrantID = id;
        }
    }

    GameObject FindWall(int quadrantID)
    {
        int i;
        for (i = 0; i < wallquadrantsdef.Length; i++)
        {
            if (wallquadrantsdef[i].walltypeNWCW == quadrantID)
                return wallquadrantsdef[i].wallobject;
        }
            return wallquadrantsdef[12].wallobject;
    }

    void SetupTileWalls(int x, int y, bool newtile)
    {
        GameObject wallobject;
        Vector3 pos;
        Vector3 rot;
        int i;

        rot.x = 0;
        rot.y = 180;
        rot.z = 270;

        if (newtile || dungeonmap.mapwalls[x, y].isroom)
        {
            for (i = 0; i < 4; i++)
            {
                SetQuadrantID(x, y, i);
                wallobject = FindWall(dungeonmap.mapwalls[x, y].quadrantsfield[i].quadrantID);


                if (mapwallobjects[x, y].quadrantsfield[i].quadrantobject != null) Destroy(mapwallobjects[x, y].quadrantsfield[i].quadrantobject);

                mapwallobjects[x, y].quadrantsfield[i].quadrantobject = Instantiate(wallobject, Vector3.zero, Quaternion.identity) as GameObject;
                mapwallobjects[x, y].quadrantsfield[i].quadrantobject.transform.SetParent(mapcontainer.transform);
                mapwallobjects[x, y].quadrantsfield[i].quadrantobject.transform.localEulerAngles = rot;
            }

            pos = Vector3.zero;
            pos.x = (x - mapoffset) * mappiecesize - 50;
            pos.y = (y - mapoffset) * mappiecesize;
            mapwallobjects[x, y].quadrantsfield[0].quadrantobject.transform.localPosition = pos;
            pos.x = (x - mapoffset) * mappiecesize - 50;
            pos.y = (y - mapoffset) * mappiecesize + 50;
            mapwallobjects[x, y].quadrantsfield[1].quadrantobject.transform.localPosition = pos;
            pos.x = (x - mapoffset) * mappiecesize;
            pos.y = (y - mapoffset) * mappiecesize + 50;
            mapwallobjects[x, y].quadrantsfield[2].quadrantobject.transform.localPosition = pos;
            pos.x = (x - mapoffset) * mappiecesize;
            pos.y = (y - mapoffset) * mappiecesize;
            mapwallobjects[x, y].quadrantsfield[3].quadrantobject.transform.localPosition = pos;
        }
    }

    void CreateTile (int x, int y, int type)
    {
        GameObject wallobject;
        Vector3 pos;
        Vector3 rot;
        int i;

        dungeonmap.mapwalls[x, y].isroom = true;

        SetupTileWalls(x, y, true);
        SetupTileWalls(x-1, y, false);
        SetupTileWalls(x+1, y, false);
        SetupTileWalls(x, y+1, false);
        SetupTileWalls(x, y-1, false);
        SetupTileWalls(x - 1, y - 1, false);
        SetupTileWalls(x + 1, y - 1, false);
        SetupTileWalls(x - 1, y + 1, false);
        SetupTileWalls(x + 1, y + 1, false);

    }

    void DeleteTile (int x, int y)
    {
        int i;

        dungeonmap.mapwalls[x, y].isroom = false;
        for (i = 0; i < 4; i++)
        {
            if (mapwallobjects[x, y].quadrantsfield[i].quadrantobject != null) Destroy(mapwallobjects[x, y].quadrantsfield[i].quadrantobject);
        }
        SetupTileWalls(x - 1, y, false);
        SetupTileWalls(x + 1, y, false);
        SetupTileWalls(x, y + 1, false);
        SetupTileWalls(x, y - 1, false);
        SetupTileWalls(x - 1, y - 1, false);
        SetupTileWalls(x + 1, y - 1, false);
        SetupTileWalls(x - 1, y + 1, false);
        SetupTileWalls(x + 1, y + 1, false);
    }
    public void FillMapArt()
    {
        int i, j;

        for (i = 1; i < mapsize - 1; i++)
            for (j = 1; j < mapsize - 1; j++)
                SetupTileWalls(i, j, false);

        minimaplocal.MapInit(false);
        //avatarobject_local.PathFindingInit();
        Debug.Log("Filling map art");
        avatarobject_local.FogInit(false);      
        
    }

    public void DestroyMap(bool firsttime)
    {
        int i, j, k;

        for (i = 0; i < mapsize; i++)
            for (j = 0; j < mapsize; j++)
            {
                dungeonmap.mapwalls[i, j].isroom = false;
                if (firsttime)
                {
                    dungeonmap.mapwalls[i, j].quadrantsfield = new quadrant[4];
                    mapwallobjects[i, j].quadrantsfield = new quadrantobj[4];
                }

                for (k = 0; k < 4; k++)
                {
                    dungeonmap.mapwalls[i, j].quadrantsfield[k].quadrantID = 0;
                    if (mapwallobjects[i, j].quadrantsfield[k].quadrantobject != null)
                        Destroy(mapwallobjects[i, j].quadrantsfield[k].quadrantobject);
                    mapwallobjects[i, j].quadrantsfield[k].quadrantobject = null;
                }
            }
    }

}
