using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardController : MonoBehaviour {
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.gameObject.tag == "Player") {
			coll.gameObject.GetComponent<Player>().charges += 1;

			// activate next weapon panel for 3.0 seconds
			if (Player.Instance.charges >= Player.Instance.maxCharges) {
				Player.Instance.charges = 0;
				PlayerUI.Instance.nextWeaponPanelCountdown = 3.0f;
			}

			Destroy(this.gameObject);
		}
	}

}
