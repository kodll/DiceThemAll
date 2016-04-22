using UnityEngine;
using System.Collections;

public class camera_lowfps : MonoBehaviour {

	float fpstime = 0;
	Camera mycamera;
	static avatarstatemachine avatarobject_local;
	// Use this for initialization
	void Start ()
	{
		mycamera = this.gameObject.GetComponent<Camera> ();
		avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;
		fpstime = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float counter;

		counter = 0.12f;
		if (avatarobject_local.avatarmoving)
		{
			counter = 0.05f;
		}

		fpstime = fpstime + Time.deltaTime;

		if (fpstime>counter)
		{
			mycamera.enabled = true;
			fpstime = 0;
		}
		else
		{
			mycamera.enabled = false;
		}	
	}
}
