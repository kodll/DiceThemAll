using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class map_manager : MonoBehaviour
{
    public GameObject[,] mapfield;
    [HideInInspector] public int mapsize = 50;
    [HideInInspector] public minimap minimaplocal;
	static avatarstatemachine avatarobject_local;
	static character_definitions character_definitions_local;
    public GameObject maptapvalid;
    public GameObject mapmover;
    public GameObject mapcamera;
	public GameObject charactercamera;
    public GameObject tapcamera;
	public GameObject GUIChestOpenedPopup;
	public GameObject GUIDungeonMovement;


    [HideInInspector] public float mappiecesize = 100;
    [HideInInspector] public float mapoffset = 25;
    [HideInInspector] public Vector3 mapvectoroffset;
    [HideInInspector] public float floorZ = -19;

    [HideInInspector] public float cameraspeedlow = 1.5f;
    [HideInInspector] public float cameraspeedhi = 5.0f;
    [HideInInspector] public float cameraspeed = 1.5f;
    [HideInInspector] public float camerafadeouttime = 1.0f;
    [HideInInspector] public float avatarstatictime;
    [HideInInspector] public float scrollmultiplier;
    [HideInInspector] public Vector3 scrollpreviousframe;

    bool tap;
    float taplength;
    Ray ray;
	[HideInInspector] public bool canscrollmanually;
    Vector3 oldmousepos;
	[HideInInspector] public Vector3 newmousepos;
	[HideInInspector] public Vector3 initialmousepos;
	[HideInInspector] public float _clickdistance = 10;
    Vector3 lastdelta;
    float lastdeltatime;
    public GameObject mouse3d;
    GameObject mymouse3d;
    Plane floorcollision;
    Camera tapcameracomponent;

    public GameObject fogcontainer;
    public GameObject fogroomprefab;

    public GameObject mapcontainer;

    //---GLOBAL DEFINITIONS------------------------------------------------------
    public GameObject DiceObject;
	public GameObject PrefabPanelDicesToRollObject;

	[HideInInspector] public Vector3[] DiceNumberRotation;

	//---END OF GLOBAL DEFINITIONS------------------------------------------------------

    // Use this for initialization
    void Start ()
    {
        int i,j;
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

        mapvectoroffset = Vector3.zero;
        mapvectoroffset.x = mapoffset * mappiecesize;
        mapvectoroffset.y = mapoffset * mappiecesize;

        mapfield = new GameObject[mapsize, mapsize];
        for (i = 0; i < mapsize; i++)
            for (j = 0; j < mapsize; j++)
            {
                mapfield[i, j] = null;
            }

		avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;

		character_definitions_local = GameObject.FindObjectOfType(typeof(character_definitions)) as character_definitions;

        map_piece_def[] myItems = FindObjectsOfType(typeof(map_piece_def)) as map_piece_def[];
        foreach (map_piece_def item in myItems)
        {

            item.GetFieldCoor(mappiecesize,mapoffset);
            i = item.gameObject.GetComponent<map_piece_def>().Xcoor;
            j = item.gameObject.GetComponent<map_piece_def>().Ycoor;
            mapfield[i, j] = item.gameObject;
            UnityEngine.UI.Button button = item.gameObject.GetComponent<UnityEngine.UI.Button>();
            AddListener(button, i, j);
        }
        minimaplocal = GameObject.FindObjectOfType(typeof(minimap)) as minimap;
        //Debug.Log("Found minimap: " + minimaplocal.name);
        minimaplocal.MapInit();

		// systems initiation----------------------------------------------------------
		avatarobject_local.PathFindingInit();
		avatarobject_local.PathFindingSetRooms(mapfield);
		avatarobject_local.FogInit();
		GUIChestOpenedPopup.GetComponent<gui_chest_unlocked_popup>().InitGuiChestSystem ();

		character_definitions_local.character_definitions_init (true);

        avatarobject_local.SetCharacter(27, 22); //start position
        avatarobject_local.FogUpdate();
        //--------------------------------------------------------------------------------
        // battles -----------------------------------------------------
        character_definitions_local.AddBattle(26, 23);
        character_definitions_local.AddBattle(25, 25);

        //--------------------------------------------------------------
        canscrollmanually = true;
        avatarstatictime = 0.4f;
        camerafadeouttime = 0.3f;
        cameraspeed = cameraspeedhi;
        scrollmultiplier = 1;
        scrollpreviousframe = Vector3.zero;

        //mymouse3d = Instantiate(mouse3d, Vector3.zero, Quaternion.identity) as GameObject;
        //mymouse3d.transform.SetParent(tapcamera.transform);
        //Cursor.visible = false;

        tap = false;
        colpos = Vector3.zero;
        colpos.y = -floorZ;
        floorcollision = new Plane(Vector3.up, colpos);
        tapcameracomponent = tapcamera.GetComponent<Camera>();
        oldmousepos = Vector3.zero;
        newmousepos = Vector3.zero;

        

    }
    
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 m3d;
        Vector2 mpos;
        float rayDistance;

		avatarobject_local.Actualize (Time.deltaTime);

		oldmousepos = newmousepos;

		if (canscrollmanually)
		{
			

			mpos = Input.mousePosition;
			ray = tapcameracomponent.ScreenPointToRay (new Vector3 (mpos.x, mpos.y, 0));

			if (floorcollision.Raycast (ray, out rayDistance)) {
				m3d = ray.GetPoint (rayDistance);
				//mymouse3d.transform.position = m3d;
				newmousepos = m3d;
			}
			if (Input.GetMouseButtonDown (0)) {
				tap = true;
				lastdeltatime = 0;
				oldmousepos = newmousepos;
				initialmousepos = newmousepos;
			}
			if (Input.GetMouseButtonUp (0)) {
				tap = false;
				taplength = 0;
				//Debug.Log ("released button ");
				lastdeltatime = 1;
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

        if (tap)
        {
            lastdelta = newmousepos - oldmousepos;
            mapmover.transform.position = mapmover.transform.position + lastdelta;
            mapcamera.transform.position = -mapmover.transform.position;
            taplength = taplength + Time.deltaTime;
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
			if (deltamousevector.magnitude>=_clickdistance && tap)
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

    void AddListener(UnityEngine.UI.Button b, int clickeditemX, int clickeditemY)
    {
        b.onClick.AddListener(() => MapClicked(clickeditemX, clickeditemY));

    }

    void MapClicked(int clickeditemX, int clickeditemY)
    {
        GameObject tapindicator;
        Vector3 pos;
		Vector3 deltamousevector;

		//EventSystem.current.SetSelectedGameObject(null);
		deltamousevector = initialmousepos - newmousepos;
		if (deltamousevector.magnitude<_clickdistance)
        {
            lastdeltatime = 0;
            lastdelta = Vector3.zero;
			avatarobject_local.FindPath(clickeditemX, clickeditemY);
            tapindicator = Instantiate(maptapvalid, Vector3.zero, Quaternion.identity) as GameObject;
            tapindicator.transform.SetParent(this.transform);
            tapindicator.transform.localScale = Vector3.one;
            tapindicator.transform.localRotation = Quaternion.identity;
            pos = Vector3.zero;
            pos.x = (clickeditemX - mapoffset) * mappiecesize;
            pos.y = (clickeditemY - mapoffset) * mappiecesize;
            //pos.z = floorZ;
            tapindicator.transform.localPosition = pos;
            
			avatarstatictime = 1.5f;
			camerafadeouttime = 0.05f;
			cameraspeed = cameraspeedlow;
			scrollmultiplier = 1;
        }
    }

	public void TriggerScrolling(bool scrolling)
	{
		//Debug.Log("Scrolling: " + scrolling);
		canscrollmanually = scrolling;
		tap = false;
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
}
