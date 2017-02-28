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
	public GameObject borderTiles; // border tiles
	public GameObject hotBorderTiles; // hot texture of border tiles
	public GameObject wallTiles; // wall tiles
	public GameObject smallMob; // small monster
	public GameObject largeMob; // large monster
	public GameObject lootBox; // loot box
	public GameObject boxTiles; // box
	public GameObject pressurePlateTile; // pressure plate
	public GameObject exit; // exit of room
    public GameObject boss;

	////public Sprite[] spritePlayer = new Sprite[40];

	// tile IDs, IDs must be different
	[HideInInspector]public int air = -1; // DO NOT MODIFY
	[HideInInspector]public int flood = 0;
	[HideInInspector]public int wall = 1; // DO NOT MODIFY
	[HideInInspector]public int ice = 2;
	[HideInInspector]public int water = 3;
	[HideInInspector]public int lava = 4;

	// entity IDs, IDs must be different
	[HideInInspector]public int player = -1; // the player
	[HideInInspector]public int empty = 0; // DO NOT MODIFY
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

		GenerateRandomRoom ();
	}

	void FixedUpdate () {

		// modify player's acceleration and drag
		Player.Instance.acceleration = Player.Instance.defaultAcceleration;
		Player.Instance.rb.drag = 0.0f;
		try {
			int x = (int) Math.Round(Player.Instance.transform.position.x, MidpointRounding.AwayFromZero); // integer x coordinate of player
			int y = (int) Math.Round(Player.Instance.transform.position.y, MidpointRounding.AwayFromZero); // integer x coordinate of player
			if (roomStructure[x, y].tile == ice) {
				Player.Instance.acceleration = 2.0f; // make floor slippery if player is on ice
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
			int RoomType = random.Next (0, 3);
			if (RoomType == 0) {
				roomWidth = 40;
				roomHeight = 40;
				GenerateCaveRoom (roomWidth, roomHeight);
				DungeonUI.Instance.transform.FindChild("Puzzle Panel").gameObject.SetActive(false); // hide panel containing box info
			} else if (RoomType == 1) {
				roomWidth = 25;
				roomHeight = 25;
				GenerateHybridRoom (roomWidth, roomHeight);
				DungeonUI.Instance.transform.FindChild("Puzzle Panel").gameObject.SetActive(true); // show panel containing box info
			} else {
				roomWidth = 12;
				roomHeight = 12;
				GeneratePuzzleRoom (roomWidth, roomHeight);
				DungeonUI.Instance.transform.FindChild("Puzzle Panel").gameObject.SetActive(true); // show panel containing box info
			}
		} else if (roomsLeftUntilBoss == 0) {
			roomWidth = 15;
			roomHeight = 11;
			GenerateBossRoom (roomWidth, roomHeight);
		} else {
			SceneManager.LoadScene(0);
		}
		roomsLeftUntilBoss -= 1;
		AstarPath.active.Scan();
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

	}

	// generate an array containing the information of a cave room in the scene
	RoomTile [,] GenerateBossRoomArray (int width, int height) // height of room, width of room
	{
		RoomTile[,] room = new RoomTile[width, height];

		playerStartPosition = new Vector2 (0.0f, 0.0f); // player spawn location
		Vector2 exitLocation = new Vector2 (9.0f, 9.0f); // room exit location

		// initializing room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				room [i, j].tile = air;
			}
		}

		// set player and exit locations
		Player.Instance.transform.position = playerStartPosition;

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
        Instantiate(boss, new Vector2(width/2, height-2), transform.rotation);
	}

    // function for generating a cave room
    void GenerateCaveRoom(int roomWidth, int roomHeight)
    {
        // create room
		roomStructure = GenerateCaveRoomArray(roomWidth, roomHeight, GameMaster.Instance.fireCount, GameMaster.Instance.iceCount);
		InstantiateCaveRoom(roomStructure);
		InstantiateRoomBorder (roomWidth, roomHeight);

		// spawn chests and monsters
		spawnLootChests (ref roomStructure);
		spawnLargeMonsters (ref roomStructure);
		spawnSmallMonsters (ref roomStructure);

		// clear mobs from room structure array
		RemoveMonstersFromArray (ref roomStructure);
    }

	// generate an array containing the information of a cave room in the scene
	RoomTile [,] GenerateCaveRoomArray (int width, int height, int fireVotes, int iceVotes) // height of room, width of room
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

		float xScale = 0.04f + 1.8f / Mathf.Min(width, height); // x scale of perlin noise
		float xOffset, yOffset; // x and y offset of perlin noise;

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

		// transform air blocks according to the temperature and humidity
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room [i, j].tile == air) { // set lava/water/floor tiles base on the temperature and humidity
					if (room [i, j].humidity > wetThreshold && room [i, j].temperature < coldThreshold) {
						room [i, j].tile = ice;
					} else if (room [i, j].humidity > wetThreshold && room [i, j].temperature < hotThreshold) {
						room [i, j].tile = water;
					} else if (room [i, j].humidity < dryThreshold && room [i, j].temperature >= hotThreshold) {
						room [i, j].tile = lava;
					} else {
						room [i, j].tile = air;
					}
				}
			}
		}

        // set player and exit locations
		Player.Instance.transform.position = playerStartPosition;
		GameObject tempExit = Instantiate (exit, exitLocation, transform.rotation);
        tempExit.transform.SetParent(dungeonVisual.transform);

		return room;
	}

	// function for creating a cave room in the scene
	void InstantiateCaveRoom (RoomTile [,] room) { // array of the room, number of fire votes, number of ice votes
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;
		GameObject tempTile; // temporary variable for storing the tile to be created

		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room [i, j].tile == lava) {
					tempTile = Instantiate (lavaTiles, new Vector3 (i, j, 0.0f), transform.rotation);
					tempTile.transform.SetParent (dungeonVisual.transform);
				} else if (room [i, j].tile == water) {
					tempTile = Instantiate (waterTiles, new Vector3 (i, j, 0.0f), transform.rotation);
					tempTile.transform.SetParent (dungeonVisual.transform);
				} else if (room [i, j].tile == ice) {
					tempTile = Instantiate (iceTiles, new Vector3 (i, j, 0.0f), transform.rotation);
					tempTile.transform.SetParent (dungeonVisual.transform);
				} else if (room [i, j].tile == air) {
					float hotTileTransparency = Mathf.Clamp01 (room [i, j].temperature * 10 - 5);
					if (hotTileTransparency < 1.0f) {
                        tempTile = Instantiate(floorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                        tempTile.transform.SetParent(dungeonVisual.transform);
                    }
					tempTile = Instantiate (hotFloorTiles, new Vector3 (i, j, 0.0f), transform.rotation);
					tempTile.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(room [i, j].temperature * 10 - 5));
					tempTile.transform.SetParent(dungeonVisual.transform);
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
						GameObject tempBorderTile = Instantiate (borderTiles, new Vector3 (i, j, 0.0f), transform.rotation); // cold borders
                        tempBorderTile.transform.SetParent(dungeonVisual.transform);
						tempTile = Instantiate (hotBorderTiles, new Vector3 (i, j, 0.0f), transform.rotation); // hot borders
						tempTile.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, Mathf.Clamp01(room [i, j].temperature * 10 - 5));
                        tempTile.transform.SetParent(dungeonVisual.transform);
                    } else {
                        // set wall tiles
                        tempTile = Instantiate(wallTiles, new Vector3 (i, j, 0.0f), transform.rotation);
                        tempTile.transform.SetParent(dungeonVisual.transform);
                    }
				}

				// place boxes
				if (room [i, j].entity == box) {
					tempTile = Instantiate(boxTiles, new Vector3 (i, j, 0.0f), transform.rotation);
					tempTile.transform.SetParent(dungeonVisual.transform);
				}

				// place plates
				if (room [i, j].plate == plate) {
					tempTile = Instantiate(pressurePlateTile, new Vector3 (i, j, 0.0f), transform.rotation);
					tempTile.transform.SetParent(dungeonVisual.transform);
				}

			}
		}
	}

	// function for generating a hybrid room
	void GenerateHybridRoom(int roomWidth, int roomHeight)
	{
		int puzzleRadius = 4; // smack a 11x11 puzzle to the room

		// lower left corner of puzzle in room
		int lowerX = -1;
		int lowerY = -1;

		// create room
		while (true) {
			roomStructure = GenerateCaveRoomArray (roomWidth, roomHeight, GameMaster.Instance.fireCount, GameMaster.Instance.iceCount);
			for (int findLargeAreaAttempt = 0; findLargeAreaAttempt < 64; findLargeAreaAttempt++) {
				int x = random.Next (puzzleRadius, roomWidth - puzzleRadius);
				int y = random.Next (puzzleRadius, roomHeight - puzzleRadius);
				if (CountAdjacentTiles (roomStructure, x, y, wall, puzzleRadius) < 20) {
					// set lower left corner of puzzle in room
					lowerX = x - puzzleRadius;
					lowerY = y - puzzleRadius;
					break;
				}
			}

			if (lowerX != -1 && lowerY != -1) { // puzzle position in room found
				break;
			}
		}
		RoomTile [,] puzzle = GeneratePuzzleRoomArray (puzzleRadius * 2 + 1, puzzleRadius * 2 + 1, lowerX, lowerY, roomStructure);

		int puzzleWidth = puzzle.GetLength(0); // width of puzzle;
		int puzzleHeight = puzzle.GetLength(1); // height of puzzle;

		// smack puzzle to room
		for (int i = 0; i < puzzleWidth; i++) {
			for (int j = 0; j < puzzleHeight; j++) {
				roomStructure [i + lowerX, j + lowerY].tile = puzzle [i, j].tile;
				roomStructure [i + lowerX, j + lowerY].entity = puzzle [i, j].entity;
				roomStructure [i + lowerX, j + lowerY].plate = puzzle [i, j].plate;
			}
		}

		InstantiateCaveRoom (roomStructure);
		InstantiateRoomBorder (roomWidth, roomHeight);

		// spawn chests and monsters
		spawnLootChests (ref roomStructure);
		spawnLargeMonsters (ref roomStructure);
		spawnSmallMonsters (ref roomStructure);

		// clear mobs from room structure array
		RemoveMonstersFromArray (ref roomStructure);
	}

	// function for generating a puzzle room
	void GeneratePuzzleRoom (int roomWidth, int roomHeight)
	{
		// create room
		roomStructure = GeneratePuzzleRoomArray (roomWidth, roomHeight);
		InstantiatePuzzleRoom (roomStructure);
		InstantiateRoomBorder (roomWidth, roomHeight);
	}

	// generate an array containing the information of a cave room in the scene
	RoomTile[,] GeneratePuzzleRoomArray (int width, int height, int lowerX = 0, int lowerY = 0, RoomTile [,] room = null)
	{
		RoomTile[,] puzzleRoom = new RoomTile[width, height];
		RoomTile[,] simulatePuzzleRoom =  new RoomTile[width, height]; // entities array for simulation
		bool[,] tileIsModified =  new bool[width, height]; // check whether the tiles of the room is being modified or not during the simulation

		bool satisfied; // whether room is satisfied or not

		do {		
			satisfied = true;

			// initializing room structure
			for (int x = 0; x < width - 0; x++) {
				for (int y = 0; y < height - 0; y++) {
					tileIsModified [x, y] = false;
					if (room == null) { // stand-alone puzzle room
						if (random.NextDouble() < 0.1 ) {
							puzzleRoom [x, y].tile = wall;
							simulatePuzzleRoom [x, y].tile = wall;
						} else if (random.NextDouble() < 0.8) {
							puzzleRoom [x, y].tile = ice;
							simulatePuzzleRoom [x, y].tile = ice;
						} else {
							puzzleRoom [x, y].tile = air;
							simulatePuzzleRoom [x, y].tile = air;
						}
					} else { // puzzle for hybrid room
						if (room[x + lowerX, y + lowerY].tile == wall) {
							puzzleRoom [x, y].tile = wall;
							simulatePuzzleRoom [x, y].tile = wall;
						} else if (random.NextDouble() < 0.1) {
							puzzleRoom [x, y].tile = wall;
							simulatePuzzleRoom [x, y].tile = wall;
						} else if (room[x + lowerX, y + lowerY].tile == ice) {
							puzzleRoom [x, y].tile = ice;
							simulatePuzzleRoom [x, y].tile = ice;
						} else if (random.NextDouble() < 0.7) {
							puzzleRoom [x, y].tile = ice;
							simulatePuzzleRoom [x, y].tile = ice;
						} else {
							puzzleRoom [x, y].tile = air;
							simulatePuzzleRoom [x, y].tile = air;
						}
					}
				}
			}

			// place boxes
			for (int count = 0; count < width; count++) {
				int placeBoxAttempt = 0; // limit the number of trials to find a position for the box to prevent an infinite loop
				while (true) {
					placeBoxAttempt += 1;
					if (placeBoxAttempt > width * height * 10) {
						satisfied = false;
						break;
					}
					int x = random.Next (1, width - 1);
					int y = random.Next (1, height - 1);
					if (puzzleRoom [x, y].tile != wall && puzzleRoom [x, y].entity == empty) {
						bool obstacleOnDownOrUp = false;
						bool obstacleOnLeftOrRight = false;

						// determine whether there is obstacles on the left or right of the location
						try {
							obstacleOnLeftOrRight = puzzleRoom [x - 1, y].tile == wall || puzzleRoom [x - 1, y].entity != empty || puzzleRoom [x + 1, y].tile == wall || puzzleRoom [x + 1, y].entity != empty;
						} catch (IndexOutOfRangeException) {
							obstacleOnLeftOrRight = true;
						}

						// determine whether there is obstacles on the bottom or top of the location
						try {
							obstacleOnDownOrUp = puzzleRoom [x, y - 1].tile == wall || puzzleRoom [x, y - 1].entity != empty || puzzleRoom [x, y + 1].tile == wall || puzzleRoom [x, y + 1].entity != empty;
						} catch (IndexOutOfRangeException) {
							obstacleOnDownOrUp = true;
						}

						// ensure that all boxes are pushable in some way 
						if (! (obstacleOnLeftOrRight && obstacleOnDownOrUp)) {
							puzzleRoom [x, y].entity = box;
							simulatePuzzleRoom [x, y].entity = box;
							break;
						}
					}
				}
			}

			// set player initial position
			while (true) {
				int x = random.Next (0, width);
				int y = random.Next (0, height);
				if (puzzleRoom [x, y].tile != wall && puzzleRoom [x, y].entity == empty) {
					playerStartPosition = new Vector2 (x, y);
					break;
				}
			}

			// DO NOT MODIFY
			Vector2[] directions = {new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1)}; // left, right, down, up

			// forward tracking
			Vector2 simulatePlayerPosition = playerStartPosition;
			int previousRandomDirection = -1; // DO NOT MODIFY
			for (int count = 0; count < 9001; count++) { // number of iterations for forward tracking simulation
				bool[] canMove = {true, true, true, true}; // whether the player can move in the 4 directions
				Vector2 dir;
				int maxSteps;
				int randomDirection;

				// choose a random direction that the player can move
				while (true) {
					// the randomly selected direction need to allow the player to move, and cannot be the previous randomly selected direction
					do {
						randomDirection = random.Next(0, 4);
					} while (canMove[randomDirection] == false || randomDirection == previousRandomDirection);
					dir = directions[randomDirection];

					// calculate the number of steps the player can move in that directoin
					maxSteps = maxDistance (simulatePuzzleRoom, simulatePlayerPosition, dir, false);
					if (maxSteps == 0) { // cannot move in that direction if the maximum steps possible in that direction is 0
						canMove[randomDirection] = false;
					} else {
						break; // direction found
					}

					// abort map and regenerate a new one if the player cannot move in all directions
					if (!canMove[0] && !canMove[1] && !canMove[2] && !canMove[3]) {
						satisfied = false;
						break;
					}
				}

				// end simulation if map is not satisfied
				if (!satisfied) {
					break;
				}

				previousRandomDirection = randomDirection; // update previous randomly sellected direction
				int randomSteps = random.Next(1, maxSteps + 1); // the number of steps to simulate: between 1 and maxSteps inclusively

				// simulate player movement
				for (int step = 0; step < randomSteps; step++) {
					push (ref simulatePuzzleRoom, ref tileIsModified, player, simulatePlayerPosition, dir); // push anything in front of the player
					simulatePlayerPosition = new Vector2(simulatePlayerPosition.x + dir.x, simulatePlayerPosition.y + dir.y); // move player by 1 step
				}

			}

			if (satisfied) {
				// place plates
				int totalPlates = 0;
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < width; y++) {
						if (!tileIsModified [x, y] && simulatePuzzleRoom [x, y].entity == box) { // convert unmoved boxes in simulation to walls
							simulatePuzzleRoom [x, y].entity = empty;
							puzzleRoom [x, y].entity = empty;
							puzzleRoom [x, y].tile = wall;
						}

						if (simulatePuzzleRoom [x, y].entity == box) { // place plates according to the final position of boxes during the simulation 
							puzzleRoom [x, y].plate = plate;
							totalPlates += 1;
						}
					}
				}
				Player.Instance.boxes = totalPlates; // set the number of boxes in the dungeon

				satisfied = satisfied && totalPlates >= (width * 2.0/3.0)? true : false; // make sure there are enough boxes in the room
			}

		} while (!satisfied);



		return puzzleRoom;
	}

	// generate a puzzle room in the scene
	void InstantiatePuzzleRoom (RoomTile [,] room) { // height of room, width of room
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;

		// reposition player
		Player.Instance.transform.position = new Vector3(playerStartPosition.x, playerStartPosition.y + 0.05f, Player.Instance.transform.position.z);

		// generate exit
		GameObject tempExit = Instantiate (exit, new Vector2 (0f, 0f), transform.rotation);
		tempExit.transform.SetParent(dungeonVisual.transform);

		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject tempTile = null;
				if (room [i, j].tile == air) {
					tempTile = floorTiles;
				} else if (room [i, j].tile == wall) {
					tempTile = wallTiles;
				} else if (room [i, j].tile == ice) {
					tempTile = iceTiles;
				} else if (room [i, j].tile == water) {
					tempTile = waterTiles;
				} else if (room [i, j].tile == lava) {
					tempTile = lavaTiles;
				}
				tempTile = Instantiate(tempTile, new Vector3(i, j, 0.0f), Quaternion.identity);
				tempTile.transform.SetParent(dungeonVisual.transform);

				GameObject tempEntity = null;
				if (room [i, j].entity == box) {
					tempEntity = boxTiles;
					tempEntity = Instantiate (tempEntity, new Vector3 (i, j, 0.0f), Quaternion.identity);
					tempEntity.transform.SetParent (dungeonVisual.transform);
				}
				if (room [i, j].plate == plate) {
					tempEntity = pressurePlateTile;
					tempEntity = Instantiate(tempEntity, new Vector3(i, j, 0.0f), Quaternion.identity);
					tempEntity.transform.SetParent(dungeonVisual.transform);
				}
			}
		}
	}

	// check wether the player can push the entity to a specific direction
	bool canPush (ref RoomTile[,] room, int entityType, Vector2 pos, Vector2 dir, bool allowBlockOnIceToHitBoundary) { // room array, boxes array, plates array, type of the pushing entity, coordinates of entity, direction vector, whether to allow block on ice to hit boundary of room
		try {
			// get information of the adjacent block 
			int adjacentPosX = (int) (pos.x + dir.x); // x-coordinate of adjacent position
			int adjacentPosY = (int) (pos.y + dir.y); // y-coordinate of adjacent position
			int adjacentTile = room[adjacentPosX, adjacentPosY].tile;
			int adjacentEntity = room[adjacentPosX, adjacentPosY].entity;
			//int adjacentPlates = plates[adjacentPosX, adjacentPosY];

			// check whether it can be pushed or not
			if (adjacentTile == wall) { // cannot push if the next tile is a wall
				return false;
			}
			if (adjacentEntity == box) {
				if (entityType == box) { // cannot push box with another box
					return false;
				}
				if (entityType == player) {
					return canPush (ref room, box, new Vector2 (adjacentPosX, adjacentPosY), dir, allowBlockOnIceToHitBoundary);
				}
				return false;
			}

			if (entityType == box && adjacentTile == ice && adjacentEntity == empty) { // slide box
				try {
					while (true) {
						adjacentPosX = (int) (adjacentPosX + dir.x); // x-coordinate of adjacent position
						adjacentPosY = (int) (adjacentPosY + dir.y); // y-coordinate of adjacent position
						adjacentTile = room[adjacentPosX, adjacentPosY].tile;
						adjacentEntity = room[adjacentPosX, adjacentPosY].entity;
						if (!(adjacentTile == ice && adjacentEntity == empty)) { // end loop if box cannot slide anymore
							return true;
						}
					}
				} catch (IndexOutOfRangeException) { // return allowBlockOnIceToHitBoundary if box slides and hit the boundary of the puzzle
					return allowBlockOnIceToHitBoundary;
				}
			}

			return true; // can push otherwise

		} catch (IndexOutOfRangeException) { // if adjacent block is outside boundary of puzzle
			return false;
		}
	}

	// check wether the entity can push the adjacent entity to a specific direction
	void push (ref RoomTile[,] room, ref bool[,] tileIsModified, int entityType, Vector2 pos, Vector2 dir) { // room array, check modified array, boxes array, plates array, type of the pushing entity, coordinates of entity, direction vector
		if (canPush (ref room, entityType, pos, dir, true)) {
			int adjacentPosX = (int)(pos.x + dir.x); // x-coordinate of adjacent position
			int adjacentPosY = (int)(pos.y + dir.y); // y-coordinate of adjacent position
			int adjacentEntity = room [adjacentPosX, adjacentPosY].entity;
			if (adjacentEntity == empty) { // done pushing if nothing to push in front
				return; 
			}

			push (ref room, ref tileIsModified, adjacentEntity, new Vector2 (adjacentPosX, adjacentPosY), dir); // push the other entity in front of the adjacent entity

			int entityPosX = (int)(adjacentPosX + dir.x); // x-coordinate of new position of entity
			int entityPosY = (int)(adjacentPosY + dir.y); // y-coordinate of new position of entity

			// move adjacent entity
			room [entityPosX, entityPosY].entity = adjacentEntity;
			room [adjacentPosX, adjacentPosY].entity = empty;
			tileIsModified [entityPosX, entityPosY] = true;
			tileIsModified [adjacentPosX, adjacentPosY] = true;

			// slide box if it is on ice until it hits another entity
			while (true) {
				try {
					int nextPosX = (int)(entityPosX + dir.x); // x-coordinate of the adjacent sliding position of entity
					int nextPosY = (int)(entityPosY + dir.y); // y-coordinate of the adjacent sliding position of entity

					if (room[entityPosX, entityPosY].tile == ice && room[nextPosX, nextPosY].tile != wall && room[nextPosX, nextPosY].entity == empty) {
						// slide entity
						room [nextPosX, nextPosY].entity = adjacentEntity;
						room [entityPosX, entityPosY].entity = empty;
						tileIsModified [nextPosX, nextPosY] = true;
						tileIsModified [entityPosX, entityPosY] = true;

						// update position of entity
						entityPosX = nextPosX;
						entityPosY = nextPosY;
					} else { // it is not on ice or it hits another entity
						break;
					}
				} catch (IndexOutOfRangeException) { // if adjacent block is outside boundary of puzzle
					break;
				}
			}
		}
	}

	// calculates the maximum distance a player can walk with the given direction
	int maxDistance (RoomTile[,] room, Vector2 playerPos, Vector2 dir, bool allowBlockOnIceToHitBoundary) { // room array, boxes array, plates array, coordinates of player, direction vector
		RoomTile[,] roomClone =	room.Clone() as RoomTile[,]; // deep copy

		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;
		bool[,] tileIsModified =  new bool[width, height];

		int distance = 0;
		while (true) {
			if (canPush (ref roomClone, player, playerPos, dir, allowBlockOnIceToHitBoundary)) {
				push (ref roomClone, ref tileIsModified, player, playerPos, dir);
				playerPos = new Vector2 (playerPos.x + dir.x, playerPos.y + dir.y);
				distance += 1;
			} else {
				return distance; // return max distance if player cannot proceed in that direction further
			}
		}
	}

	void spawnLootChests (ref RoomTile [,] room) {
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;

		for (int spawnCount = 0; spawnCount < width * height / 625; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 16; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
				} while (room [x, y].tile != air || room [x, y].entity != empty || CountAdjacentTiles (room, x, y, wall, 1) <= 3);
				if (CountAdjacentTiles (room, x, y, wall, 2) > 11 && CountAdjacentEntities (room, x, y, loot, 16) == 0 && CountAdjacentEntities (room, x, y, box, 3) == 0) { // if there are more than 11 wall tiles in the 5x5 square area and there is no boxes/loot chests nearby
					// spawn loot box
					GameObject tempEntity = Instantiate(lootBox, new Vector3 (x, y, 0.0f), transform.rotation);
					tempEntity.transform.SetParent(dungeonVisual.transform);
					room [x, y].entity = loot;
					break;
				}
			}
		}
	}

	void spawnLargeMonsters (ref RoomTile [,] room) {
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;

		for (int spawnCount = 0; spawnCount < width * height / 625; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 128; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				float distanceToPlayer;
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
					distanceToPlayer = ((new Vector2(x, y)) - playerStartPosition).magnitude;
				} while (room [x, y].tile != air || distanceToPlayer < 10.0f);
				if (CountAdjacentTiles (room, x, y, wall, 4) < 4 && CountAdjacentEntities (room, x, y, large, 12) == 0 && CountAdjacentEntities (room, x, y, box, 3) == 0) { // if there are less than 4 wall tiles in the 9x9 square area, and no boxes / large monsters nearby
					// spawn large monster
					GameObject tempEntity = (GameObject)Instantiate (largeMob, new Vector3 (x, y, 0.0f), transform.rotation);
					tempEntity.transform.SetParent(enemyVisual.transform);
					room [x, y].entity = large;
					break;
				}
			}
		}
	}

	void spawnSmallMonsters (ref RoomTile [,] room) {
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;

		for (int spawnCount = 0; spawnCount < width * height / 200; spawnCount++) {
			for (int spawnAttempt = 0; spawnAttempt < 64; spawnAttempt++) {
				int x, y; // x and y coordinates of the room
				float distanceToPlayer;
				do {
					x = random.Next (0, width);
					y = random.Next (0, height);
					distanceToPlayer = ((new Vector2(x, y)) - playerStartPosition).magnitude;
				} while (room [x, y].tile != air || distanceToPlayer < 10.0f);
				if (CountAdjacentTiles (room, x, y, wall, 2) < 4 && CountAdjacentEntities (room, x, y, small, 8) == 0 && CountAdjacentEntities (room, x, y, large, 8) == 0 && CountAdjacentEntities (room, x, y, box, 3) == 0) { // if there are less than 4 wall tiles in the 5x5 square area and no boxes / mobs nearby
					// spawn small monster
					GameObject tempEntity = (GameObject)Instantiate (smallMob, new Vector3 (x, y, 0.0f), transform.rotation);
					tempEntity.transform.SetParent(dungeonVisual.transform);
					room [x, y].entity = small;
					int mobCluster = random.Next (3, 6); // size of mob cluster (3 to 5 inclusive)
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
	
	// clear all monsters from the array
	void RemoveMonstersFromArray (ref RoomTile[,] room) {
		int width = room.GetLength(0); // width of dungeon;
		int height = room.GetLength(1); // height of dungeon;
		
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (room[i, j].entity == large || room[i, j].entity == small) {
					room[i, j].entity = empty;
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

	// count the number of specific entities in the area of a specific point
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
