using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScript : MonoBehaviour
{

	public float speed; // speed of player

	public GameObject player; // the player
	public GameObject wall; // wall tiles

	////public Sprite[] spritePlayer = new Sprite[40];

	// Use this for initialization
	void Start ()
	{
		// initialize player object
		player = (GameObject)Instantiate (player, transform.position, transform.rotation);
		player.transform.position = new Vector3 (0.0f, 0.0f, 0.0f);

		// set speed off player
		speed = 10.0f;

		// create room
		createRoom (100, 100);

		// initialize camera
		Camera.main.orthographicSize = 10;
		Camera.main.transform.position = new Vector3 (0.0f, 0.0f, -10.0f);
	}

	// Update is called once per frame
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

		// update camera position
		Camera.main.transform.position = new Vector3 (player.transform.position[0], player.transform.position[1], Camera.main.transform.position[2]);
	}

	// create a random room
	void createRoom (int width, int height)
	{
		System.Random random = new System.Random ();
		int[,] room = new int[width, height];

		// initializing room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				room [i, j] = random.NextDouble () < 0.5 ? 1 : -1;
			}
		}

		// smoothen room
		for (int count = 0; count < 10; count++) { // number of iterations
			int[,] newRoom = new int[width, height]; // array for storing the smoothed room

			// loop through each cell
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {

					// count whether there are more air blocks/wall blocks in the 3x3 area
					int wallCounter = 0;
					for (int row = i - 1; row <= i + 1; row++) {
						for (int col = j - 1; col <= j + 1; col++) {

							// add to counter 
							try {
								wallCounter += room [row, col];
							} catch (IndexOutOfRangeException) {}
						}
					}
					newRoom [i, j] = (wallCounter >= 0) ? 1 : -1; // set the block to wall/air depending on the counter

				}
			}

			room = newRoom; // update the room
		}

		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room [i, j] == 1) {
					// place walls
					wall = (GameObject)Instantiate (wall, transform.position, transform.rotation);
					wall.transform.position = new Vector3 (i - width / 2.0f, j - height / 2.0f, 0.0f);
				}
			}
		}
	}
		
}
