using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTimerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//transform.FindChild ("Value").GetComponent<Text> ().text = string.Format ("{0:F1}", 0.0f);
		transform.FindChild ("Max Value").GetComponent<Text> ().text = string.Format ("{0:F1}", 20.0f);
		transform.FindChild("Pie").GetComponent<Image> ().fillAmount = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		float value = float.Parse(transform.FindChild ("Real Value").GetComponent<Text> ().text); // value of the pie chart
		float maxValue = float.Parse(transform.FindChild ("Max Value").GetComponent<Text> ().text); // max value of the pie chart

		// show timer if there is a countdown, else hide it
		if (value > 0.0f) {
			gameObject.SetActive(true);
		} else {
			if (gameObject.activeSelf) { // reset current weapon on GM
				Player.Instance.currentWeapon = Player.Instance.sword;
                Player.Instance.UpdateAnimator();
			}

			gameObject.SetActive(false);
		}

		bool PauseMenuActive = false; // is there any menus active in the scene
		try {
			PauseMenuActive = DungeonUI.Instance.transform.Find ("Pause Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}

		// update pie chart
		if (!PauseMenuActive) {
			transform.FindChild ("Real Value").GetComponent<Text> ().text = Mathf.Max (value - Time.deltaTime, 0.0f).ToString();
			value = float.Parse (transform.FindChild ("Real Value").GetComponent<Text> ().text);
		}
		transform.FindChild("Pie").GetComponent<Image> ().fillAmount = Mathf.Clamp01(value / maxValue);

	}
}
