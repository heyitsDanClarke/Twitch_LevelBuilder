using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		float value = float.Parse (transform.FindChild ("Value").GetComponent<Text> ().text);
		float maxValue = float.Parse(transform.FindChild ("Max Value").GetComponent<Text> ().text);

		transform.FindChild("Bar").GetComponent<RectTransform> ().localScale = new Vector2 (Mathf.Max (value / maxValue, 0), 1);
	}
}
