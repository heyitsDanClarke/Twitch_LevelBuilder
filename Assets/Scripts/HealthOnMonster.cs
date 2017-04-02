using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOnMonster : MonoBehaviour {
	public float value;
	public float maxValue;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		try {
		value = gameObject.GetComponentInParent<MonsterAI>().health;
		maxValue = gameObject.GetComponentInParent<MonsterAI>().maxHealth;
		} catch (NullReferenceException) {}
		try {
			value = gameObject.GetComponentInParent<EyeBat>().health;
			maxValue = gameObject.GetComponentInParent<EyeBat>().maxHealth;
		} catch (NullReferenceException) {}
		try {
			value = gameObject.GetComponentInParent<LootBoxController>().health;
			maxValue = gameObject.GetComponentInParent<LootBoxController>().maxHealth;
		} catch (NullReferenceException) {}



		// show health bar if enemy is not on full health
		//if (value != maxValue) {
		//	gameObject.SetActive (true);
		//}

		transform.FindChild("Bar").transform.localScale = new Vector2 (Mathf.Max (value / maxValue, 0), 1);
	}
}
