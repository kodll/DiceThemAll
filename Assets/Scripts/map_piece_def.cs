using UnityEngine;
using System.Collections;

public class map_piece_def : MonoBehaviour
{
    [HideInInspector] public int Xcoor;
    [HideInInspector] public int Ycoor;


    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    public void GetFieldCoor(float mappiecesize, float mapoffset)
    {
        float val;
        val = transform.position.x / mappiecesize + mapoffset;
        Xcoor = (int)val;
        val = transform.position.z / mappiecesize + mapoffset;
        Ycoor = (int)val;
    }
}
