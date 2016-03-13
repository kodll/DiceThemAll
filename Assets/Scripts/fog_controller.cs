using UnityEngine;
using System.Collections;

public class fog_controller : MonoBehaviour {

    public GameObject ObjectSubfog;
    bool changedcolor = false;
    float deltatime = 0;
    float actualtime = 0;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (changedcolor && actualtime >= deltatime)
        {
            Color newmaincolor, newsubcolor;
            newmaincolor = this.gameObject.GetComponent<SpriteRenderer>().color;
            newsubcolor = ObjectSubfog.gameObject.GetComponent<SpriteRenderer>().color;
            newmaincolor.a = newmaincolor.a - Time.deltaTime;
            if (newmaincolor.a < 0) newmaincolor.a = 0;
            newsubcolor.a = newmaincolor.a;

            this.gameObject.GetComponent<SpriteRenderer>().color = newmaincolor;
            ObjectSubfog.gameObject.GetComponent<SpriteRenderer>().color = newsubcolor;
        }
        if (changedcolor)
        {
            actualtime = actualtime + Time.deltaTime;
            if (actualtime > deltatime) actualtime = deltatime;
        }
	}

    public void SetRoomAlpha(float intime)
    {
        Color newmaincolor;
        newmaincolor = this.gameObject.GetComponent<SpriteRenderer>().color;
        if (newmaincolor.a > 0 && deltatime == 0)
        {
            changedcolor = true;
            deltatime = intime;
            actualtime = 0;
        }
        

        
    }
}
