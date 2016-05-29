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
	}
	const float betweendiceswait = 0.15f;

	DiceSlot[] DicesField;

	// Use this for initialization
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
					DicesField [i].Dice.transform.RotateAround (DicesField [i].Dice.transform.position, Vector3.up, 100 * Time.deltaTime);
			}
			else if (DicesField [i].Dice != null)
			{
					rot = DicesField [i].Dice.transform.localEulerAngles;

					DicesField [i].Dice.transform.localEulerAngles = rot/2;
			}
		}
	}
	public void RollDice(int index)
	{
		int randomrollednumber;
		int randomvariant;
		randomrollednumber = Random.Range (1, 6);
		randomvariant = Random.Range (1, 3);
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


	public void InitDicesToRoll (bool init)
	{
		int i;
		Vector3 RandomRot;
		Vector3 Offset;

		Offset = Vector3.zero;
		Offset.z = -100;

		if (init) //INIT
		{
			for (i = 0; i < 6; i++) {
				DicesField [i].Slot = Instantiate (SlotPrefabObject, Vector3.zero, Quaternion.identity) as GameObject;
				DicesField [i].Slot.transform.SetParent (LayoutObject.transform);
				DicesField [i].Slot.transform.localPosition = Vector3.zero;
				DicesField [i].Slot.transform.localScale = Vector3.one;

				RandomRot.x = Random.Range(1,255);
				RandomRot.y = Random.Range(1,255);
				RandomRot.z = Random.Range(1,255);

				DicesField [i].Dice = Instantiate (map_manager_local.DiceObject, Vector3.zero, Quaternion.identity) as GameObject;
				DicesField [i].Dice.transform.SetParent (DicesField [i].Slot.transform);
				DicesField [i].Dice.transform.localPosition = Offset;
				DicesField [i].Dice.transform.localScale = map_manager_local.DiceObject.transform.localScale;
				DicesField [i].Dice.transform.Rotate (RandomRot.x, RandomRot.y, RandomRot.z);
				DicesField [i].dicesrolled = false;
			}

		}
		else //DEINIT
		{
			for (i = 0; i < 6; i++)
			{
				if (DicesField [i].Slot != null) Destroy (DicesField [i].Slot);
				if (DicesField [i].Dice != null) Destroy (DicesField [i].Dice);
			}
				
		}	
	}
}
