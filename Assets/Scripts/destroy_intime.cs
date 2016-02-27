using UnityEngine;
using System.Collections;

public class destroy_intime : MonoBehaviour {

    public float wait;
    static float mytime = 0;

	// Use this for initialization
	void Start ()
    {
        mytime = 0; 
	}
	
	// Update is called once per frame
	void Update ()
    {
        mytime = mytime + Time.deltaTime;
        if (mytime > wait) Destroy(this.gameObject);

    }
}
