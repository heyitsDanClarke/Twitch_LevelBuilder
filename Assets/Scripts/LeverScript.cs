using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeverScript : MonoBehaviour {


	public bool isActive;

    public Sprite active;
    public Sprite inActive;
    public AudioClip leverSound;

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
		bool nextLevelMenuActive = false; // is the next level menu active in the scene
		bool pauseMenuActive = false; // is the pause menu active in the scene
		bool deathMenuActive = false; // is the death menu active in the scene
		try {
			nextLevelMenuActive = DungeonUI.Instance.transform.Find ("Next Level Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}
		try {
			pauseMenuActive = DungeonUI.Instance.transform.Find ("Pause Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}
		try {
			deathMenuActive = DungeonUI.Instance.transform.Find ("Death Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}

		// if player touches the block and the player is holding down the space button
		if (other.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space) && !nextLevelMenuActive && !pauseMenuActive && !deathMenuActive)
		{ 
			// activate/deactivate the switch
			isActive = !isActive;
			GetComponent<SpriteRenderer> ().color = isActive? new Color (0.3f, 0.3f, 1.0f): new Color (1.0f, 1.0f, 1.0f);
            GetComponent<SpriteRenderer>().sprite = isActive ? active : inActive;
            Player.Instance.levers += isActive? 1 : -1;
			PlayerUI.Instance.transform.FindChild("Puzzle Bar").FindChild ("Value").GetComponent<Text>().text = Player.Instance.levers.ToString();

            SoundController.instance.PlaySingle(leverSound);
        }
	}

}
