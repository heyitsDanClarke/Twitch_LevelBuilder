using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonUI : MonoBehaviour {

	public static DungeonUI Instance;

	[HideInInspector] public bool nextLevelMenuActive; // boolean value stating whether the next level menu is active or not
	[HideInInspector] public bool pauseMenuActive; // boolean value stating whether the pause menu is active or not
	[HideInInspector] public bool settingsMenuActive; // boolean value stating whether the settings menu is active or not
	[HideInInspector] public bool deathMenuActive; // boolean value stating whether the death menu is active or not

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
		settingsMenuActive = false;
		deathMenuActive = false;
	}

	void Start() {
		// import variables from GameMaster
		transform.FindChild ("Settings Menu").FindChild ("Music Volume Slider").GetComponent<Slider> ().value = GameMaster.Instance.music;
		transform.FindChild("Settings Menu").FindChild("SFX Volume Slider").GetComponent<Slider>().value = GameMaster.Instance.sfx;

	}

	void Update () {
		// update variables of GameMaster
		GameMaster.Instance.music = transform.FindChild ("Settings Menu").FindChild ("Music Volume Slider").GetComponent<Slider> ().value;
		SoundController.instance.musicSource.volume = GameMaster.Instance.music;
		GameMaster.Instance.sfx = transform.FindChild("Settings Menu").FindChild("SFX Volume Slider").GetComponent<Slider>().value;
		SoundController.instance.efxSource.volume = GameMaster.Instance.sfx;

		if (Input.GetKeyDown (KeyCode.Escape) && settingsMenuActive) { // if user presses the ESC key and the settings menu is active
			HideSettingsMenu();
		} else if (Input.GetKeyDown (KeyCode.Escape) && !nextLevelMenuActive && !deathMenuActive) { // if user presses the ESC key and the next level menu is not active
			pauseMenuActive = !pauseMenuActive;
			if (pauseMenuActive) {
				showPauselMenu ();
			} else {
				hidePauseMenu ();
			}
		}

		if (Player.Instance.health <= 0 && !nextLevelMenuActive && !pauseMenuActive) {
			ShowDeathMenu ();
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

	public void ShowSettingsMenu() {
		settingsMenuActive = true;
		transform.FindChild("Settings Menu").gameObject.SetActive(true);
	}

	public void HideSettingsMenu() {
		settingsMenuActive = false;
		transform.FindChild("Settings Menu").gameObject.SetActive(false);
	}

	public void ShowDeathMenu() {
		// play sounds once
		if (!deathMenuActive) {
			SoundController.instance.musicSource.mute = true;
			SoundController.instance.deathMusicSource.mute = false;
			SoundController.instance.deathMusicSource.Play ();
		}

		deathMenuActive = true;
		transform.FindChild("Death Menu").gameObject.SetActive(true);

		Player.Instance.gameObject.SetActive (false); // hide player

	}

	public void HideDeathMenu() {
		// unmute sounds once
		if (deathMenuActive) {
			SoundController.instance.deathMusicSource.mute = true;
			SoundController.instance.musicSource.mute = false;
		}

		deathMenuActive = false;
		transform.FindChild("Death Menu").gameObject.SetActive(false);

		Player.Instance.gameObject.SetActive (true); // show player
	}

	public void SetPauseMenuActiveToFalse() {
		DungeonUI.Instance.pauseMenuActive = false;
	}

	public void quitDungeon () {
		SceneManager.LoadScene(0);
	}

}
