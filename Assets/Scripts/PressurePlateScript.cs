using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressurePlateScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
			Player.Instance.boxes += 1;
			PlayerUI.Instance.transform.FindChild("Puzzle Bar").FindChild ("Value").GetComponent<Text>().text = Player.Instance.boxes.ToString();
        }
    }

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Box"))
		{
			//other.gameObject.GetComponentInChildren<MeshRenderer> ().material.SetFloat ("_Blend", 1.0f);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Box"))
		{
			//other.gameObject.GetComponentInChildren<MeshRenderer> ().material.SetFloat ("_Blend", 0.0f);
			Player.Instance.boxes -= 1;
			PlayerUI.Instance.transform.FindChild("Puzzle Bar").FindChild ("Value").GetComponent<Text>().text = Player.Instance.boxes.ToString();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
