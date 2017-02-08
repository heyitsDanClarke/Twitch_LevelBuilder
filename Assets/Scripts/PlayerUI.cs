using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public GameObject player;

    Text healthValue;

    void Awake()
    {
        healthValue = transform.GetChild(0).FindChild("Health_Value").GetComponent<Text>();
    }

	void FixedUpdate()
    {
        healthValue.text = player.GetComponent<Player>().health.ToString();
    }
}
