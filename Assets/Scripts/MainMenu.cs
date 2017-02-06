using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	//public GUIStyle largeButton; // GUI style for large buttons
	//public Texture backgroundTexture; // background of the menu

    public GameObject settingsMenu;

	public float musicVolume;
	public float sfxVolume;

	//private int state; // state of the UI

	// UI ID
	//private int menu = 0;
	//private int settings = 1;
	//private int credits = 2;

	void Start ()
    {
        /*
		largeButton.normal.textColor = new Color (0, 0, 0);		
		largeButton.alignment = TextAnchor.MiddleCenter;
		state = 0;*/
		musicVolume = 0.5f;
		sfxVolume = 0.5f;
	}
	

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenSettings() //todo: enable settings panel
    {

    }

    public void OpenCredits() //todo: enable credits panel
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //Gonna suggest we go with the Unity canvas system. Easier to manipulate in my opinion.
	/*void OnGUI() {
		GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		largeButton.fontSize = Screen.height / 20;


		if (state == menu) {
			if (GUI.Button (new Rect (Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.07f), "Start", largeButton)) {
				Application.LoadLevel ("DungeonScene");
			}

			if (GUI.Button (new Rect (Screen.width * 0.25f, Screen.height * 0.4f, Screen.width * 0.5f, Screen.height * 0.07f), "Settings", largeButton)) {
				state = settings;
			}

			if (GUI.Button (new Rect (Screen.width * 0.25f, Screen.height * 0.55f, Screen.width * 0.5f, Screen.height * 0.07f), "Credits", largeButton)) {
				state = credits;
			}

			if (GUI.Button (new Rect (Screen.width * 0.25f, Screen.height * 0.7f, Screen.width * 0.5f, Screen.height * 0.07f), "Quit", largeButton)) {
				Application.Quit ();
			}

		} else if (state == settings) {
			GUI.HorizontalSlider(new Rect (Screen.width * 0.25f, Screen.height * 0.25f, Screen.height * 0.5f, Screen.height * 0.07f), musicVolume, 0.0f, 1.0f);
			GUI.HorizontalSlider(new Rect (Screen.width * 0.25f, Screen.height * 0.4f, Screen.height * 0.5f, Screen.height * 0.07f), sfxVolume, 0.0f, 1.0f);
			GameMaster.Instance.music = musicVolume;
			GameMaster.Instance.sfx = sfxVolume;

			if (GUI.Button (new Rect (Screen.width * 0.25f, Screen.height * 0.7f, Screen.width * 0.5f, Screen.height * 0.07f), "Back", largeButton)) {
				state = menu;
			}
		} else if (state == credits) {
			GUI.TextField (new Rect (Screen.width * 0.25f, Screen.height * 0.25f, Screen.height * 0.5f, Screen.height * 0.2f), "CREDITS TEXT");
			if (GUI.Button (new Rect (Screen.width * 0.25f, Screen.height * 0.7f, Screen.width * 0.5f, Screen.height * 0.07f), "Back", largeButton)) {
				state = menu;
			}
		}
	}*/
}
