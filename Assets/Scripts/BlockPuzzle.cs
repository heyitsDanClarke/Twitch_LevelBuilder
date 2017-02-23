using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPuzzle : MonoBehaviour
{
	public static BlockPuzzle Instance;

	public int roomWidth = 10; // width of room
	public int roomHeight = 10; // height of room

	[HideInInspector]
	public GameObject puzzleVisual; // dungeon visual

	public GameObject iceTiles; // ice tiles
	public GameObject waterTiles; // water tiles
	public GameObject wallTiles; // wall tiles
	public GameObject floorTiles; // floor tiles
	public GameObject lavaTiles; // lava tiles
	public GameObject boxTile; // box
	public GameObject plateTile; // plate

	// tile IDs, IDs must be different
	[HideInInspector]public int air = 0; // DO NOT MODIFY
	[HideInInspector]public int wall = 1;
	[HideInInspector]public int ice = 2;
	[HideInInspector]public int water = 3;
	[HideInInspector]public int lava = 4;

	// entity IDs, IDs must be different
	[HideInInspector]public int player = -1; // the player
	[HideInInspector]public int empty = 0; // DO NOT MODIFY
	[HideInInspector]public int box = 1;

	// plate IDs, IDs must be different
	//[HideInInspector]public int empty = 0; // COMMENTED OUT, BUT STILL DO NOT MODIFY
	[HideInInspector]public int plate = 1;

	[HideInInspector]public int[,] room; // room structure
	[HideInInspector]public int[,] entities; // entities structure
	[HideInInspector]public int[,] plates; // location of the plates

	private System.Random random; // random numnber generator
	private Vector2 playerStartPos; // player starting position

	// Use this for initialization
	void Start ()
	{
		puzzleVisual = null;

		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		random = new System.Random ();

		// initialize camera
		Camera.main.transform.position = new Vector3(0.0f, Mathf.Tan(Mathf.Deg2Rad * -20.0f) * 20.0f, -20.0f);

		generateRoom(roomWidth, roomHeight, null);

		// set player size and speed
		Player.Instance.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		Player.Instance.GetComponent<Player> ().speed = 2.0f;
	}
		

	// function for generating a room
	void generateRoom(int roomWidth, int roomHeight, string type)
	{
		if (puzzleVisual != null)
		{
			Destroy(puzzleVisual);
		}

		puzzleVisual = new GameObject ();
		puzzleVisual.transform.name = "Puzzle Visual";

		// create room
		createRoom(roomWidth, roomHeight);
		//Poll.Instance.ResetVote(); // reset votes
	}

	// generate a room in the scene
	void createRoom (int width, int height) { // height of room, width of room
		room = new int[width, height];
		plates = new int[width, height];
		entities = new int[width, height];
		int[,] simulateEntities =  new int[width, height]; // entities array for simulation

		bool satisfied; // whether room is satisfied or not

		do {		
			satisfied = true;

			// initializing room structure
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (random.NextDouble() < 0.12) {
						room [x, y] = wall;
					} else if (random.NextDouble() < 0.88) {
						room [x, y] = ice;
					} else {
						room [x, y] = air;
					}
				}
			}

			// place boxes
			for (int count = 0; count < 10; count++) {
				while (true) {
					int x = random.Next (1, width - 1);
					int y = random.Next (1, height - 1);
					if (room [x, y] != wall && entities [x, y] == empty) {
						bool obstacleOnDownOrUp = false;
						bool obstacleOnLeftOrRight = false;

						// determine whether there is obstacles on the left or right of the location
						try {
							obstacleOnLeftOrRight = room [x - 1, y] == wall || entities [x - 1, y] != empty || room [x + 1, y] == wall || entities [x + 1, y] != empty;
						} catch (IndexOutOfRangeException) {
							obstacleOnLeftOrRight = true;
						}

						// determine whether there is obstacles on the bottom or top of the location
						try {
							obstacleOnDownOrUp = room [x, y - 1] == wall || entities [x, y - 1] != empty || room [x, y + 1] == wall || entities [x, y + 1] != empty;
						} catch (IndexOutOfRangeException) {
							obstacleOnDownOrUp = true;
						}

						if (! (obstacleOnLeftOrRight && obstacleOnDownOrUp)) {
							entities [x, y] = box;
							simulateEntities [x, y] = box;
							break;
						}
					}
				}
			}

			// set player final position
			while (true) {
				int x = random.Next (0, width);
				int y = random.Next (0, height);
				if (room [x, y] != wall && entities [x, y] == empty) {
					playerStartPos = new Vector2 (x, y);
					break;
				}
			}

			// DO NOT MODIFY
			Vector2[] directions = {new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1)}; // left, right, down, up

			// forward tracking
			Vector2 simulatePlayerPos = playerStartPos;
			int previousRandomDirection = -1; // DO NOT MODIFY
			for (int count = 0; count < 1000; count++) { // number of iterations for forward tracking simulation
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
					maxSteps = maxDistance (room, simulateEntities, plates, simulatePlayerPos, dir);
					if (maxSteps == 0) {
						canMove[randomDirection] = false;
					} else {
						break;
					}
				}

				previousRandomDirection = randomDirection; // update previous randomly sellected direction
				int randomSteps = random.Next(1, maxSteps + 1); // the number of steps to simulate

				//Debug.Log(simulatePlayerPos.ToString() + " Direction: " + dir.ToString() + " MoveDist: " + randomSteps.ToString() + " MaxDist: " + maxSteps.ToString());

				// simulate player movement
				for (int step = 0; step < randomSteps; step++) {
					push (ref room, ref simulateEntities, ref plates, player, simulatePlayerPos, dir);
					simulatePlayerPos = new Vector2(simulatePlayerPos.x + dir.x, simulatePlayerPos.y + dir.y);
				}

			}

			// place plates
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
					if (simulateEntities [x, y] == box) {
						plates [x, y] = plate;
					}
				}
			}
		} while (!satisfied);

		// reposition player
		Player.Instance.transform.position = new Vector3(playerStartPos.x, playerStartPos.y + 0.05f, Player.Instance.transform.position.z);

		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject tempTile = null;
				if (room [i, j] == air) {
					tempTile = floorTiles;
				} else if (room [i, j] == wall) {
					tempTile = wallTiles;
				} else if (room [i, j] == ice) {
					tempTile = iceTiles;
				} else if (room [i, j] == water) {
					tempTile = waterTiles;
				} else if (room [i, j] == lava) {
					tempTile = lavaTiles;
				}
				tempTile = Instantiate(tempTile, new Vector3(i, j, 0.0f), Quaternion.identity);
				tempTile.transform.SetParent(puzzleVisual.transform);

				GameObject tempEntity = null;
				if (entities [i, j] == box) {
					tempEntity = boxTile;
					tempEntity = Instantiate (tempEntity, new Vector3 (i, j, 0.0f), Quaternion.identity);
					tempEntity.transform.SetParent (puzzleVisual.transform);
				}
				if (plates [i, j] == plate) {
					tempEntity = plateTile;
					tempEntity = Instantiate(tempEntity, new Vector3(i, j, 0.0f), Quaternion.identity);
					tempEntity.transform.SetParent(puzzleVisual.transform);
				}
			}
		}
	}

	// check wether the player can push the entity to a specific direction
	bool canPush (ref int[,] room, ref int[,] entities, ref int[,] plates, int entityType, Vector2 pos, Vector2 dir) { // room array, boxes array, plates array, type of the pushing entity, coordinates of entity, direction vector
		try {
			// get information of the adjacent block 
			int adjacentPosX = (int) (pos.x + dir.x); // x-coordinate of adjacent position
			int adjacentPosY = (int) (pos.y + dir.y); // y-coordinate of adjacent position
			int adjacentTile = room[adjacentPosX, adjacentPosY];
			int adjacentEntity = entities[adjacentPosX, adjacentPosY];
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
					return canPush (ref room, ref entities, ref plates, box, new Vector2 (adjacentPosX, adjacentPosY), dir);
				}
				return false;
			}
			return true; // can push otherwise

		} catch (IndexOutOfRangeException) { // if adjacent block is outside boundary of puzzle
			return false;
		}
	}

	// check wether the entity can push the adjacent entity to a specific direction
	void push (ref int[,] room, ref int[,] entities, ref int[,] plates, int entityType, Vector2 pos, Vector2 dir) { // room array, boxes array, plates array, type of the pushing entity, coordinates of entity, direction vector
		if (canPush (ref room, ref entities, ref plates, entityType, pos, dir)) {
			int adjacentPosX = (int)(pos.x + dir.x); // x-coordinate of adjacent position
			int adjacentPosY = (int)(pos.y + dir.y); // y-coordinate of adjacent position
			int adjacentEntity = entities [adjacentPosX, adjacentPosY];
			if (adjacentEntity == empty) { // done pushing if nothing to push in front
				return; 
			}

			push (ref room, ref entities, ref plates, adjacentEntity, new Vector2 (adjacentPosX, adjacentPosY), dir); // push the other entity in front of the adjacent entity

			int entityPosX = (int)(adjacentPosX + dir.x); // x-coordinate of new position of entity
			int entityPosY = (int)(adjacentPosY + dir.y); // y-coordinate of new position of entity

			// move adjacent entity
			entities [entityPosX, entityPosY] = adjacentEntity;
			entities [adjacentPosX, adjacentPosY] = empty;

			// slide box if it is on ice until it hits another entity
			while (true) {
				try {
					int nextPosX = (int)(entityPosX + dir.x); // x-coordinate of the adjacent sliding position of entity
					int nextPosY = (int)(entityPosY + dir.y); // y-coordinate of the adjacent sliding position of entity

					if (room[entityPosX, entityPosY] == ice && room[nextPosX, nextPosY] != wall && entities[nextPosX, nextPosY] == empty) {
						// slide entity
						entities [nextPosX, nextPosY] = adjacentEntity;
						entities [entityPosX, entityPosY] = empty;

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
	int maxDistance (int[,] room, int[,] entities, int[,] plates, Vector2 playerPos, Vector2 dir) { // room array, boxes array, plates array, coordinates of player, direction vector
		int[,] entitiesClone =	entities.Clone() as int[,]; // deep copy

		int distance = 0;
		while (true) {
			if (canPush (ref room, ref entitiesClone, ref plates, player, playerPos, dir)) {
				push (ref room, ref entitiesClone, ref plates, player, playerPos, dir);
				playerPos = new Vector2 (playerPos.x + dir.x, playerPos.y + dir.y);
				distance += 1;
			} else {
				return distance; // return max distance if player cannot proceed in that direction further
			}
		}
	}
		
}
