using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    Text healthValue;
	Text coinValue;

    void Awake()
    {
        healthValue = transform.GetChild(0).FindChild("Health_Value").GetComponent<Text>();
		//coinValue = transform.GetChild(0).FindChild("Coin_Value").GetComponent<Text>();
    }

	void FixedUpdate()
    {
        //coinValue.text = Player.Instance.coins.ToString();
    }
}
