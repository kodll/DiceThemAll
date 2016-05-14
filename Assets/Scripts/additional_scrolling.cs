using UnityEngine;
using System.Collections;

public class additional_scrolling : MonoBehaviour {
	public GameObject followcamera;
	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.localPosition = followcamera.transform.localPosition*speed;
	}
}
