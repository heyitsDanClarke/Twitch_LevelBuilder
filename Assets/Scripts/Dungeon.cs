using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dungeon : MonoBehaviour
{
    public static Dungeon Instance;

    public int roomWidth = 48; // width of room
    public int roomHeight = 32; // height of room
	public int roomsLeftUntilBoss; // number of rooms left before boss room

	public struct RoomTile // structure for every pixel of the room
	{
		public int tile;
		public int entity;
		public int plate;
		public float temperature;
		public float humidity;
	}

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
	[HideInInspector]public int box = 4;

	// plate IDs, IDs must be different
	//[HideInInspector]public int empty = 0; // COMMENTED OUT, BUT STILL DO NOT MODIFY
	[HideInInspector]public int plate = 1;

	[HideInInspector]public RoomTile[,] roomStructure; // room structure

	private Vector2 playerStartPosition; // starting position of player
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

		roomsLeftUntilBoss = 5;
		GenerateRandomRoom ();
	}

	void FixedUpdate () {

		// modify player's acceleration and drag
		Player.Instance.acceleration = Player.Instance.defaultAcceleration;
		Player.Instance.rb.drag = 0.0f;
		try {
			int x = (int) Math.Round(player.transform.position.x, MidpointRounding.AwayFromZero); // integer x coordinate of player
			int y = (int) Math.Round(player.transform.position.y, MidpointRounding.AwayFromZero); // integer x coordinate of player
			if (roomStructure[x, y].tile == ice) {
				Player.Instance.acceleration = 1.5f; // make floor slippery if player is on ice
			}
			if (roomStructure[x, y].tile == water) {
				Player.Instance.rb.drag = 25.0f; // slow down player if player is in water
				Player.Instance.acceleration = Player.Instance.defaultAcceleration / 2.0f; // make water slightly slippery if player is on ice
			}
		} catch (IndexOutOfRangeException) {}
			
	}

    public void GenerateRandomRoom()
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

		if (roomsLeftUntilBoss > 0) {
			GenerateCaveRoom (roomWidth, roomHeight, null);
		} else if (roomsLeftUntilBoss == 0) {
			GenerateBossRoom (roomWidth, roomHeight);
		} else {
			SceneManager.LoadScene(0);
		}
		roomsLeftUntilBoss -= 1;
		Poll.Instance.ResetVote(); // reset votes
    }

	// function for creating the border of the room in the scene
	void InstantiateRoomBorder (int roomWidth, int roomHeight) {
		for (int i = -1; i < roomWidth + 1; i++) {
			GameObject wallTile = Instantiate(wallTiles, new Vector3(i, -1, 0.0f), transform.rotation);
			wallTile.transform.SetParent(dungeonVisual.transform);
			wallTile = Instantiate(wallTiles, new Vector3(i, roomHeight, 0.0f), transform.rotation);
			wallTile.transform.SetParent(dungeonVisual.transform);
		}
		for (int j = 0; j < roomHeight; j++) {
			GameObject wallTile = Instantiate(wallTiles, new Vector3(-1, j, 0.0f), transform.rotation);
			wallTile.transform.SetParent(dungeonVisual.transform);
			wallTile = Instantiate(wallTiles, new Vector3(roomWidth, j, 0.0f), transform.rotation);
			wallTile.transform.SetParent(dungeonVisual.transform);
		}

	}

	// function for generating a boss room
	void GenerateBossRoom(int roomWidth, int roomHeight)
	{
		// create room
		roomStructure = GenerateBossRoomArray(roomWidth, roomHeight);
		InstantiateBossRoom(roomStructure); // room array, fire votes, ice votes
		InstantiateRoomBorder (roomWidth, roomHeight);
		AstarPath.active.Scan();
	}

	// generate an array containing the information of a cave room in the scene
	RoomTile [,] GenerateBossRoomArray (int width, int height) // height of room, width of room
	{
		RoomTile[,] room = new RoomTile[width, height];

		playerStartPosition = new Vector2 (0.0f, 0.0f); // player spawn location
		Vector2 exitLocation = new Vector2 (10.0f, 10.0f); // room exit location

		// initializing room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				room [i, j].tile = air;
			}
		}

		// set player and exit locations
		player.transform.position = playerStartPosition;
		GameObject tempExit = Instantiate (exit, exitLocation, transform.rotation);
		tempExit.transform.SetParent(dungeonVisual.transform);

		return room;
	}

	void InstantiateBossRoom (RoomTile [,] room) {
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;

		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room [i, j].tile == air) { // set floor tiles
					GameObject floorTile = Instantiate (floorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
					floorTile.transform.SetParent (dungeonVisual.transform);
				}
			}
		}
	}

    // function for generating a cave room
    void GenerateCaveRoom(int roomWidth, int roomHeight, string type)
    {
        // create room
        roomStructure = GenerateCaveRoomArray(roomWidth, roomHeight);
		InstantiateCaveRoom(roomStructure, GameMaster.Instance.fireCount, GameMaster.Instance.iceCount); // room array, fire votes, ice votes
		InstantiateRoomBorder (roomWidth, roomHeight);
		AstarPath.active.Scan();
    }

	// generate an array containing the information of a cave room in the scene
	RoomTile [,] GenerateCaveRoomArray (int width, int height) // height of room, width of room
	{
		RoomTile[,] room = new RoomTile[width, height];

		playerStartPosition = new Vector2 (0.0f, 0.0f); // player spawn location
		Vector2 exitLocation = new Vector2 (0.0f, 0.0f); // room exit location

		bool satisfied; // whether room is satisfied or not

		do {		
			satisfied = true;

			// initializing room and add exterior boundary to room
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					room [i, j].tile = random.NextDouble () < 0.475 ? wall : air;
					room [i, j].tile = (i == 0 || j == 0 || i == width - 1 || j == height - 1) ? wall : room [i, j].tile; // add exterior boundary
				}
			}
				
			// create exits of the room on the opposite sides
			int w = 4; // width of exit
			if (random.NextDouble () < 0.5) {
				bool playerSpawnAtTop = (random.NextDouble () < 0.5)? true : false;

				// create exits on top and bottom of room
				int position = random.Next (1, width - w - 1);
				if (!playerSpawnAtTop) {
					playerStartPosition = new Vector2 (position + w / 2.0f, 0.0f);
				} else {
					exitLocation = new Vector2 (position + w / 2.0f, 0.0f);
				}
				room = FillAirSquare (room, position, -w / 2, w);

				position = random.Next (1, width - w - 1);
				if (playerSpawnAtTop) {
					playerStartPosition = new Vector2 (position + w / 2.0f, height - 1);
				} else {
					exitLocation = new Vector2 (position + w / 2.0f, height - 1);
				}
				room = FillAirSquare (room, position, height - w / 2, w);
			} else {
				bool playerSpawnAtRight = (random.NextDouble () < 0.5)? true : false;

				// create exits on left and right of room
				int position = random.Next (1, height - w - 1);
				if (!playerSpawnAtRight) {
					playerStartPosition = new Vector2 (0.0f, position + w / 2.0f);
				} else {
					exitLocation = new Vector2 (0.0f, position + w / 2.0f);
				}
				room = FillAirSquare (room, -w / 2, position, w);

				position = random.Next (1, height - w - 1);
				if (playerSpawnAtRight) {
					playerStartPosition = new Vector2 (width - 1, position + w / 2.0f);
				} else {
					exitLocation = new Vector2 (width - 1, position + w / 2.0f);
				}
				room = FillAirSquare (room, width - w / 2, position, w);
			}

			// smoothen room
			for (int loopCount = 0; loopCount < 8; loopCount++) { // number of iterations
				RoomTile [,] newRoom = new RoomTile [width, height]; // array for storing the smoothed room

				// loop through each cell
				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {

						// count whether there are more air tiles/wall tiles in the 3x3 area
						int wallCounter = 0;
						for (int row = i - 1; row <= i + 1; row++) {
							for (int col = j - 1; col <= j + 1; col++) {

								// add to counter 
								try {
									wallCounter += room [row, col].tile;
								} catch (IndexOutOfRangeException) {}
							}
						}
						newRoom [i, j].tile = (wallCounter >= 0) ? wall : air; // set the block to wall/air depending on the counter

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
			} while (room [x, y].tile != air);
			room = FloodFill (room, x, y);

			// set flood to air, and others to wall
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					room [i, j].tile = (room [i, j].tile == flood) ? air : wall;
					if (room [i, j].tile == air) {
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
							if (room [i, j].tile == air) {
								airCount += 1;
							}
						}
					}
					satisfied = satisfied && (airCount * 4.0f / width / height >= 0.4f); // at least 40% of each quarter of the room is air
				}
			}
				
		} while (!satisfied);

        // set player and exit locations
		player.transform.position = playerStartPosition;
		GameObject tempExit = Instantiate (exit, exitLocation, transform.rotation);
        tempExit.transform.SetParent(dungeonVisual.transform);

		return room;
	}

	// function for creating a cave room in the scene
	void InstantiateCaveRoom (RoomTile [,] room, int fireVotes, int iceVotes) { // array of the room, number of fire votes, number of ice votes
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;
		float xScale = 0.04f + 1.8f / Mathf.Min(width, height); // x scale of perlin noise
		float xOffset, yOffset; // x and y offset of perlin noise;
		GameObject tempTile; // temporary variable for storing the tile to be created
		GameObject tempEntity; // temporary variable for storing the entity to be created

		// set temperature map of room;
		xOffset = (float) (random.NextDouble ()) * 200;
		yOffset = (float) (random.NextDouble ()) * 200;
		float heightOffset = (fireVotes + iceVotes == 0)? 0.0f : 0.3f * (fireVotes - iceVotes) / ((float) (fireVotes + iceVotes)); // height offset of perlin noise for the temperature map
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				room [i, j].temperature = (float) Mathf.Clamp01(Mathf.Clamp01(Mathf.PerlinNoise (xOffset + i * xScale, yOffset + j * xScale) * 1.5f - 0.25f) * 0.4f + 0.3f + heightOffset);
			}
		}

		// set humidity map of room;
		xOffset = (float) (random.NextDouble ()) * -200 - 400;
		yOffset = (float) (random.NextDouble ()) * -200 - 400;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				room [i, j].humidity = (float) Mathf.Clamp01(Mathf.PerlinNoise (xOffset + i * xScale, yOffset + j * xScale));
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
				if (room [i, j].tile == air) { // set lava/water/floor tiles base on the temperature and humidity
					if (room [i, j].humidity > wetThreshold && room [i, j].temperature < hotThreshold) {
						GameObject setTile; // the tile to be set in the dungeon
						int setTileID; // the id of the tile to be set in the dungeon
						if (room [i, j].temperature < coldThreshold) {
							setTile = iceTiles;
							setTileID = ice;
						} else {
							setTile = waterTiles;
							setTileID = water;
						}
						tempTile = Instantiate(setTile, new Vector3(i, j, 0.0f), transform.rotation);
                        tempTile.transform.SetParent(dungeonVisual.transform);
						room [i, j].tile = setTileID;
					} else if (room [i, j].humidity < dryThreshold && room [i, j].temperature >= hotThreshold) {
						tempTile = Instantiate (lavaTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                        tempTile.transform.SetParent(dungeonVisual.transform);
                        room [i, j].tile = lava;
					} else {
						float hotTileTransparency = Mathf.Clamp01 (room [i, j].temperature * 10 - 5);
						if (hotTileTransparency < 1.0f) {
                            tempTile = Instantiate(floorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                            tempTile.transform.SetParent(dungeonVisual.transform);
                        }
						tempTile = Instantiate (hotFloorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						tempTile.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(room [i, j].temperature * 10 - 5));
						tempTile.transform.SetParent(dungeonVisual.transform);
                    }
				}

				if (room [i, j].tile == wall) { // set the walls and borders of the room
					// see if there are air tiles in the 3x3 area
					bool nearAir = false; // there is air tiles nearby
					for (int row = i - 1; row <= i + 1; row++) {
						for (int col = j - 1; col <= j + 1; col++) {
							try {
								if (room [row, col].tile != wall) {
									nearAir = true;
								}
							} catch (IndexOutOfRangeException) {}
						}
					}

					if (nearAir) {
						// set border tiles
						float hotTileTransparency = Mathf.Clamp01 (room [i, j].temperature * 10 - 5);
						if (hotTileTransparency < 1.0f) {
							GameObject tempBorderTile = Instantiate (borderTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                            tempBorderTile.transform.SetParent(dungeonVisual.transform);
                        }
						tempTile = (GameObject)Instantiate (hotBorderTiles, new Vector3 (i, j, 0.0f), transform.rotation);
						tempTile.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(room [i, j].temperature * 10 - 5));
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
				} while (room [x, y].tile != air || room [x, y].entity != empty || CountAdjacentTiles (room, x, y, wall, 1) <= 3);
				if (CountAdjacentTiles (room, x, y, wall, 2) > 11 && CountAdjacentEntities (room, x, y, loot, 16) == 0) { // if there are more than 11 wall tiles in the 5x5 square area and there is no loot boxes nearby
                                                                                                                 // spawn loot box
                    tempEntity = Instantiate(lootBox, new Vector3 (x, y, 0.0f), transform.rotation);
                    tempEntity.transform.SetParent(dungeonVisual.transform);
                    room [x, y].entity = loot;
					break;
				}
			}
		}

		// spawn large monsters
		for (int spawnCount = 0; spawnCount < width * height / 1536; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 64; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				float distanceToPlayer;
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
					distanceToPlayer = ((new Vector2(x, y)) - playerStartPosition).magnitude;
				} while (room [x, y].tile != air || distanceToPlayer < 10.0f);
				if (CountAdjacentTiles (room, x, y, wall, 4) < 4 && CountAdjacentEntities (room, x, y, large, 12) == 0) { // if there are less than 4 wall tiles in the 9x9 square area, and no large monsters nearby
					// spawn large monster
					tempEntity = (GameObject)Instantiate (largeMob, new Vector3 (x, y, 0.0f), transform.rotation);
                    tempEntity.transform.SetParent(enemyVisual.transform);
                    room [x, y].entity = large;
					break;
				}
			}
		}

		// spawn small monsters
		for (int spawnCount = 0; spawnCount < width * height / 384; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 64; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				float distanceToPlayer;
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
					distanceToPlayer = ((new Vector2(x, y)) - playerStartPosition).magnitude;
				} while (room [x, y].tile != air || distanceToPlayer < 10.0f);
				if (CountAdjacentTiles (room, x, y, wall, 2) < 4 && CountAdjacentEntities (room, x, y, small, 8) == 0 && CountAdjacentEntities (room, x, y, large, 8) == 0) { // if there are less than 4 wall tiles in the 5x5 square area and no mobs nearby
					// spawn small monster
					tempEntity = (GameObject)Instantiate (smallMob, new Vector3 (x, y, 0.0f), transform.rotation);
                    tempEntity.transform.SetParent(dungeonVisual.transform);
                    room [x, y].entity = small;
					int mobCluster = random.Next (3 , 6); // size of mob cluster (3 to 5 inclusive)
					for (int mobCount = 1; mobCount < mobCluster; mobCount++) {
						for (int spawnClusterAttempt = 0; spawnClusterAttempt < 12; spawnClusterAttempt++) {
							int i, j; // x and y coordinates of the room
							while (true) {
								// choose a point within 2 distance from the first small mob in the cluster
								i = random.Next (x - 2, x + 2 + 1);
								j = random.Next (y - 2, y + 2 + 1);
								try {
									if (room [i, j].tile != wall && room [i, j].entity != small && room [i, j].entity != large && room [i, j].entity != loot) {
										break;
									}
								} catch (IndexOutOfRangeException) {}
							}
                            tempEntity = Instantiate(smallMob, new Vector3(i, j, 0.0f), transform.rotation);
                            tempEntity.transform.SetParent(enemyVisual.transform);
                            room [x, y].entity = small;
							break;
						}
					}
					break;
				}
			}
		}
	}

	// flood filling tiles around chosen point
	RoomTile [,] FloodFill (RoomTile[,] room, int i, int j) {
		if (room [i, j].tile == air) {
			room [i, j].tile = flood;
			try {
				room = FloodFill (room, i + 1, j);
				room = FloodFill (room, i - 1, j);
				room = FloodFill (room, i, j + 1);
				room = FloodFill (room, i, j - 1);
			} catch (IndexOutOfRangeException) {}
			return room;
		} else {
			return room;
		}
	}

	// fill the square area with defined size with air, with [x, y] as the bottom left corner of the square
	RoomTile [,] FillAirSquare (RoomTile[,] room, int x, int y, int size) { // room array, x-coordinate, y-coordinate, size of square
		for (int row = x; row <= x + size; row++) {
			for (int col = y; col <= y + size; col++) {
				try {
					room [row, col].tile = air;
				} catch (IndexOutOfRangeException) {}
			}
		}
		return room;
	}

	// count the number of specific tiles in the area of a specific point
	int CountAdjacentTiles (RoomTile [,] room, int x, int y, int tile, int distance) { // room array, x-coordinate, y-coordinate, tile type, maximum distance from the tile
		int tileCounter = 0;
		for (int row = x - distance; row <= x + distance; row++) {
			for (int col = y - distance; col <= y + distance; col++) {
				// add to counter 
				try {
					if (room [row, col].tile == tile) {
						tileCounter += 1;
					}
				} catch (IndexOutOfRangeException) {}
			}
		}
		return tileCounter;
	}

	// count the number of specific tiles in the area of a specific point
	int CountAdjacentEntities (RoomTile [,] room, int x, int y, int entity, int distance) { // room array, x-coordinate, y-coordinate, tile type, maximum distance from the tile
		int tileCounter = 0;
		for (int row = x - distance; row <= x + distance; row++) {
			for (int col = y - distance; col <= y + distance; col++) {
				// add to counter 
				try {
					if (room [row, col].entity == entity) {
						tileCounter += 1;
					}
				} catch (IndexOutOfRangeException) {}
			}
		}
		return tileCounter;
	}

}
