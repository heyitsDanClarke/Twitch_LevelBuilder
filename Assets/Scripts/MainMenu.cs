using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject settingsMenu;

	public float musicVolume;
	public float sfxVolume;

	void Start ()
    {
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
}
