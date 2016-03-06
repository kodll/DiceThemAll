using UnityEngine;
using System.Collections;

public class minimap : MonoBehaviour {

    public GameObject mappiece;
    GameObject[,] minimapdata;
    [HideInInspector] public map_manager map_manager_local;


    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void MapInit()
    {
        int i, j;
        Vector3 pos = Vector3.zero;

        map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;

        //Debug.Log("Found map_manager: " + map_manager_local.name);
        //Debug.Log("MapSize: " + map_manager_local.mapsize);

        minimapdata = new GameObject[map_manager_local.mapsize, map_manager_local.mapsize];

        for (i = 0; i < map_manager_local.mapsize; i++)
            for (j = 0; j < map_manager_local.mapsize; j++)
            {
                if (map_manager_local.mapfield[i,j] != null)
                {
                    //Debug.Log("Found map_piece: " + map_manager_local.mapfield[i,j]);
                    minimapdata[i, j] = Instantiate(mappiece, Vector3.zero, Quaternion.identity) as GameObject;

                    pos.x = i * minimapdata[i, j].GetComponent<RectTransform>().sizeDelta.x;
                    pos.y = j * minimapdata[i, j].GetComponent<RectTransform>().sizeDelta.y;
                    minimapdata[i, j].transform.SetParent(transform);
                    minimapdata[i, j].transform.localScale = Vector3.one;
                    minimapdata[i, j].transform.localRotation = Quaternion.identity;
                    minimapdata[i, j].transform.localPosition = pos;
                }

            }
    }

    public void MapUpdate()
    {

    }
}
