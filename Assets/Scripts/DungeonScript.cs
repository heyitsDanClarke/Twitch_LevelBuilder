using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScript : MonoBehaviour
{

	public float speed; // speed of player

	public GameObject iceTiles; // ice tiles
	public GameObject waterTiles; // water tiles
	public GameObject steamTiles; // steam tiles
	public GameObject floorTiles; // floor tiles
	public GameObject hotFloorTiles; // hot texture of floor tiles
	public GameObject lavaTiles; // lava tiles
	public GameObject player; // the player
	public GameObject borderTiles; // border tiles
	public GameObject hotBorderTiles; // hot texture of border tiles
	public GameObject wallTiles; // wall tiles
	public GameObject smallMob; // small monster
	public GameObject largeMob; // large monster
	public GameObject lootBox; // loot box

	////public Sprite[] spritePlayer = new Sprite[40];

	// tile IDs, IDs must be different
	private int air = -1; // DO NOT MODIFY
	private int flood = 0;
	private int wall = 1; // DO NOT MODIFY

	// entity IDs, IDs must be different
	private int empty = 0;
	private int small = 1;
	private int large = 2;
	private int ice = 3;
	private int water = 4;
	private int steam = 5;
	private int lava = 6;
	private int loot = 7;


	private System.Random random; // random numnber generator

	// Use this for initialization
	void Start ()
	{
		random = new System.Random ();
		int roomWidth = 96; // width of room
		int roomHeight = 64; // height of room

		// set speed off player
		speed = 10.0f;

		// create room
		int [,] randomRoom = generateRoomArray (roomWidth, roomHeight);
		createRoom (randomRoom, 0, 0);

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

	// generate an array containing the information of a room
	int [,] generateRoomArray (int width, int height) // height of room, width of room
	{
		int[,] room = new int[width, height];

		bool satisfied; // whether room is satisfied or not
		do {		
			satisfied = true;

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

						// count whether there are more air tiles/wall tiles in the 3x3 area
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
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					room [i, j] = (room [i, j] == flood) ? air : wall;
					if (room [i, j] == air) {
						if (i == 0 || j == 0) {
							touchBottomOrLeft = true;
						}
						if (i == width - 1 || j == height - 1) {
							touchTopOrRight = true;
						}
					}
				}
			}

			satisfied = satisfied && (touchBottomOrLeft && touchTopOrRight); // the exits are connected

			// check whether each quarter of the room has enough air tiles
			for (int xCount = 0; xCount <= 1; xCount++) {
				for (int yCount = 0; yCount <= 1; yCount++) {
					int airCount = 0; // counter for number of air tiles
					// loop through all the tiles in a quarter of the room
					for (int i = xCount * width / 2; i < (xCount + 1) * width / 2; i++) {
						for (int j = yCount * height / 2; j < (yCount + 1) * height / 2; j++) {
							if (room [i, j] == air) {
								airCount += 1;
							}
						}
					}
					satisfied = satisfied && (airCount * 4.0f / width / height >= 0.4f); // at least 40% of each quarter of the room is air
				}
			}
				
		} while (!satisfied);

		return room;
	}

	void createRoom (int [,] room, int fireVotes, int iceVotes) { // array of the room, number of fire votes, number of ice votes
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;
		float xScale = 0.06f + 2.0f / Mathf.Min(width, height); // x scale of perlin noise
		float xOffset, yOffset; // x and y offset of perlin noise;

		// set temperature map of room;
		float [,] temperature = new float[width, height];
		xOffset = (float) (random.NextDouble ()) * 1000;
		yOffset = (float) (random.NextDouble ()) * 1000;
		float heightOffset = (fireVotes + iceVotes == 0)? 0.0f : 0.3f * (fireVotes - iceVotes) / ((float) (fireVotes + iceVotes)); // height offset of perlin noise for the temperature map
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				temperature [i, j] = (float) Mathf.Clamp01(Mathf.Clamp01(Mathf.PerlinNoise (xOffset + i * xScale, xOffset + j * xScale) * 1.5f - 0.25f) * 0.4f + 0.3f + heightOffset);
			}
		}

		// set humidity map of room;
		float [,] humidity = new float[width, height]; // humidity map of room;
		xOffset = (float) (random.NextDouble ()) * 1000;
		yOffset = (float) (random.NextDouble ()) * 1000;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				humidity [i, j] = (float) Mathf.Clamp01(Mathf.PerlinNoise (xOffset + i * xScale, xOffset + j * xScale));
			}
		}
			
		// create array for storing eneities
		int[,] entities = new int[width, height];	
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				entities [i, j] = empty;
			}
		}
			
		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room [i, j] == air) { // set lava/steam/water/floor tiles base on the temperature and humidity
					if (humidity [i, j] > 0.65f) {
						GameObject setTile; // the tile to be set in the dungeon
						int setTileID; // the id of the tile to be set in the dungeon 
						if (temperature [i, j] > 0.55f) {
							setTile = steamTiles;
							setTileID = steam;
						} else if (temperature [i, j] < 0.45f) {
							setTile = iceTiles;
							setTileID = ice;
						} else {
							setTile = waterTiles;
							setTileID = water;
						}
						GameObject tileClone = (GameObject)Instantiate (setTile, new Vector3 (i, j, 0.0f), transform.rotation);
						entities [i, j] = setTileID;
					} else if (humidity [i, j] < 0.35f && temperature [i, j] > 0.55f) {
						GameObject tileClone = (GameObject)Instantiate (lavaTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						entities [i, j] = lava;
					} else {
						GameObject tileClone = (GameObject)Instantiate (floorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						tileClone = (GameObject)Instantiate (hotFloorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						tileClone.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(temperature [i, j] * 10 - 4));
					}
				}

				if (room [i, j] == wall) { // set the walls and borders of the room
					// see if there are air tiles in the 3x3 area
					bool nearAir = false; // there is air tiles nearby
					for (int row = i - 1; row <= i + 1; row++) {
						for (int col = j - 1; col <= j + 1; col++) {
							try {
								if (room [row, col] == air) {
									nearAir = true;
								}
							} catch (IndexOutOfRangeException) {}
						}
					}

					if (nearAir) {
						// set border tiles
						GameObject tileClone = (GameObject)Instantiate (borderTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						tileClone = (GameObject)Instantiate (hotBorderTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						tileClone.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(temperature [i, j] * 10 - 4));
					} else {
						// set wall tiles
						GameObject tileClone = (GameObject)Instantiate (wallTiles, new Vector3 (i, j, 0.0f), transform.rotation);
					}
				}
			}
		}

		// place loots
		for (int spawnCount = 0; spawnCount < width * height / 1536; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 16; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
				} while (room [x, y] != air || entities [x, y] != empty || countAdjacent (room, x, y, wall, 1) <= 3);
				if (countAdjacent (room, x, y, wall, 2) > 11 && countAdjacent (entities, x, y, loot, 16) == 0) { // if there are more than 11 wall tiles in the 5x5 square area and there is no loot boxes nearby
					// spawn loot box
					GameObject mobClone = (GameObject)Instantiate (lootBox, new Vector3 (x, y, 0.0f), transform.rotation);
					entities [x, y] = loot;
					break;
				}
			}
		}

		// spawn large monsters
		for (int spawnCount = 0; spawnCount < width * height / 1536; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 64; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
				} while (room [x, y] != air);
				if (countAdjacent (room, x, y, wall, 4) < 4 && countAdjacent (entities, x, y, large, 12) == 0) { // if there are less than 4 wall tiles in the 9x9 square area, and no large monsters nearby
					// spawn large monster
					GameObject mobClone = (GameObject)Instantiate (largeMob, new Vector3 (x, y, 0.0f), transform.rotation);
					entities [x, y] = large;
					break;
				}
			}
		}

		// spawn small monsters
		for (int spawnCount = 0; spawnCount < width * height / 384; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 64; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
				} while (room [x, y] != air);
				if (countAdjacent (room, x, y, wall, 2) < 4 && countAdjacent (entities, x, y, small, 8) == 0 && countAdjacent (entities, x, y, large, 8) == 0) { // if there are less than 4 wall tiles in the 5x5 square area and no mobs nearby
					// spawn small monster
					GameObject mobClone = (GameObject)Instantiate (smallMob, new Vector3 (x, y, 0.0f), transform.rotation);
					entities [x, y] = small;
					int mobCluster = random.Next (3 , 6); // size of mob cluster (3 to 5 inclusive)
					for (int mobCount = 1; mobCount < mobCluster; mobCount++) {
						for (int spawnClusterAttempt = 0; spawnClusterAttempt < 12; spawnClusterAttempt++) {
							int i, j; // x and y coordinates of the room
							while (true) {
								// choose a point within 2 distance from the first small mob in the cluster
								i = random.Next (x - 2, x + 2 + 1);
								j = random.Next (y - 2, y + 2 + 1);
								try {
									if (room [i, j] == air && entities [i, j] != small && entities [i, j] != large && entities [i, j] != loot) {
										break;
									}
								} catch (IndexOutOfRangeException) {}
							}
							mobClone = (GameObject)Instantiate (smallMob, new Vector3 (i, j, 0.0f), transform.rotation);
							entities [x, y] = small;
							break;
						}
					}
					break;
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
	int[,] fillAirSquare (int[,] room, int x, int y, int size) { // room array, x-coordinate, y-coordinate, size of square
		for (int row = x; row <= x + size; row++) {
			for (int col = y; col <= y + size; col++) {
				try {
					room [row, col] = air;
				} catch (IndexOutOfRangeException) {}
			}
		}
		return room;
	}

	// count the number of specific tiles in the area of a specific point
	int countAdjacent (int [,] room, int x, int y, int tile, int distance) { // room array, x-coordinate, y-coordinate, tile type, maximum distance from the tile
		int tileCounter = 0;
		for (int row = x - distance; row <= x + distance; row++) {
			for (int col = y - distance; col <= y + distance; col++) {
				// add to counter 
				try {
					if (room [row, col] == tile) {
						tileCounter += 1;
					}
				} catch (IndexOutOfRangeException) {}
			}
		}
		return tileCounter;
	}
}
