using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeverScript : MonoBehaviour {


	public bool isActive;


	// Use this for initialization
	void Start () {
		isActive = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay2D(Collider2D other) 
	{
		OnTriggerEnter2D(other);
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		bool PauseMenuActive = false; // is there any menus active in the scene

		try {
			PauseMenuActive = DungeonUI.Instance.transform.Find ("Pause Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}

		// if player touches the block and the player is holding down the space button
		if (other.gameObject.CompareTag("Player") && !PauseMenuActive && Input.GetKeyDown(KeyCode.Space) )
		{ 
			// activate/deactivate the switch
			isActive = !isActive;
			GetComponent<SpriteRenderer> ().color = isActive? new Color (0.3f, 0.3f, 1.0f): new Color (1.0f, 1.0f, 1.0f);
			Player.Instance.levers += isActive? 1 : -1;
			PlayerUI.Instance.transform.FindChild("Puzzle Bar").FindChild ("Value").GetComponent<Text>().text = Player.Instance.levers.ToString();

		}
	}

}
