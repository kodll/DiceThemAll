using UnityEngine;
using System.Collections;

public class aspectratio : MonoBehaviour {

	public GameObject pixelcamera;
	public GameObject maincanvas;
	RenderTexture myrendertexture;
	const int texturesize = 256;

	// Use this for initialization
	void Start ()
	{
		Vector2 rectsize;

		if ( pixelcamera.GetComponent<Camera>().targetTexture != null ) {
			pixelcamera.GetComponent<Camera>().targetTexture.Release( );
		}


		myrendertexture = new RenderTexture ( texturesize*Screen.width / Screen.height, texturesize, 24 );
		myrendertexture.useMipMap = false;
		myrendertexture.filterMode = FilterMode.Point;

		pixelcamera.GetComponent<Camera> ().targetTexture = myrendertexture;
		this.GetComponent<UnityEngine.UI.RawImage> ().texture = myrendertexture;

		rectsize.y = maincanvas.GetComponent<RectTransform> ().sizeDelta.y;
		rectsize.x = rectsize.y*Screen.width/Screen.height;
		maincanvas.GetComponent<RectTransform> ().sizeDelta = rectsize;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
