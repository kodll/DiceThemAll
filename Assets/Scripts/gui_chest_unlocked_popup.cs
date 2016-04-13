using UnityEngine;
using System.Collections;

public class gui_chest_unlocked_popup : MonoBehaviour {

	public GameObject ActiveElementObject;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	public void OpenChest (bool open)
	{
		if (ActiveElementObject != null)
		{
			if (open)
			{
				ActiveElementObject.GetComponent<map_piece_def> ().SetActiveElement (3);
			}
			else
			{
				ActiveElementObject.GetComponent<map_piece_def> ().SetActiveElement (1);
			}
		}
		else
		{
			Debug.Log("FATAL!!!! CHest is not initialized!!!");  
		}	
	}
		
}
