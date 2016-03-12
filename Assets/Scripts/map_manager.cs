using UnityEngine;
using System.Collections;

public class map_manager : MonoBehaviour
{
    public GameObject[,] mapfield;
    [HideInInspector] public int mapsize = 50;
    [HideInInspector] public minimap minimaplocal;
    public GameObject myavatar;
    public GameObject maptapvalid;
    public GameObject mapmover;
    public GameObject mapcamera;
    public GameObject tapcamera;


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
    Vector3 oldmousepos;
    Vector3 newmousepos;
    Vector3 lastdelta;
    float lastdeltatime;
    public GameObject mouse3d;
    GameObject mymouse3d;
    Plane floorcollision;
    Camera tapcameracomponent;

    // Use this for initialization
    void Start ()
    {
        int i,j;
        Vector3 colpos;

        mapvectoroffset = Vector3.zero;
        mapvectoroffset.x = mapoffset * mappiecesize;
        mapvectoroffset.y = mapoffset * mappiecesize;

        mapfield = new GameObject[mapsize, mapsize];

        for (i = 0; i < mapsize; i++)
            for (j = 0; j < mapsize; j++)
            {
                mapfield[i, j] = null;
            }



        map_piece_def[] myItems = FindObjectsOfType(typeof(map_piece_def)) as map_piece_def[];

        //Debug.Log("Found " + myItems.Length + " map pieces");

        foreach (map_piece_def item in myItems)
        {

            item.GetFieldCoor(mappiecesize,mapoffset);
            //Debug.Log("X " + item.gameObject.GetComponent<map_piece_def>().Xcoor + ", Y " + item.gameObject.GetComponent<map_piece_def>().Ycoor);

            i = item.gameObject.GetComponent<map_piece_def>().Xcoor;
            j = item.gameObject.GetComponent<map_piece_def>().Ycoor;
            mapfield[i, j] = item.gameObject;

            //Debug.Log("Found: " + mapfield[i,j].name + " X:" + i + " Y:" +j);

            UnityEngine.UI.Button button = item.gameObject.GetComponent<UnityEngine.UI.Button>();
            AddListener(button, i, j);
        }
        minimaplocal = GameObject.FindObjectOfType(typeof(minimap)) as minimap;
        //Debug.Log("Found minimap: " + minimaplocal.name);
        minimaplocal.MapInit();

        myavatar.GetComponent<avatarstatemachine>().PathFindingInit();

        myavatar.GetComponent<avatarstatemachine>().PathFindingSetRooms(mapfield);

        myavatar.GetComponent<avatarstatemachine>().SetCharacter(25, 26);

        avatarstatictime = 0.4f;
        camerafadeouttime = 0.3f;
        cameraspeed = cameraspeedhi;
        scrollmultiplier = 1;
        scrollpreviousframe = Vector3.zero;

        //mymouse3d = Instantiate(mouse3d, Vector3.zero, Quaternion.identity) as GameObject;
        //mymouse3d.transform.SetParent(tapcamera.transform);

        tap = false;
        colpos = Vector3.zero;
        colpos.y = -floorZ;
        floorcollision = new Plane(Vector3.up, colpos);
        tapcameracomponent = tapcamera.GetComponent<Camera>();
        //Cursor.visible = false;
        oldmousepos = Vector3.zero;
        newmousepos = Vector3.zero;


    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 m3d;
        Vector2 mpos;
        float rayDistance;

        oldmousepos = newmousepos;
        myavatar.GetComponent<avatarstatemachine>().Actualize(Time.deltaTime);
        mpos = Input.mousePosition;
        ray = tapcameracomponent.ScreenPointToRay(new Vector3(mpos.x, mpos.y, 0));

        if (floorcollision.Raycast(ray, out rayDistance))
        {
            m3d = ray.GetPoint(rayDistance);
            //mymouse3d.transform.position = m3d;
            newmousepos = m3d;
        }
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            lastdeltatime = 0;
            oldmousepos = newmousepos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            tap = false;
            taplength=0;
            Debug.Log("released button ");
            lastdeltatime = 1;
        }

        ScrollMap();
    }

    void ScrollMap()
    {
        Vector3 actual;
        Vector3 newpos;
        Vector3 delta;
        float cuttedscrollmultiplier;

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
            delta = actual - scrollpreviousframe;

            
            //Debug.Log("Scroll delta: " + (delta.magnitude / Time.deltaTime));
            //Debug.Log("Time delta: " + Time.deltaTime);

            if (taplength >= 0.15)
            {
                scrollmultiplier = -4.0f;
            }
            if (scrollmultiplier > 1) scrollmultiplier = 1;

            cuttedscrollmultiplier = scrollmultiplier;
            if (cuttedscrollmultiplier < 0) cuttedscrollmultiplier = 0;

            



            newpos = actual - myavatar.transform.localPosition;
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

        if (taplength < 0.15f)
        {
            lastdeltatime = 0;
            lastdelta = Vector3.zero;
            myavatar.GetComponent<avatarstatemachine>().FindPath(clickeditemX, clickeditemY);
            tapindicator = Instantiate(maptapvalid, Vector3.zero, Quaternion.identity) as GameObject;
            tapindicator.transform.SetParent(this.transform);
            tapindicator.transform.localScale = Vector3.one;
            tapindicator.transform.localRotation = Quaternion.identity;
            pos = Vector3.zero;
            pos.x = (clickeditemX - mapoffset) * mappiecesize;
            pos.y = (clickeditemY - mapoffset) * mappiecesize;
            //pos.z = floorZ;
            tapindicator.transform.localPosition = pos;
            avatarstatictime = 0.9f;
            camerafadeouttime = 0.05f;
            cameraspeed = cameraspeedlow;
            scrollmultiplier = 1;
        }
    }
}
