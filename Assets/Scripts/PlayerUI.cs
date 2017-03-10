using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    Text healthValue;
	Text coinValue;
    Text mercuryBladeLevelValue;

    void Awake()
    {
		healthValue = transform.FindChild("Player Panel").FindChild("Health Value").GetComponent<Text>();
		coinValue = transform.FindChild("Player Panel").FindChild("Coin Value").GetComponent<Text>();
        mercuryBladeLevelValue = transform.FindChild("Player Panel").FindChild("Mercury Blade Value").GetComponent<Text>();
    }

	void FixedUpdate()
    {
        healthValue.text = Player.Instance.health.ToString();
        coinValue.text = Player.Instance.coins.ToString();
        mercuryBladeLevelValue.text = Player.Instance.mercuryBladeShardLevel.ToString();
    }
}
