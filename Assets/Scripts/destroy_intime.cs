using UnityEngine;
using System.Collections;

public class destroy_intime : MonoBehaviour {

    public float wait;

	// Use this for initialization
	void Start ()
    {
        Destroy(gameObject, wait);
    }
}
