using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public static PlayerUI Instance;

    Text healthValue;
	Text coinValue;
	Text iceValue;
	Text fireValue;
    Text chargesValue;
	Text fireResistanceValue;

	public Sprite boxIcon;
	public Sprite leverIcon;

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
		coinValue = transform.FindChild("Coins Panel").FindChild("Value").GetComponent<Text>();
		iceValue = transform.FindChild("Ice Power Panel").FindChild("Value").GetComponent<Text>();
		fireValue = transform.FindChild("Fire Power Panel").FindChild("Value").GetComponent<Text>();
    }

	void Update() {
		transform.FindChild("Health Bar").FindChild("Max Value").GetComponent<Text>().text = Player.Instance.maxHealth.ToString ();
		transform.FindChild("Charges Bar").FindChild("Max Value").GetComponent<Text>().text = Player.Instance.maxCharges.ToString ();
		transform.FindChild ("Fire Resistance Bar").FindChild ("Max Value").GetComponent<Text> ().text = Player.Instance.maxFireResistance.ToString ();
		healthValue.text = Player.Instance.health.ToString ();
		chargesValue.text = Player.Instance.charges.ToString ();
		fireResistanceValue.text = Player.Instance.fireResistance.ToString ();
		coinValue.text = Player.Instance.coins.ToString ();
		iceValue.text = Player.Instance.icePower.ToString ();
		fireValue.text = Player.Instance.firePower.ToString ();
    }
}
