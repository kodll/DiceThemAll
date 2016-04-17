using UnityEngine;
using System.Collections;

public class panel_dicestoroll : MonoBehaviour
{
	public GameObject SlotPrefabObject;
	public GameObject LayoutObject;
	static map_manager map_manager_local;
	bool dicesrolled = false;

	public struct DiceSlot
	{
		public GameObject Slot;
		public GameObject Dice;
	}

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
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		int i;
		if (!dicesrolled)
		{
			for (i = 0; i < 6; i++) {
				if (DicesField[i].Dice!=null) DicesField [i].Dice.transform.RotateAround (DicesField[i].Dice.transform.position, Vector3.up, 100 * Time.deltaTime);
			}
		}
	}

	public void RollDices()
	{
		int i;
		Vector3 force, pos;
		dicesrolled = true;
		for (i = 0; i < 6; i++)
		{
			force.x = Random.Range(1,100)*1000000-50000000;
			force.y = Random.Range(1,100)*1000000-50000000;
			force.z = Random.Range(1,100)*1000000-50000000;

			pos.x = Random.Range(0,50)*100-2500;
			pos.y = Random.Range(0,50)*100-2500;
			pos.z = Random.Range(0,50)*100-2500;

			DicesField [i].Dice.GetComponent<Rigidbody> ().isKinematic = false;
			DicesField [i].Dice.GetComponent<Rigidbody> ().useGravity = true;
			DicesField [i].Dice.GetComponent<Rigidbody> ().AddForceAtPosition(force/50, pos);
			//DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.left*force.x);
			//DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up*force.y);
			//DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward*force.z);
			DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(force*1000000);
			DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(force*1000000);
			DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(force*1000000);
			DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(force*1000000);
			DicesField [i].Dice.GetComponent<Rigidbody>().AddRelativeTorque(force*1000000);
		}

		
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
			}
			dicesrolled = false;
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
