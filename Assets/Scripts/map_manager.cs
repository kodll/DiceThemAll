using UnityEngine;
using System.Collections;

public class map_manager : MonoBehaviour
{
    public GameObject[,] mapfield;
    [HideInInspector] public int mapsize = 50;
    [HideInInspector] public minimap minimaplocal;
    public GameObject myavatar;
    public GameObject maptapvalid;
    public GameObject mapcontainer;

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

    // Use this for initialization
    void Start ()
    {
        int i,j;

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



    }
	
	// Update is called once per frame
	void Update ()
    {
        myavatar.GetComponent<avatarstatemachine>().Actualize(Time.deltaTime);
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

        Debug.Log("Scroll multi: " + scrollmultiplier);

        if (avatarstatictime > 0) avatarstatictime = avatarstatictime - Time.deltaTime * camerafadeouttime;

        if (avatarstatictime > 0)
        {
            actual = -mapcontainer.transform.localPosition;
            delta = actual - scrollpreviousframe;

            
            //Debug.Log("Scroll delta: " + (delta.magnitude / Time.deltaTime));
            //Debug.Log("Time delta: " + Time.deltaTime);

            if (delta.magnitude / Time.deltaTime > 800)
            {
                scrollmultiplier = -4.0f;
            }
            if (scrollmultiplier > 1) scrollmultiplier = 1;

            cuttedscrollmultiplier = scrollmultiplier;
            if (cuttedscrollmultiplier < 0) cuttedscrollmultiplier = 0;

            



            newpos = actual - myavatar.transform.localPosition;
            newpos = -actual + newpos * Time.deltaTime * cameraspeed * avatarstatictime * cuttedscrollmultiplier;
            newpos.z = 0;
            
            

            mapcontainer.transform.localPosition = newpos;
            scrollpreviousframe = -newpos;
        }
    }

    void AddListener(UnityEngine.UI.Button b, int clickeditemX, int clickeditemY)
    {
        b.onClick.AddListener(() => MapClicked(clickeditemX, clickeditemY));
    }

    void MapClicked(int clickeditemX, int clickeditemY)
    {
        GameObject tap;
        Vector3 pos;


        myavatar.GetComponent<avatarstatemachine>().FindPath(clickeditemX, clickeditemY);
        tap = Instantiate(maptapvalid, Vector3.zero, Quaternion.identity) as GameObject;
        tap.transform.SetParent(mapcontainer.transform);
        tap.transform.localScale = Vector3.one;
        tap.transform.localRotation = Quaternion.identity;
        pos = Vector3.zero;
        pos.x = (clickeditemX - mapoffset) * mappiecesize;
        pos.y = (clickeditemY - mapoffset) * mappiecesize;
        //pos.z = floorZ;
        tap.transform.localPosition = pos;
        avatarstatictime = 0.9f;
        camerafadeouttime = 0.05f;
        cameraspeed = cameraspeedlow;
        scrollmultiplier = 1;
    }
}
