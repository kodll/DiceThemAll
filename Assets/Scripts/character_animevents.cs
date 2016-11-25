using UnityEngine;
using System.Collections;

public class character_animevents : MonoBehaviour
{
    public GameObject[] particlestep;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void AnimEvent(int i)
    {
        GameObject go;
        if (i==1) //step
        {
            go = Instantiate(particlestep[0], Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(this.transform);
            go.transform.localPosition = Vector3.zero;
        }
    }
}
