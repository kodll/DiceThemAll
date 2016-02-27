using UnityEngine;
using System.Collections;

public class map_manager : MonoBehaviour
{
    public GameObject[,] mapfield;
    [HideInInspector] public int mapsize = 50;
    [HideInInspector] public minimap minimaplocal;
    public GameObject myavatar;

    [HideInInspector] public float mappiecesize = 100;
    [HideInInspector] public float mapoffset = 25;

    public float floorZ = -19;

    // Use this for initialization
    void Start ()
    {
        int i,j;

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


        



    }
	
	// Update is called once per frame
	void Update ()
    {
        myavatar.GetComponent<avatarstatemachine>().Actualize(Time.deltaTime);
	}

    void AddListener(UnityEngine.UI.Button b, int clickeditemX, int clickeditemY)
    {
        b.onClick.AddListener(() => MapClicked(clickeditemX, clickeditemY));
    }

    void MapClicked(int clickeditemX, int clickeditemY)
    {
        myavatar.GetComponent<avatarstatemachine>().FindPath(clickeditemX, clickeditemY);
    }
}
