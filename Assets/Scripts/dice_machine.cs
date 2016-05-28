using UnityEngine;
using System.Collections;

public class dice_machine : MonoBehaviour {
	public GameObject diceanimation;
	public GameObject diceanimateddummy;
	public GameObject diceobject;
	const float cuttedangle = 11.25f;


	// Use this for initialization
	void Start ()
	{
		

	}

	// Update is called once per frame
	void Update ()
	{
		Vector3 vect;

		diceobject.transform.position = diceanimateddummy.transform.position;
		diceobject.transform.localPosition = diceobject.transform.localPosition*9;
		diceobject.transform.rotation = diceanimateddummy.transform.rotation;
		vect = diceobject.transform.eulerAngles;
		vect.x = Mathf.Round (vect.x / cuttedangle) * cuttedangle;
		vect.y = Mathf.Round (vect.y / cuttedangle) * cuttedangle;
		vect.z = Mathf.Round (vect.z / cuttedangle) * cuttedangle;
		diceobject.transform.eulerAngles = vect;

		vect.x = (diceobject.transform.localPosition.y + 2)/30+1;
		vect.y = vect.x;
		vect.z = vect.x;
		diceobject.transform.localScale = vect;
	}
}
