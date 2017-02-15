using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	}

	public void hideNextLevelMenu () {
		nextLevelMenuActive = false;
		transform.GetChild (0).gameObject.SetActive(false);
	}

	public void showPauselMenu () {
		transform.GetChild (1).gameObject.SetActive(true);
	}

	public void hidePauseMenu () {
		transform.GetChild (1).gameObject.SetActive(false);
	}

	public void quitDungeon () {
		SceneManager.LoadScene(0);
	}

}
