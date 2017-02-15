using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonUI : MonoBehaviour {

	public static DungeonUI Instance;

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
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		
	}

	public void showNextLevelMenu () {
		transform.GetChild (0).gameObject.SetActive(true);
	}

	public void hideNextLevelMenu () {
		transform.GetChild (0).gameObject.SetActive(false);
	}

}
