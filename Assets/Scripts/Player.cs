using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public static Player Instance;

	public float speed; // speed of player
	public float acceleration; // acceleration of player;
	[HideInInspector]
	public float defaultSpeed = 6.0f; // default speed of player
	[HideInInspector]
	public float defaultAcceleration = 20.0f; // default acceleration of player
	public int damage; // base damage
	public int health = 8; // base hit points
	public int healthRegeneration; // health regeneration speed;

	public Vector2 savedVelocity; // velocity of player

	[HideInInspector]
	public Rigidbody2D rb; // rigid body of playersprite

	// Use this for initialization
	void Start () {
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		rb = GetComponent<Rigidbody2D>();
		rb.mass = 1.0f; // mass of player
		rb.drag = 0.0f; // drag of player
		speed = defaultSpeed;
		savedVelocity = new Vector2 (0, 0);
		acceleration = defaultAcceleration;
	}

	void FixedUpdate ()
	{
		float moveHorizontal = 0.0f;
		float moveVertictal = 0.0f;

		bool menusActive = false; // is there any menus active in the scene

		try {
			if (DungeonUI.Instance != null) {
				foreach (Transform child in DungeonUI.Instance.transform) {
					menusActive = menusActive || child.gameObject.activeSelf;
				}
			}
		} catch (NullReferenceException) {}


		if (!menusActive) { // if no menus are active
			moveHorizontal = Input.GetAxisRaw ("Horizontal");
			moveVertictal = Input.GetAxisRaw ("Vertical");
		}

		// pause player while pause menu is active
		try {
			if (DungeonUI.Instance != null) {
				if (DungeonUI.Instance.transform.GetChild (1).gameObject.activeSelf) { // if pause menu is active
					if (rb.velocity != new Vector2 (0, 0)) { // if player is moving
						savedVelocity = rb.velocity; // save current velocity of player
						rb.velocity = new Vector2 (0, 0); // pause player
					}
				} else { // if pause menu is not active
					if (savedVelocity != new Vector2 (0, 0)) {// if velocity is being saved
						rb.velocity = savedVelocity; // restore velocity of player
						savedVelocity = new Vector2 (0, 0); // reset saved velocity
					}
				}
			}
		} catch (NullReferenceException) {}

		Vector2 targetVelocity = new Vector3 (moveHorizontal, moveVertictal).normalized * speed; // target velocity of player

		Vector2 velocityDifference = (targetVelocity - rb.velocity) * acceleration;
		rb.AddForce (velocityDifference);

	}

	// Update is called once per frame
	void Update ()
	{		
		// update camera position
		Camera.main.transform.position = new Vector3 (transform.position[0], transform.position[1] + Mathf.Tan(Mathf.Deg2Rad * -20.0f) * 20.0f, Camera.main.transform.position[2]);

	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Exit")
		{
			DungeonUI.Instance.showNextLevelMenu ();
		}
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            if (health > 0)
                health -= 1;
            Destroy(coll.gameObject);
        }
    }
}
