using UnityEngine;
using System.Collections;

public class gui_chest_unlocked_popup : MonoBehaviour {

	public GameObject ActiveElementObject;
	public GameObject AvatarInfrontOfChestObject;
	static avatarstatemachine avatarobject_local;
	public bool destroyavatar = false;
	static float mytime = 0;
	public float wait;

	// Use this for initialization
	void Start ()
	{
		avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (destroyavatar)
		{
			mytime = mytime + Time.deltaTime;
			if (mytime > wait)
			{
				Destroy (AvatarInfrontOfChestObject);
				destroyavatar = false;
			}
		}
	}
	public void InitDuplicatedAvatar()
	{
		mytime = 0;
		wait = 0.25f;
		destroyavatar = false;
	}

	public void OpenChest (bool open)
	{
		if (ActiveElementObject != null)
		{
			if (open)
			{
				ActiveElementObject.GetComponent<map_piece_def> ().SetActiveElement (3);
				avatarobject_local.avatarobject.GetComponent<Animator> ().SetTrigger ("show");
			}
			else
			{
				ActiveElementObject.GetComponent<map_piece_def> ().SetActiveElement (1);
				avatarobject_local.avatarobject.GetComponent<Animator> ().SetTrigger ("show");
			}
			destroyavatar = true;
		}
		else
		{
			Debug.Log("FATAL!!!! CHest is not initialized!!!");  
		}	
	}
		
}
