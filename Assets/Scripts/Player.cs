using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public float speed; // speed of player
	public float acceleration; // acceleration of player;
	[HideInInspector]
	public float defaultSpeed = 6.0f; // default speed of player
	[HideInInspector]
	public float defaultAcceleration = 20.0f; // default acceleration of player
	public int damage; // base damage
	public int health = 8; // base hit points
	public int healthRegeneration; // health regeneration speed;

	private Rigidbody2D rb; // rigid body of playersprite

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		rb.mass = 1.0f; // mass of player
		rb.drag = 0.0f; // drag of player
		speed = defaultSpeed;
		acceleration = defaultAcceleration;
	}

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertictal = Input.GetAxis("Vertical");

		Vector2 targetVelocity = new Vector3 (moveHorizontal, moveVertictal).normalized * speed; // target velocity of player

		Vector2 velocityDifference = (targetVelocity - rb.velocity) * acceleration;
		rb.AddForce(velocityDifference);
	}

	// Update is called once per frame
	void Update ()
	{		
		// update camera position
		Camera.main.transform.position = new Vector3 (transform.position[0], transform.position[1], Camera.main.transform.position[2]);

	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Exit")
		{
            Dungeon.Instance.GenerateRoom(Dungeon.Instance.roomWidth, Dungeon.Instance.roomHeight, null);
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
