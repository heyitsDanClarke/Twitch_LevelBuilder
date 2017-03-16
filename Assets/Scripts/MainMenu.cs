using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject settingsMenu;

	void Start ()
    {
		// import variables from GameMaster
		transform.FindChild ("Settings Panel").FindChild ("Music Volume Slider").GetComponent<Slider> ().value = GameMaster.Instance.music;
		transform.FindChild("Settings Panel").FindChild("SFX Volume Slider").GetComponent<Slider>().value = GameMaster.Instance.sfx;
		transform.FindChild("Settings Panel").FindChild("Twitch Username InputField").GetComponent<InputField>().text = GameMaster.Instance.username;
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) { // if user presses the ESC key
			CloseAllPanels();
		}

		// update variables of GameMaster
		GameMaster.Instance.music = transform.FindChild ("Settings Panel").FindChild ("Music Volume Slider").GetComponent<Slider> ().value;
		GameMaster.Instance.sfx = transform.FindChild("Settings Panel").FindChild("SFX Volume Slider").GetComponent<Slider>().value;
		GameMaster.Instance.username = transform.FindChild("Settings Panel").FindChild("Twitch Username InputField").GetComponent<InputField>().text;
	}

    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void OpenSettingsPanel() {
		transform.FindChild("Settings Panel").gameObject.SetActive(true);
    }

	public void OpenCreditsPanel() {
		transform.FindChild("Credits Panel").gameObject.SetActive(true);
    }

	public void OpenQuitPanel()	{
		transform.FindChild("Quit Panel").gameObject.SetActive(true);
	}

	public void CloseAllPanels() {
		transform.FindChild("Settings Panel").gameObject.SetActive(false);
		transform.FindChild("Credits Panel").gameObject.SetActive(false);
		transform.FindChild("Quit Panel").gameObject.SetActive(false);
	}

    public void QuitGame()
    {
        Application.Quit();
    }
}
