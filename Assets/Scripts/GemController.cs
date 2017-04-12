using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour {
	
	Rigidbody2D rb;

	public int firePower; // the fire power of the gem
	public int icePower; // the ice power of the gem

	public Sprite fireGemSprite;
	public Sprite iceGemSprite;

    public float speed = 1.0f;

    // Use this for initialization
    void Start () {
		if (firePower == 1) {
			GetComponent<SpriteRenderer>().sprite = fireGemSprite;
		} else {
			GetComponent<SpriteRenderer>().sprite = iceGemSprite;
		}

		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 playerPos = Player.Instance.transform.position;
		bool PauseMenuActive = false; // is the pause menus active in the scene
		try {
			PauseMenuActive = DungeonUI.Instance.transform.Find ("Pause Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}

		Vector2 targetVelocity = new Vector2 (0, 0);
		if (!PauseMenuActive) { // if the pause menu is not active
			targetVelocity = (new Vector3(playerPos.x, playerPos.y, transform.position.z) - transform.position).normalized * speed; // target velocity of gem
			speed += 5.0f * Time.deltaTime; // increase speed
		}

		Vector2 velocityDifference = (targetVelocity - rb.velocity) * 10.0f;
		rb.AddForce (velocityDifference);
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.gameObject.tag == "Player") {

			coll.gameObject.GetComponent<Player>().firePower += firePower;
			coll.gameObject.GetComponent<Player>().icePower += icePower;

			Player.Instance.score += 25;

			Destroy(this.gameObject);
		}
	}
}
