using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public float speed; // speed of player
	public int damage; // base damage
	public int health = 8; // base hit points
	public int healthRegeneration; // health regeneration speed;

	private Rigidbody2D rb; // rigid body of playersprite

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		rb.mass = 0.01f; // mass of player
		rb.drag = 1000.0f; // drag of player

		// set speed off player
		speed = 100.0f;
		
	}

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertictal = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, moveVertictal, 0.0f);
		movement = movement.normalized * speed;

		rb.AddForce(movement);
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

        else if(coll.gameObject.tag == "Enemy")
        {
            if (health > 0)
                health -= 1;
        }
	}
}
