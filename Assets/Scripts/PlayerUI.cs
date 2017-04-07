using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public static PlayerUI Instance;

    Text healthValue;
	Text iceValue;
	Text fireValue;
    Text chargesValue;
	Text fireResistanceValue;
	Text bossHealthValue;

	public Sprite boxIcon;
	public Sprite leverIcon;

	public float nextWeaponPanelCountdown;

    void Awake() {

		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		healthValue = transform.FindChild("Health Bar").FindChild("Value").GetComponent<Text>();
		chargesValue = transform.FindChild("Charges Bar").FindChild("Value").GetComponent<Text>();
		fireResistanceValue = transform.FindChild("Fire Resistance Bar").FindChild("Value").GetComponent<Text>();
		iceValue = transform.FindChild("Ice Power Panel").FindChild("Value").GetComponent<Text>();
		fireValue = transform.FindChild("Fire Power Panel").FindChild("Value").GetComponent<Text>();
		bossHealthValue = transform.FindChild("Boss Bar").FindChild("Value").GetComponent<Text>();
    }

	void Update() {
		transform.FindChild ("Health Bar").FindChild("Max Value").GetComponent<Text>().text = Player.Instance.maxHealth.ToString ();
		transform.FindChild ("Charges Bar").FindChild("Max Value").GetComponent<Text>().text = Player.Instance.maxCharges.ToString ();
		transform.FindChild ("Fire Resistance Bar").FindChild ("Max Value").GetComponent<Text> ().text = Player.Instance.maxFireResistance.ToString ();
		try {
			transform.FindChild ("Boss Bar").FindChild ("Max Value").GetComponent<Text> ().text = GameObject.FindGameObjectWithTag ("Boss").GetComponent<Cerberus> ().maxHealth.ToString ();
			PlayerUI.Instance.transform.FindChild ("Boss Bar").gameObject.SetActive (true);
		} catch (NullReferenceException) {
			PlayerUI.Instance.transform.FindChild ("Boss Bar").gameObject.SetActive (false);
		}
		healthValue.text = Player.Instance.health.ToString ();
		chargesValue.text = Player.Instance.charges.ToString ();
		fireResistanceValue.text = Player.Instance.fireResistance.ToString ();
		iceValue.text = Player.Instance.icePower.ToString ();
		fireValue.text = Player.Instance.firePower.ToString ();
		try {
			bossHealthValue.text = GameObject.FindGameObjectWithTag ("Boss").GetComponent<Cerberus> ().health.ToString ();
			PlayerUI.Instance.transform.FindChild ("Boss Bar").gameObject.SetActive (true);
		} catch (NullReferenceException) {
			PlayerUI.Instance.transform.FindChild ("Boss Bar").gameObject.SetActive (false);
		}

		bool PauseMenuActive = false; // is there any menus active in the scene
		try {
			PauseMenuActive = DungeonUI.Instance.transform.Find ("Pause Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}

		if (!PauseMenuActive) {
			nextWeaponPanelCountdown = Mathf.Max (nextWeaponPanelCountdown - Time.deltaTime, 0.0f); // reduce next weapon panel countdown
		}

		// show next weapon panel when the next weapon panel countdown timer is still on
		if (nextWeaponPanelCountdown > 0.0f) {
			// update weapon durability panel
			if (!transform.FindChild ("Next Weapon Panel").gameObject.activeSelf) {
				transform.FindChild ("Next Weapon Panel").FindChild("Text").GetComponent<Text> ().text = "You are about\nto get a\n" + Poll.Instance._weaponDisplay.text + "!";

				// update next weapon on GameMaster
				if (Poll.Instance._weaponDisplay.text == Poll.Instance.hammerText) {
					Player.Instance.nextWeapon = Player.Instance.hammer;
                } else if (Poll.Instance._weaponDisplay.text == Poll.Instance.spearText) {
                    Debug.Log(Player.Instance.spear + " " + Player.Instance.nextWeapon);
                    Player.Instance.nextWeapon = Player.Instance.spear;
                    Debug.Log(Player.Instance.spear + " " + Player.Instance.nextWeapon);
                } else if (Poll.Instance._weaponDisplay.text == Poll.Instance.knifeText) {
					Player.Instance.nextWeapon = Player.Instance.knife;
                } else {
					Player.Instance.nextWeapon = Player.Instance.defaultSword;
				}

				Poll.Instance.ResetVoteWeapon(); // reset weapon votes
			}
			transform.FindChild ("Next Weapon Panel").gameObject.SetActive (true);
		} else {
			// show and update weapon durability timer
			if (transform.FindChild ("Next Weapon Panel").gameObject.activeSelf) {

				// update current weapon on GameMaster
				Player.Instance.currentWeapon = Player.Instance.nextWeapon;
                Player.Instance.UpdateAnimator();
				Player.Instance.nextWeapon = Player.Instance.defaultSword;

				transform.FindChild ("Weapon Timer").gameObject.SetActive (true);
				float weaponTimerMaxValue = float.Parse(transform.FindChild("Weapon Timer").FindChild ("Max Value").GetComponent<Text> ().text); // max value of the pie chart
				transform.FindChild ("Weapon Timer").FindChild ("Real Value").GetComponent<Text> ().text = string.Format ("{0:F1}", weaponTimerMaxValue);
			}
			transform.FindChild ("Next Weapon Panel").gameObject.SetActive (false);
		}

		// update displayed value on timer
		transform.FindChild ("Weapon Timer").FindChild ("Value").GetComponent<Text> ().text = string.Format ("{0:F1}", float.Parse(transform.FindChild ("Weapon Timer").FindChild ("Real Value").GetComponent<Text> ().text));

    }
}
