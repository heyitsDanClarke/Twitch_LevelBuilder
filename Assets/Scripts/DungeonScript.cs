using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScript : MonoBehaviour
{

	public float speed;	// speed of player
	public GameObject player; // the player
	////public Sprite[] spritePlayer = new Sprite[40];


	void Start ()
	{
		// initialize player object
		player = (GameObject)Instantiate (player, transform.position, transform.rotation);
		player.transform.position = new Vector3 (0.0f, 0.0f, 0.0f);

		// set speed off player
		speed = 5.0f;
	}

	void Update ()
	{
		// change of position of the player
		float horizontalMovement = speed * Time.deltaTime;
		float diagonalMovement = (float)Math.Sqrt (0.5) * speed * Time.deltaTime;

		// move the player according to the key combinations
		if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.LeftArrow)) {
			player.transform.position += new Vector3 (-diagonalMovement, diagonalMovement, 0.0f);
		} else if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.LeftArrow)) {
			player.transform.position += new Vector3 (-diagonalMovement, -diagonalMovement, 0.0f);
		} else if (Input.GetKey (KeyCode.DownArrow) && Input.GetKey (KeyCode.RightArrow)) {
			player.transform.position += new Vector3 (diagonalMovement, -diagonalMovement, 0.0f);
		} else if (Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.RightArrow)) {
			player.transform.position += new Vector3 (diagonalMovement, diagonalMovement, 0.0f);
		} else if (Input.GetKey (KeyCode.UpArrow)) {
			player.transform.position += new Vector3 (0.0f, horizontalMovement, 0.0f);
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			player.transform.position += new Vector3 (0.0f, -horizontalMovement, 0.0f);
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			player.transform.position += new Vector3 (-horizontalMovement, 0.0f, 0.0f);
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			player.transform.position += new Vector3 (horizontalMovement, 0.0f, 0.0f);
		}
	}
}
