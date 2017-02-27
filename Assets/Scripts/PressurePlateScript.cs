using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
			
        }
    }

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Box"))
		{
			other.gameObject.GetComponent<SpriteRenderer> ().color = new Color (1, 0.4f, 0, 0.5f);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Box"))
		{
			other.gameObject.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.5f);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
