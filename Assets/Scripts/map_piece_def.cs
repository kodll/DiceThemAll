using UnityEngine;
using System.Collections;

public class map_piece_def : MonoBehaviour
{
	
    [HideInInspector] public int Xcoor;
    [HideInInspector] public int Ycoor;

	[HideInInspector] public bool cancelzoom = false;

	public bool isroom=false;
	public GameObject ActiveElementButton = null;
	public GameObject ActiveElementObject = null;
	bool ActiveElementStateUnopened = true;
	public int ActiveElementType;
	//[HideInInspector] public bool cancel;

	static map_manager map_manager_local;
	static avatarstatemachine avatarobject_local;
	static camera_lowfps camera_lowfps_local;

    // Use this for initialization
    void Start ()
    {
		map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
		avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;
		camera_lowfps_local = GameObject.FindObjectOfType(typeof(camera_lowfps)) as camera_lowfps;
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
		float activelementangle;
		Vector3 targetdir;
		Vector3 cross;
		Vector3 ActiveElementZero;
		Vector3 AvatarElementZero;
		int i;

		yield return null;

		if (!avatarobject_local.avatarmoving)
		{
			for (i = 0; i < 50; i++)
			{
				avatarobject_local.RotateAvatarToWaypoint (ActiveElementObject.transform.position, 3);
				yield return null;
			}
			//avatarobject_local.RotateAvatarToWaypoint (ActiveElementObject.transform.position, -1);
		}
		/*
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
		*/

		/*avatarobject_local.camerafolowobject.transform.position = (ActiveElementZero + AvatarElementZero) / 2;
		ActiveElementZero = avatarobject_local.camerafolowobject.transform.position;
		ActiveElementZero.x = ActiveElementZero.x + 2f;
		avatarobject_local.camerafolowobject.transform.position = ActiveElementZero;
*/
		yield return new WaitForSeconds(0.5f);

		//avatarobject_local.avatarrotationobject.transform.localEulerAngles = targetdir;

		if (!cancelzoom) {
			avatarobject_local.avatarobject.GetComponent<Animator> ().SetTrigger ("chestwaiting");
			avatarobject_local.avatardetail = true;
			Debug.Log ("AvatarLowFPS");
		}
		avatarobject_local.RotateAvatarToWaypoint (ActiveElementObject.transform.position,-1);

		cancelzoom = false;

	}

	/*public void CancelZoom()
	{
		StopCoroutine(SetDetailAnim ());
		avatarobject_local.avatardetail = false;
		camera_lowfps_local.fpstime = 100;
		Debug.Log("CancelZoom");  
	}*/

	public void PressedButton()
	{
		Vector3 deltamousevector;

		deltamousevector = map_manager_local.initialmousepos - map_manager_local.newmousepos;

		if (deltamousevector.magnitude < map_manager_local._clickdistance)
		{

			//CHEST UNLOCKED
			map_manager_local.GUIDungeonMovement.GetComponent<Animator>().SetTrigger("PanelHide");

			map_manager_local.scrollmultiplier = 1;
			map_manager_local.avatarstatictime = 0.6f;
			map_manager_local.camerafadeouttime = 0.3f;
			map_manager_local.cameraspeed = 20;

			SetActiveElement (0);
			map_manager_local.canscrollmanually = false;

			avatarobject_local.FindPath(Xcoor, Ycoor);

			map_manager_local.mapcamera.GetComponent<Animator> ().SetTrigger ("smalldetail_in");
			map_manager_local.GUIChestOpenedPopup.GetComponent<Animator> ().SetTrigger ("PanelShow");
			avatarobject_local.avatarcamera.GetComponent<Animator> ().SetTrigger ("zoomin");

			StartCoroutine (SetDetailAnim ());

			avatarobject_local.SetHiMaterial (true);
			map_manager_local.GUIChestOpenedPopup.GetComponent<gui_chest_unlocked_popup> ().InitChestAppearance ();
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
