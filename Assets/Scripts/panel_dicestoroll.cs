using UnityEngine;
using System.Collections;

public class panel_dicestoroll : MonoBehaviour
{
	public GameObject SlotPrefabObject;
	public GameObject LayoutObject;
	static map_manager map_manager_local;


	public struct DiceSlot
	{
		public GameObject Slot;
		public GameObject Dice;
		public bool dicesrolled;
		public int rollednumber;
		public dice_machine dice_machine_local;
	}
	const float betweendiceswait = 0.15f;

	DiceSlot[] DicesField;


	public void InitDicesToRoll (bool init)
	{
		int i;
		Vector3 RandomRot;
		Vector3 Offset;
		Vector3 vect;

		Offset = Vector3.zero;
		Offset.z = -100;

		if (init) //INIT
		{
			for (i = 0; i < 6; i++)
			{
				DicesField [i].Slot = Instantiate (SlotPrefabObject, Vector3.zero, Quaternion.identity) as GameObject;

				DicesField [i].Slot.transform.SetParent (LayoutObject.transform);
				DicesField [i].Slot.transform.localPosition = Vector3.zero;
				DicesField [i].Slot.transform.localScale = Vector3.one;

				RandomRot.x = Random.Range(1,255);
				RandomRot.y = Random.Range(1,255);
				RandomRot.z = Random.Range(1,255);

				DicesField [i].Dice = Instantiate (map_manager_local.DiceObject, Vector3.zero, Quaternion.identity) as GameObject;
				DicesField [i].dice_machine_local = DicesField [i].Dice.GetComponent("dice_machine") as dice_machine;

				DicesField [i].Dice.transform.SetParent (DicesField [i].Slot.transform);
				DicesField [i].Dice.transform.localPosition = Offset;
				DicesField [i].Dice.transform.localScale = map_manager_local.DiceObject.transform.localScale;
				DicesField [i].dice_machine_local.dicelookatrotation.transform.Rotate (RandomRot.x, RandomRot.y, RandomRot.z);

				vect = DicesField [i].dice_machine_local.dicelookatrotation.transform.eulerAngles;
				vect.x = Mathf.Round (vect.x / 90) * 90f;
				vect.y = Mathf.Round (vect.y / 90) * 90f;
				vect.z = Mathf.Round (vect.z / 90) * 90f;
				DicesField [i].dice_machine_local.dicelookatrotation.transform.eulerAngles = vect;

				DicesField [i].dicesrolled = false;
				DicesField [i].rollednumber = -1;


			}

		}
		else //DEINIT
		{
			for (i = 0; i < 6; i++)
			{
				if (DicesField [i].Dice != null) DicesField [i].dice_machine_local.DestroyDice ();
				if (DicesField [i].Slot != null) Destroy (DicesField [i].Slot);

			}

		}	
	}


	void Start ()
	{
		int i;
		map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;

		DicesField = new DiceSlot[6];

		for (i = 0; i < 6; i++)
		{
			DicesField[i].Slot = null;
			DicesField[i].Dice = null;
			DicesField[i].dicesrolled = false;
			DicesField[i].rollednumber = -1;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		int i;
		Vector3 rot;

		for (i = 0; i < 6; i++)
		{
			if (DicesField [i].Dice != null && !DicesField [i].dicesrolled) 
			{
				DicesField [i].dice_machine_local.dicelookatrotation.transform.RotateAround (DicesField [i].Dice.transform.position, Vector3.up, 100 * Time.deltaTime);
			}
			else if (DicesField [i].Dice != null)
			{
				rot = DicesField [i].dice_machine_local.dicelookatrotation.transform.localEulerAngles;

				DicesField [i].dice_machine_local.dicelookatrotation.transform.localEulerAngles = rot/2;
			}
		}

	}
	public void RollDice(int index)
	{
		int randomrollednumber;
		int randomvariant;
		randomrollednumber = Random.Range (1, 7);
		randomvariant = Random.Range (1, 4);
		randomvariant = 1;

		DicesField [index].dicesrolled = true;
		DicesField [index].rollednumber = randomrollednumber;
		DicesField [index].Dice.GetComponent<dice_machine> ().diceanimation.GetComponent<Animator> ().SetTrigger ("roll"+randomrollednumber+randomvariant);
	}

	IEnumerator ThrowAllDicesSequence()
	{
		int i;
		Vector3 force, pos;
		for (i = 0; i < 6; i++)
		{
			RollDice (i);
			yield return new WaitForSeconds(betweendiceswait);
		}
	}
	public void RollAllDices()
	{
		StartCoroutine (ThrowAllDicesSequence ());
	}



}
