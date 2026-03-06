using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterScr : MonoBehaviour {
    public int count;
    public Sprite[] Numbs;
    private Image Img;

	// Use this for initialization
	void Start () {
        transform.parent.transform.localScale = new Vector3(1, 1, 1) * Screen.width * 0.001f;
        Img = GetComponent<Image>();
        Img.sprite = Numbs[count];
        if (count > 0) Img.enabled = true;
        else Img.enabled = false;


    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void AddFlag()
    {
        count++;
        if (count<11) Img.sprite = Numbs[count];
        Img.enabled = true;
    }

    public bool GetFlag()
    {
        if (count>0)
        {
            count--;
            Img.sprite = Numbs[count];
            return true;
        }
        else return false;
    }
}
