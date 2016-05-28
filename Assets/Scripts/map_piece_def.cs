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
	static avatarstatemachine avatarobject_local;

    // Use this for initialization
    void Start ()
    {
		map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
		avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;
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

	IEnumerator SetDetailAnim()
	{
		yield return new WaitForSeconds(0.5f);
		avatarobject_local.avatardetail = true;

	}

	public void PressedButton()
	{
		Vector3 deltamousevector;
		float activelementangle;
		Vector3 targetdir;
		Vector3 cross;
		//Vector3 sourcedir;
		//Vector3 sourcedirnew;
		Vector3 ActiveElementZero;
		Vector3 AvatarElementZero;

		deltamousevector = map_manager_local.initialmousepos - map_manager_local.newmousepos;

		if (deltamousevector.magnitude < map_manager_local._clickdistance)
		{

			//CHEST UNLOCKED
			map_manager_local.GUIDungeonMovement.GetComponent<Animator>().SetTrigger("PanelHide");

			map_manager_local.scrollmultiplier = 1;
			map_manager_local.avatarstatictime = 0.6f;
			map_manager_local.camerafadeouttime = 0.3f;
			map_manager_local.cameraspeed = 10;

			SetActiveElement (0);
			map_manager_local.TriggerScrolling (false);
			map_manager_local.mapcamera.GetComponent<Animator> ().SetTrigger ("smalldetail_in");
			map_manager_local.GUIChestOpenedPopup.GetComponent<Animator> ().SetTrigger ("PanelShow");
			avatarobject_local.avatarcamera.GetComponent<Animator> ().SetTrigger ("zoomin");
			avatarobject_local.avatarobject.GetComponent<Animator> ().SetTrigger ("chestwaiting");
			StartCoroutine (SetDetailAnim ());
			avatarobject_local.SetHiMaterial (true);
			map_manager_local.GUIChestOpenedPopup.GetComponent<gui_chest_unlocked_popup> ().InitChestAppearance ();
			map_manager_local.GUIChestOpenedPopup.GetComponent<gui_chest_unlocked_popup> ().ActiveElementObject = this.gameObject;

			ActiveElementZero = ActiveElementObject.transform.position;
			AvatarElementZero = avatarobject_local.transform.position;
			ActiveElementZero.y = 0;
			AvatarElementZero.y = 0;

			targetdir=ActiveElementZero-AvatarElementZero;
			activelementangle = Vector3.Angle(Vector3.right,targetdir);
			cross = Vector3.Cross(Vector3.right,targetdir);
			if (cross.y > 0)
			{
				activelementangle = 360 - activelementangle;
			}
			targetdir = Vector3.zero;
			targetdir.z = activelementangle;
			avatarobject_local.transform.localEulerAngles = targetdir;
			avatarobject_local.camerafolowobject.transform.position = (ActiveElementZero + AvatarElementZero) / 2;
			ActiveElementZero = avatarobject_local.camerafolowobject.transform.position;
			ActiveElementZero.x = ActiveElementZero.x + 2f;
			avatarobject_local.camerafolowobject.transform.position = ActiveElementZero;



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
