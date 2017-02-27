using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonUI : MonoBehaviour {

	public static DungeonUI Instance;

	private bool nextLevelMenuActive; // boolean value stating whether the next level menu is active or not
	private bool pauseMenuActive; // boolean value stating whether the pause menu is active or not
	private GameObject nextLevelMenu;

	// Use this for initialization
	void Awake () {
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		nextLevelMenuActive = false;
		pauseMenuActive = false;
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && !nextLevelMenuActive) { // if user presses the ESC key and next level menu is not active
			pauseMenuActive = !pauseMenuActive;
			if (pauseMenuActive) {
				showPauselMenu ();
			} else {
				hidePauseMenu ();
			}
		}
	}

	void FixedUpdate () {
		
	}

	public void showNextLevelMenu () {
		nextLevelMenuActive = true;
		transform.FindChild("Next Level Menu").gameObject.SetActive(true);

		// display alternative text if player
		if (Dungeon.Instance.roomsLeftUntilBoss < 0) {
			transform.FindChild("Next Level Menu").FindChild("Continue Button").GetChild(0).GetComponent<Text>().text = "Main Menu";
		}
	}

	public void hideNextLevelMenu () {
		nextLevelMenuActive = false;
		transform.FindChild("Next Level Menu").gameObject.SetActive(false);
	}

	public void showPauselMenu () {
		transform.FindChild("Pause Menu").gameObject.SetActive(true);

		// pause moving entities
		foreach (GameObject gameObjectInScene in FindObjectsOfType(typeof(GameObject))) {
			try {
				GameObject savedVelocity = gameObjectInScene.transform.Find ("Saved Velocity").gameObject;
				if (savedVelocity != null) { // if the "Saved Velocity" is a child of the game object
					Rigidbody2D rb = gameObjectInScene.GetComponent<Rigidbody2D>(); // rigidbody of entity
					savedVelocity.GetComponent<Text> ().text = rb.velocity.x.ToString () + "," + rb.velocity.y.ToString (); // save current velocity of entity
					rb.velocity = new Vector2 (0, 0); // pause entity
				}
			} catch (NullReferenceException) {}
		}
	}

	public void hidePauseMenu () {
		transform.FindChild("Pause Menu").gameObject.SetActive(false);

		// resume moving entities
		foreach (GameObject gameObjectInScene in FindObjectsOfType(typeof(GameObject))) {
			try {
				GameObject savedVelocity = gameObjectInScene.transform.Find ("Saved Velocity").gameObject;
				if (savedVelocity != null) { // if the "Saved Velocity" is a child of the game object
					Rigidbody2D rb = gameObjectInScene.GetComponent<Rigidbody2D>(); // rigidbody of entity
					string[] savedVelocityArray = savedVelocity.GetComponent<Text> ().text.Split (',');
					rb.velocity = new Vector2(float.Parse(savedVelocityArray[0]), float.Parse(savedVelocityArray[1])); // restore velocity of entity
					savedVelocity.GetComponent<Text> ().text = "0,0"; // reset saved velocity
				}
			} catch (NullReferenceException) {}
		}
	}

	public void quitDungeon () {
		SceneManager.LoadScene(0);
	}

}
