using UnityEngine;
using System.Collections;

public class map_piece_def : MonoBehaviour
{
    [HideInInspector] public int Xcoor;
    [HideInInspector] public int Ycoor;
	public bool isroom=false;
	public GameObject ActiveElementButton = null;
	public GameObject ActiveElementObject = null;
	bool ActiveElementStateUnopened = true;

	static map_manager map_manager_local;

    // Use this for initialization
    void Start ()
    {
		map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

	public void SetActiveElement(int state)
	{
		if (ActiveElementObject != null)
		{
			if (state == 0 && ActiveElementStateUnopened)
			{
				ActiveElementButton.SetActive (false);
				ActiveElementObject.GetComponent<Animator> ().SetTrigger ("Closed");
			}
			else if (state == 1 && ActiveElementStateUnopened)
			{
				ActiveElementButton.SetActive (true);
				ActiveElementObject.GetComponent<Animator> ().SetTrigger ("Highlight");
			}
			else
			{
				ActiveElementButton.SetActive (false);
				ActiveElementObject.GetComponent<Animator> ().SetTrigger ("Open");
				ActiveElementStateUnopened = false;
				Debug.Log ("Active Element Opened");
			}	
		}

	}

	public void PressedButton()
	{
		Vector3 deltamousevector;
		deltamousevector = map_manager_local.initialmousepos - map_manager_local.newmousepos;

		if (deltamousevector.magnitude < map_manager_local._clickdistance)
		{

			//CHEST UNLOCKED
			map_manager_local.scrollmultiplier = 1;
			map_manager_local.avatarstatictime = 0.4f;
			map_manager_local.camerafadeouttime = 0.15f;
			map_manager_local.cameraspeed = 20;

			SetActiveElement (0);
			map_manager_local.TriggerScrolling (false);
			map_manager_local.charactercamera.GetComponent<Animator> ().SetTrigger ("smalldetail_in");
			//map_manager_local.GUIChestOpenedPopup.SetActive (true);
			map_manager_local.GUIChestOpenedPopup.GetComponent<Animator> ().SetTrigger ("PanelShow");
			map_manager_local.GUIChestOpenedPopup.GetComponent<gui_chest_unlocked_popup> ().ActiveElementObject = this.gameObject;
		}
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
