using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {

	private GUIStyle guiStyle;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {

		if (GUI.Button (new Rect (Screen.width * 0.3f, Screen.height * 0.1f, Screen.width * 0.4f, Screen.height * 0.1f), "Start")) {
			Application.LoadLevel ("DungeonScene");
		}

		if (GUI.Button (new Rect (Screen.width * 0.3f, Screen.height * 0.25f, Screen.width * 0.4f, Screen.height * 0.1f), "Settings")) {
		}

		if (GUI.Button (new Rect (Screen.width * 0.3f, Screen.height * 0.4f, Screen.width * 0.4f, Screen.height * 0.1f), "Credits")) {
		}

		if (GUI.Button (new Rect (Screen.width * 0.3f, Screen.height * 0.55f, Screen.width * 0.4f, Screen.height * 0.1f), "Quit")) {
		}
	}
}
