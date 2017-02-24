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
	void Start () {
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
		transform.GetChild (0).gameObject.SetActive(true);

		// display alternative text if player
		if (Dungeon.Instance.roomsLeftUntilBoss < 0) {
			transform.GetChild (0).GetChild(1).GetChild(0).GetComponent<Text>().text = "Main Menu";
		}
	}

	public void hideNextLevelMenu () {
		nextLevelMenuActive = false;
		transform.GetChild (0).gameObject.SetActive(false);
	}

	public void showPauselMenu () {
		transform.GetChild (1).gameObject.SetActive(true);

		// pause moving entities
		foreach (GameObject gameObjectInScene in FindObjectsOfType(typeof(GameObject))) {
			if (gameObjectInScene.layer == 9) { // if the game object is an entity
				GameObject savedVelocity = gameObjectInScene.transform.Find ("Saved Velocity").gameObject;
				if (savedVelocity != null) { // if the "Saved Velocity" is a child of the game object
					Rigidbody2D rb = gameObjectInScene.GetComponent<Rigidbody2D>(); // rigidbody of entity
					savedVelocity.GetComponent<Text> ().text = rb.velocity.x.ToString () + "," + rb.velocity.y.ToString (); // save current velocity of entity
					rb.velocity = new Vector2 (0, 0); // pause entity
				}
			}
		}
	}

	public void hidePauseMenu () {
		transform.GetChild (1).gameObject.SetActive(false);

		// resume moving entities
		foreach (GameObject gameObjectInScene in FindObjectsOfType(typeof(GameObject))) {
			if (gameObjectInScene.layer == 9) { // if the game object is an entity
				GameObject savedVelocity = gameObjectInScene.transform.Find ("Saved Velocity").gameObject;
				if (savedVelocity != null) { // if the "Saved Velocity" is a child of the game object
					Rigidbody2D rb = gameObjectInScene.GetComponent<Rigidbody2D>(); // rigidbody of entity
					string[] savedVelocityArray = savedVelocity.GetComponent<Text> ().text.Split (',');
					rb.velocity = new Vector2(float.Parse(savedVelocityArray[0]), float.Parse(savedVelocityArray[1])); // restore velocity of entity
					savedVelocity.GetComponent<Text> ().text = "0,0"; // reset saved velocity
				}
			}
		}
	}

	public void quitDungeon () {
		SceneManager.LoadScene(0);
	}

}
