using UnityEngine;
using System.Collections;

public class dice_machine : MonoBehaviour {
	public GameObject diceanimation;
	public GameObject diceanimateddummy;
	public GameObject diceobject;
	public GameObject dicelookatrotation;
	public GameObject dice2dimageobject;
	public GameObject pixelcamera;
	public RenderTexture myrendertexture;
	const int texturesize = 42;

	const float cuttedangle = 11.25f;



	public void DestroyDice()
	{
		Destroy (myrendertexture);
		Destroy (this.gameObject);
	}
	// Use this for initialization
	void Start ()
	{
		Vector2 rectsize;

		if ( pixelcamera.GetComponent<Camera>().targetTexture != null ) {
			pixelcamera.GetComponent<Camera>().targetTexture.Release( );
		}


		myrendertexture = new RenderTexture ( texturesize, texturesize, 24 );
		myrendertexture.useMipMap = false;
		myrendertexture.filterMode = FilterMode.Point;

		pixelcamera.GetComponent<Camera> ().targetTexture = myrendertexture;
		dice2dimageobject.GetComponent<UnityEngine.UI.RawImage> ().texture = myrendertexture;

	}

	// Update is called once per frame
	void Update ()
	{
		Vector3 vect;

		dice2dimageobject.transform.position = diceanimateddummy.transform.position;
		dice2dimageobject.transform.localPosition = dice2dimageobject.transform.localPosition*5;
		diceobject.transform.rotation = diceanimateddummy.transform.rotation;

		vect = diceobject.transform.eulerAngles;
		vect.x = Mathf.Round (vect.x / cuttedangle) * cuttedangle;
		vect.y = Mathf.Round (vect.y / cuttedangle) * cuttedangle;
		vect.z = Mathf.Round (vect.z / cuttedangle) * cuttedangle;
		diceobject.transform.eulerAngles = vect;

		vect.x = 1-(dice2dimageobject.transform.localPosition.z + 2)/20;
		vect.y = vect.x;
		vect.z = vect.x;
		dice2dimageobject.transform.localScale = vect;
	}
}
