using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    Text healthValue;

    void Awake()
    {
        healthValue = transform.GetChild(0).FindChild("Health_Value").GetComponent<Text>();
    }

	void FixedUpdate()
    {
        healthValue.text = Player.Instance.health.ToString();
    }
}
