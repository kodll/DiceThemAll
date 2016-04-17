using UnityEngine;
using System.Collections;

public class gui_chest_unlocked_popup : MonoBehaviour {

	[HideInInspector] public GameObject ActiveElementObject;
	[HideInInspector] public GameObject AvatarInfrontOfChestObject;
	static avatarstatemachine avatarobject_local;
	static map_manager map_manager_local;
	[HideInInspector] public bool destroyavatar = false;
	static float mytime = 0;
	[HideInInspector] public float wait;

	public GameObject LootLayoutObject;
	public GameObject PrefabLootItemObject;
	public GameObject PanelDicesToRollPlaceholderObject;
	GameObject PanelToRollDicesObject;
	GameObject[] LootItemObjectField;
	GameObject[] DiceRewardField;

	// Use this for initialization
	void Start ()
	{
		
	}

	public void InitGuiChestSystem()
	{
		int i;

		map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
		avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;

		LootItemObjectField = new GameObject[6];
		DiceRewardField = new GameObject[6];
		for (i = 0; i < 6; i++)
		{
			LootItemObjectField[i] = Instantiate (PrefabLootItemObject, Vector3.zero, Quaternion.identity) as GameObject;
			LootItemObjectField[i].transform.SetParent(LootLayoutObject.transform);
			LootItemObjectField[i].transform.localScale = PrefabLootItemObject.transform.localScale;
			LootItemObjectField[i].SetActive (false);

			DiceRewardField[i] = Instantiate (map_manager_local.DiceObject, Vector3.zero, Quaternion.identity) as GameObject;
			DiceRewardField[i].transform.SetParent(LootItemObjectField[i].GetComponent<loot_item_def>().DiceSlot.transform);
			DiceRewardField[i].transform.localPosition = Vector3.zero;
			DiceRewardField[i].transform.localScale = map_manager_local.DiceObject.transform.localScale;
			DiceRewardField[i].transform.Rotate(map_manager_local.DiceNumberRotation[i].x,map_manager_local.DiceNumberRotation[i].y,map_manager_local.DiceNumberRotation[i].z);

		}

		PanelToRollDicesObject = Instantiate (map_manager_local.PrefabPanelDicesToRollObject, Vector3.zero, Quaternion.identity) as GameObject;
		PanelToRollDicesObject.transform.SetParent(PanelDicesToRollPlaceholderObject.transform);
		PanelToRollDicesObject.transform.localRotation = Quaternion.identity;
		PanelToRollDicesObject.transform.localScale = Vector3.one;
		PanelToRollDicesObject.transform.localPosition = Vector3.zero;

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

	public void InitChestAppearance()
	{
		mytime = 0;
		wait = 0.25f;
		destroyavatar = false;

		StartCoroutine (ShowPossibleReward ());
	}

	void HidePossibleReward()
	{
		int i;
		for (i = 0; i < 6; i++)
		{
			LootItemObjectField [i].SetActive (false);

		}

	}

	IEnumerator ShowPossibleReward()
	{
		int i;

		HidePossibleReward();

		yield return new WaitForSeconds(0.1f);
		PanelToRollDicesObject.GetComponent<panel_dicestoroll> ().InitDicesToRoll (true);

		for (i = 0; i < 6; i++)
		{
			LootItemObjectField [i].SetActive (true);

			yield return new WaitForSeconds(0.05f);
		}
	}

	IEnumerator ThrowDices()
	{
		PanelToRollDicesObject.GetComponent<panel_dicestoroll> ().RollDices();
		yield return new WaitForSeconds(4.0f);
		this.GetComponent<Animator> ().SetTrigger ("PanelHide");
		ActiveElementObject.GetComponent<map_piece_def> ().SetActiveElement (3);
		avatarobject_local.avatarobject.GetComponent<Animator> ().SetTrigger ("show");
		map_manager_local.charactercamera.GetComponent<Animator> ().SetTrigger ("smalldetail_out");
		destroyavatar = true;
		yield return new WaitForSeconds(0.5f);

		map_manager_local.GUIDungeonMovement.GetComponent<Animator> ().SetTrigger ("PanelShow");
		map_manager_local.TriggerScrolling (true);

		PanelToRollDicesObject.GetComponent<panel_dicestoroll> ().InitDicesToRoll (false);

	}

	public void OpenChest (bool open)
	{
		if (ActiveElementObject != null)
		{
			if (open)
			{
				StartCoroutine (ThrowDices ());
			}
			else
			{
				ActiveElementObject.GetComponent<map_piece_def> ().SetActiveElement (1);
				avatarobject_local.avatarobject.GetComponent<Animator> ().SetTrigger ("show");
				destroyavatar = true;

				PanelToRollDicesObject.GetComponent<panel_dicestoroll> ().InitDicesToRoll (false);

			}

		}
		else
		{
			Debug.Log("FATAL!!!! CHest is not initialized!!!");  
		}	
	}
		
}
