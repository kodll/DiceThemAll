using UnityEngine;
using System.Collections;

public class global_billboard : MonoBehaviour {

	public Vector3 myrotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.eulerAngles = myrotation;
	}
}
