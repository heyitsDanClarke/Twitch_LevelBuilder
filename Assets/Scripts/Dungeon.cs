using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public static Dungeon Instance;

    public int roomWidth = 48; // width of room
    public int roomHeight = 32; // height of room

	[HideInInspector]
    public GameObject dungeonVisual; // dungeon visual
    [HideInInspector]
    public GameObject enemyVisual; //enemy visual

    public GameObject iceTiles; // ice tiles
	public GameObject waterTiles; // water tiles
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
	public GameObject exit; // exit of room

	////public Sprite[] spritePlayer = new Sprite[40];

	// tile IDs, IDs must be different
	[HideInInspector]public int air = -1; // DO NOT MODIFY
	[HideInInspector]public int flood = 0;
	[HideInInspector]public int wall = 1; // DO NOT MODIFY
	[HideInInspector]public int ice = 2;
	[HideInInspector]public int water = 3;
	[HideInInspector]public int lava = 4;

	// entity IDs, IDs must be different
	[HideInInspector]public int empty = 0;
	[HideInInspector]public int loot = 1;
	[HideInInspector]public int small = 2;
	[HideInInspector]public int large = 3;

	[HideInInspector]public int[,] roomStructure; // room structure

	private System.Random random; // random numnber generator

	// Use this for initialization
	void Start ()
	{
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        random = new System.Random ();

        // initialize camera
		Camera.main.transform.position = new Vector3(0.0f, Mathf.Tan(Mathf.Deg2Rad * -20.0f) * 20.0f, -20.0f);

        generateRoom(roomWidth, roomHeight, null);

	}

	void FixedUpdate () {

		// modify player's acceleration and drag
		Player.Instance.acceleration = Player.Instance.defaultAcceleration;
		Player.Instance.rb.drag = 0.0f;
		try {
			int x = (int) Math.Round(player.transform.position.x, MidpointRounding.AwayFromZero); // integer x coordinate of player
			int y = (int) Math.Round(player.transform.position.y, MidpointRounding.AwayFromZero); // integer x coordinate of player
			if (roomStructure[x, y] == ice) {
				Player.Instance.acceleration = 1.5f; // make floor slippery if player is on ice
			}
			if (roomStructure[x, y] == water) {
				Player.Instance.rb.drag = 25.0f; // slow down player if player is in water
				Player.Instance.acceleration = Player.Instance.defaultAcceleration / 2.0f; // make water slightly slippery if player is on ice
			}
		} catch (IndexOutOfRangeException) {}


	}

	// function for generating a room from a random selected type and size
	public void generateRandomRoom() {
		generateRoom (roomWidth, roomHeight, null);
	}
		
	// function for generating a room
    void generateRoom(int roomWidth, int roomHeight, string type)
    {
        if (dungeonVisual != null)
			Destroy(dungeonVisual);
        if (enemyVisual != null)
            Destroy(enemyVisual);

        dungeonVisual = new GameObject(); 
		dungeonVisual.transform.name = "Dungeon Visual";
        dungeonVisual.transform.SetParent(transform);

        enemyVisual = new GameObject();
        enemyVisual.transform.name = "Enemy Visual";
        enemyVisual.transform.SetParent(transform);

        // create room
        roomStructure = generateRoomArray(roomWidth, roomHeight);
		createRoom(roomStructure, GameMaster.Instance.fireCount, GameMaster.Instance.iceCount); // room array, fire votes, ice votes
        Poll.Instance.ResetVote(); // reset votes
    }

	// generate an array containing the information of a room in the scene
	int [,] generateRoomArray (int width, int height) // height of room, width of room
	{
		int[,] room = new int[width, height];

		Vector3 playerLocation = new Vector3 (0.0f, 0.0f, 0.0f); // player spawn location
		Vector3 exitLocation = new Vector3 (0.0f, 0.0f, 0.0f); // room exit location

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
				bool playerSpawnAtTop = (random.NextDouble () < 0.5)? true : false;

				// create exits on top and bottom of room
				int position = random.Next (1, width - w - 1);
				if (!playerSpawnAtTop) {
					playerLocation = new Vector3 (position + w / 2.0f, 0.0f, 0.0f);
				} else {
					exitLocation = new Vector3 (position + w / 2.0f, 0.0f, 0.0f);
				}
				room = fillAirSquare (room, position, -w / 2, w);

				position = random.Next (1, width - w - 1);
				if (playerSpawnAtTop) {
					playerLocation = new Vector3 (position + w / 2.0f, height - 1, 0.0f);
				} else {
					exitLocation = new Vector3 (position + w / 2.0f, height - 1, 0.0f);
				}
				room = fillAirSquare (room, position, height - w / 2, w);
			} else {
				bool playerSpawnAtRight = (random.NextDouble () < 0.5)? true : false;

				// create exits on left and right of room
				int position = random.Next (1, height - w - 1);
				if (!playerSpawnAtRight) {
					playerLocation = new Vector3 (0.0f, position + w / 2.0f, 0.0f);
				} else {
					exitLocation = new Vector3 (0.0f, position + w / 2.0f, 0.0f);
				}
				room = fillAirSquare (room, -w / 2, position, w);

				position = random.Next (1, height - w - 1);
				if (playerSpawnAtRight) {
					playerLocation = new Vector3 (width - 1, position + w / 2.0f, 0.0f);
				} else {
					exitLocation = new Vector3 (width - 1, position + w / 2.0f, 0.0f);
				}
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

        // set player and exit locations
        player.transform.position = playerLocation;
		GameObject tempExit = Instantiate (exit, exitLocation, transform.rotation);
        tempExit.transform.SetParent(dungeonVisual.transform);

		return room;
	}

	void createRoom (int [,] room, int fireVotes, int iceVotes) { // array of the room, number of fire votes, number of ice votes
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;
		float xScale = 0.04f + 1.8f / Mathf.Min(width, height); // x scale of perlin noise
		float xOffset, yOffset; // x and y offset of perlin noise;
		GameObject tempTile; // temporary variable for storing the tile to be created
		GameObject tempEntity; // temporary variable for storing the entity to be created

		// set temperature map of room;
		float [,] temperature = new float[width, height];
		xOffset = (float) (random.NextDouble ()) * 200;
		yOffset = (float) (random.NextDouble ()) * 200;
		float heightOffset = (fireVotes + iceVotes == 0)? 0.0f : 0.3f * (fireVotes - iceVotes) / ((float) (fireVotes + iceVotes)); // height offset of perlin noise for the temperature map
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				temperature [i, j] = (float) Mathf.Clamp01(Mathf.Clamp01(Mathf.PerlinNoise (xOffset + i * xScale, yOffset + j * xScale) * 1.5f - 0.25f) * 0.4f + 0.3f + heightOffset);
			}
		}

		// set humidity map of room;
		float [,] humidity = new float[width, height]; // humidity map of room;
		xOffset = (float) (random.NextDouble ()) * -200 - 400;
		yOffset = (float) (random.NextDouble ()) * -200 - 400;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				humidity [i, j] = (float) Mathf.Clamp01(Mathf.PerlinNoise (xOffset + i * xScale, yOffset + j * xScale));
			}
		}
			
		// create array for storing eneities
		int[,] entities = new int[width, height];	
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				entities [i, j] = empty;
			}
		}

        // thresholds for climates
		float wetThreshold = 0.6f;
		float dryThreshold = 0.4f;
		float hotThreshold = 0.55f;
		float coldThreshold = 0.45f;

		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room [i, j] == air) { // set lava/water/floor tiles base on the temperature and humidity
					if (humidity [i, j] > wetThreshold && temperature [i, j] < hotThreshold) {
						GameObject setTile; // the tile to be set in the dungeon
						int setTileID; // the id of the tile to be set in the dungeon
						if (temperature [i, j] < coldThreshold) {
							setTile = iceTiles;
							setTileID = ice;
						} else {
							setTile = waterTiles;
							setTileID = water;
						}
						tempTile = Instantiate(setTile, new Vector3(i, j, 0.0f), transform.rotation);
                        tempTile.transform.SetParent(dungeonVisual.transform);
						room [i, j] = setTileID;
					} else if (humidity [i, j] < dryThreshold && temperature [i, j] >= hotThreshold) {
						tempTile = Instantiate (lavaTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                        tempTile.transform.SetParent(dungeonVisual.transform);
                        room [i, j] = lava;
					} else {
						float hotTileTransparency = Mathf.Clamp01 (temperature [i, j] * 10 - 5);
						if (hotTileTransparency < 1.0f) {
                            tempTile = Instantiate(floorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                            tempTile.transform.SetParent(dungeonVisual.transform);
                        }
						tempTile = Instantiate (hotFloorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						tempTile.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(temperature [i, j] * 10 - 5));
						tempTile.transform.SetParent(dungeonVisual.transform);
                    }
				}

				if (room [i, j] == wall) { // set the walls and borders of the room
					// see if there are air tiles in the 3x3 area
					bool nearAir = false; // there is air tiles nearby
					for (int row = i - 1; row <= i + 1; row++) {
						for (int col = j - 1; col <= j + 1; col++) {
							try {
								if (room [row, col] != wall) {
									nearAir = true;
								}
							} catch (IndexOutOfRangeException) {}
						}
					}

					if (nearAir) {
						// set border tiles
						float hotTileTransparency = Mathf.Clamp01 (temperature [i, j] * 10 - 5);
						if (hotTileTransparency < 1.0f) {
							GameObject tempBorderTile = Instantiate (borderTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                            tempBorderTile.transform.SetParent(dungeonVisual.transform);
                        }
						tempTile = (GameObject)Instantiate (hotBorderTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                        tempTile.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(temperature [i, j] * 10 - 5));
                        tempTile.transform.SetParent(dungeonVisual.transform);
                    } else {
                        // set wall tiles
                        tempTile = Instantiate(wallTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                        tempTile.transform.SetParent(dungeonVisual.transform);
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
                    tempEntity = Instantiate(lootBox, new Vector3 (x, y, 0.0f), transform.rotation);
                    tempEntity.transform.SetParent(dungeonVisual.transform);
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
					tempEntity = (GameObject)Instantiate (largeMob, new Vector3 (x, y, 0.0f), transform.rotation);
                    tempEntity.transform.SetParent(enemyVisual.transform);
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
					tempEntity = (GameObject)Instantiate (smallMob, new Vector3 (x, y, 0.0f), transform.rotation);
                    tempEntity.transform.SetParent(dungeonVisual.transform);
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
									if (room [i, j] != wall && entities [i, j] != small && entities [i, j] != large && entities [i, j] != loot) {
										break;
									}
								} catch (IndexOutOfRangeException) {}
							}
                            tempEntity = Instantiate(smallMob, new Vector3(i, j, 0.0f), transform.rotation);
                            tempEntity.transform.SetParent(enemyVisual.transform);
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
