using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScript : MonoBehaviour
{

	public float speed; // speed of player

	public GameObject iceTiles; // ice tiles
	public GameObject lavaTiles; // lava tiles
	public GameObject player; // the player
	public GameObject wallTiles; // wall tiles

	////public Sprite[] spritePlayer = new Sprite[40];

	// tile IDs, IDs must be different
	private int air = -1; // DO NOT MODIFY
	private int flood = 0;
	private int wall = 1; // DO NOT MODIFY
	private int fire = 2;
	private int ice = 3;

	// Use this for initialization
	void Start ()
	{
		int roomWidth = 96; // width of room
		int roomHeight = 64; // height of room

		// set speed off player
		speed = 10.0f;

		// create room
		createRoom (roomWidth, roomHeight, 3, 5);

		// initialize player object
		player = (GameObject)Instantiate (player, new Vector3 (roomWidth / 2, roomHeight / 2, 0.0f), transform.rotation);

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
	void createRoom (int width, int height, int fireVotes, int iceVotes) // height of room, width of room, number of fire votes, number of ice votes
	{
		System.Random random = new System.Random ();
		int[,] room = new int[width, height];

		bool satisfied = false; // whether room is satisfied or not
		do {		

			// initializing room and add exterior boundary to room
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					room [i, j] = random.NextDouble () < 0.475 ? wall : air;
					room [i, j] = (i == 0 || j == 0 || i == width - 1 || j == height - 1) ? wall : room [i, j]; // add exterior boundary
				}
			}

			// create exits of the room on the opposite sides
			int w = 4; // width of exit
			if (random.NextDouble () < 0.5) {
				// create exits on top and bottom of room
				int position = random.Next (1, width - w - 1);
				room = fillAirSquare (room, position, -w / 2, w);
				position = random.Next (1, width - w - 1);
				room = fillAirSquare (room, position, height - w / 2, w);
			} else {
				// create exits on left and right of room
				int position = random.Next (1, height - w - 1);
				room = fillAirSquare (room, -w / 2, position, w);
				position = random.Next (1, height - w - 1);
				room = fillAirSquare (room, width - w / 2, position, w);
			}

			// smoothen room
			for (int loopCount = 0; loopCount < 8; loopCount++) { // number of iterations
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
						newRoom [i, j] = (wallCounter >= 0) ? wall : air; // set the block to wall/air depending on the counter

					}
				}

				room = newRoom; // update the room
			}

			// flood fill to check whether the room is large enough and the two exits are connected
			bool touchBottomOrLeft = false;
			bool touchTopOrRight = false;
			// select a random point in the room that is an air tile
			int x, y;
			do {
				x = random.Next (0, width);
				y = random.Next (0, height);
			} while (room [x, y] != air);
			room = floodFill (room, x, y);

			// set flood to air, and others to wall
			int airCount = 0; // counter for number of air tiles
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					room [i, j] = (room [i, j] == flood) ? air : wall;
					if (room [i, j] == air) {
						airCount += 1;
						if (i == 0 || j == 0) {
							touchBottomOrLeft = true;
						}
						if (i == width - 1 || j == height - 1) {
							touchTopOrRight = true;
						}
					}
				}
			}

			satisfied = satisfied || airCount / width / height >= 0.4; // at least 40% of the room is air
			satisfied = satisfied || (touchBottomOrLeft && touchTopOrRight); // the exits are connected
		} while (!satisfied);

		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room [i, j] == wall) {
					// place walls
					GameObject wallClone = (GameObject)Instantiate (wallTiles, new Vector3 (i, j, 0.0f), transform.rotation);
				}
			}
		}
	}

	// flood filling tiles around chosen point
	int[,] floodFill (int[,] room, int i, int j) {
		if (room [i, j] == air) {
			room [i, j] = flood;
			try {
				room = floodFill (room, i + 1, j);
				room = floodFill (room, i - 1, j);
				room = floodFill (room, i, j + 1);
				room = floodFill (room, i, j - 1);
			} catch (IndexOutOfRangeException) {}
			return room;
		} else {
			return room;
		}
	}

	// fill the square area with defined size with air, with [x, y] as the bottom left corner of the square
	int[,] fillAirSquare (int[,] room, int x, int y, int size) {
		for (int row = x; row <= x + size; row++) {
			for (int col = y; col <= y + size; col++) {
				try {
					room [row, col] = air;
				} catch (IndexOutOfRangeException) {}
			}
		}
		return room;
	}
}
