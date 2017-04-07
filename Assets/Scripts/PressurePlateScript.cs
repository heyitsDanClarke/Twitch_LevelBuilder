using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressurePlateScript : MonoBehaviour {
    public AudioClip boxHitSound;
	public Sprite inactivePlate;
	public Sprite activePlate;

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            SoundController.Instance.PlaySingle(boxHitSound);
            Player.Instance.boxes += 1;
			PlayerUI.Instance.transform.FindChild("Puzzle Bar").FindChild ("Value").GetComponent<Text>().text = Player.Instance.boxes.ToString();
        }
    }

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Box"))
		{
			GetComponent<SpriteRenderer> ().sprite = activePlate;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Box"))
		{
			GetComponent<SpriteRenderer> ().sprite = inactivePlate;
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
