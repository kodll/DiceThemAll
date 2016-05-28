using UnityEngine;
using System.Collections;

public class camera_lowfps : MonoBehaviour {


	[HideInInspector] public float fpstime = 0;
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
	void Update ()
	{
		float counter;
		if (avatarobject_local.avatardetail)
		{
			counter = 0.2f;
		}
		else
		{
			counter = 0;
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
