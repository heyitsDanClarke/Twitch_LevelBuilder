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
		if(GetComponent<RectTransform>().localScale.x > 0)
            GetComponent<RectTransform>().localScale = new Vector2(Mathf.Max( GetComponent<RectTransform>().localScale.x - Time.deltaTime,0),1);
	}
}
