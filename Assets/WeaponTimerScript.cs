using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTimerScript : MonoBehaviour {

	public float value; // value of the pie chart
	public float maxValue; // max value of the pie chart

	// Use this for initialization
	void Start () {
		value = 20.0f;
		maxValue = 20.0f;
	}
	
	// Update is called once per frame
	void Update () {
		// update pie chart
		value = Mathf.Max(value - Time.deltaTime, 0.0f);
		transform.FindChild ("Value").GetComponent<Text> ().text = string.Format ("{0:F1}", value);
		transform.FindChild ("Max Value").GetComponent<Text> ().text = string.Format ("{0:F1}", maxValue);

		//value = float.Parse (transform.FindChild ("Value").GetComponent<Text> ().text);
		//maxValue = float.Parse(transform.FindChild ("Max Value").GetComponent<Text> ().text);

		transform.FindChild("Pie").GetComponent<Image> ().fillAmount = Mathf.Clamp01(value / maxValue);
	}
}
