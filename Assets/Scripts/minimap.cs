using UnityEngine;
using System.Collections;

public class minimap : MonoBehaviour {

    public GameObject maproom;
	public GameObject mappath;
    GameObject[,] minimapdata;
	GameObject avatarobject;
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

		avatarobject = Instantiate (mappath, Vector3.zero, Quaternion.identity) as GameObject;
		avatarobject.transform.SetParent(transform);
		avatarobject.transform.localScale = Vector3.one;
		avatarobject.transform.localRotation = Quaternion.identity;

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

					if (map_manager_local.mapfield [i, j].GetComponent<map_piece_def> ().isroom)
					{
						//Debug.Log("Drawn room");
						minimapdata [i, j] = Instantiate (maproom, Vector3.zero, Quaternion.identity) as GameObject;
					} else
					{
						minimapdata [i, j] = Instantiate (mappath, Vector3.zero, Quaternion.identity) as GameObject;
					}

                    pos.x = i * 4;
                    pos.y = j * 4;
                    minimapdata[i, j].transform.SetParent(transform);
                    minimapdata[i, j].transform.localScale = Vector3.one;
                    minimapdata[i, j].transform.localRotation = Quaternion.identity;
                    minimapdata[i, j].transform.localPosition = pos;
					MapUpdate (i, j, 0);
                }

            }
    }

	public void MapUpdate(int x, int y, int state) // state 0 - hidden, state 1 - visible, state 2 - visited, state 3 - avatar
    {
		Color newcolor;
		newcolor = Color.white;
		Vector3 pos = Vector3.zero;


		if (state == 0)
		{
			newcolor.a = 0f;
		}
		else if (state == 1)
		{
			newcolor.a = 0.1f;
		}	
		else if (state == 2)
		{
			newcolor.a = 0.2f;
		}	
		else if (state == 3)
		{
			pos.x = x * 4;
			pos.y = y * 4;
			avatarobject.transform.localPosition = pos;
		}
		if (map_manager_local.mapfield [x, y] != null && state !=3) 
		{
			minimapdata [x, y].GetComponent<UnityEngine.UI.Image> ().color = newcolor;
		}

    }
}
