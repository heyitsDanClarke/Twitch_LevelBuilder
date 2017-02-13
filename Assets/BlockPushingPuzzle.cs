using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPushingPuzzle : MonoBehaviour
{
	public static BlockPushingPuzzle Instance;

	public int roomWidth = 10; // width of room
	public int roomHeight = 10; // height of room

	[HideInInspector]
	public GameObject puzzleVisual; // dungeon visual

	public GameObject emptyObject; // empty GameObject
	public GameObject iceTiles; // ice tiles
	public GameObject waterTiles; // water tiles
	public GameObject wallTiles; // wall tiles
	public GameObject floorTiles; // floor tiles
	public GameObject lavaTiles; // lava tiles
	public GameObject player; // the player
	public GameObject boxTile; // box
	public GameObject plateTile; // plate

	// tile IDs, IDs must be different
	[HideInInspector]public const int air = 0; // DO NOT MODIFY
	[HideInInspector]public const int wall = 1;
	[HideInInspector]public const int ice = 2;
	[HideInInspector]public const int water = 3;
	[HideInInspector]public const int lava = 4;

	// entity IDs, IDs must be different
	[HideInInspector]public const int empty = 0; // DO NOT MODIFY
	[HideInInspector]public const int box = 1;
	[HideInInspector]public const int plate = 2;

	[HideInInspector]public int[,] roomStructure; // room structure

	private System.Random random; // random numnber generator

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

	}


	// function for generating a room
	void generateRoom(int roomWidth, int roomHeight, string type)
	{
		if (puzzleVisual != null)
		{
			Destroy(puzzleVisual);
		}

		puzzleVisual = Instantiate(emptyObject, new Vector3(0, 0, 0), Quaternion.identity);
		puzzleVisual.transform.name = "Puzzle Visual";

		// create room
		createRoom(roomWidth, roomHeight);
		//Poll.Instance.ResetVote(); // reset votes
	}

	// generate a room in the scene
	void createRoom (int width, int height) { // height of room, width of room
		int[,] room = new int[width, height];
		int[,] entities = new int[width, height];

		bool satisfied; // whether room is satisfied or not

		do {		
			satisfied = true;

			// initializing room
			for (int count = 0; count < 20; count++) {
				while (true) {
					int x = random.Next (0, width);
					int y = random.Next (0, height);
					if (room [x, y] == air) {
						room [x, y] = wall;
						break;
					}
				}
				while (true) {
					int x = random.Next (0, width);
					int y = random.Next (0, height);
					if (room [x, y] == air) {
						room [x, y] = ice;
						break;
					}
				}
				while (true) {
					int x = random.Next (0, width);
					int y = random.Next (0, height);
					if (room [x, y] == air) {
						room [x, y] = water;
						break;
					}
				}
				while (true) {
					int x = random.Next (0, width);
					int y = random.Next (0, height);
					if (room [x, y] == air) {
						room [x, y] = lava;
						break;
					}
				}
			}
			for (int count = 0; count < 10; count++) {
				while (true) {
					int x = random.Next (0, width);
					int y = random.Next (0, height);
					if (room [x, y] != wall && entities [x, y] == empty) {
						entities [x, y] = box;
						break;
					}
				}
				while (true) {
					int x = random.Next (0, width);
					int y = random.Next (0, height);
					if (room [x, y] != wall && entities [x, y] == empty) {
						entities [x, y] = plate;
						break;
					}
				}
			}
				
		} while (!satisfied);

		// set player location
		player.transform.position = new Vector3 (-1.0f, -1.0f, 0.0f);



		// create room
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject tempTile = null;
				switch (room [i, j])
				{
				case air:
					tempTile = floorTiles;
					break;
				case wall:
					tempTile = wallTiles;
					break;
				case ice:
					tempTile = iceTiles;
					break;
				case water:
					tempTile = waterTiles;
					break;
				case lava:
					tempTile = lavaTiles;
					break;
				}
				tempTile = Instantiate(tempTile, new Vector3(i, j, 0.0f), Quaternion.identity);
				tempTile.transform.SetParent(puzzleVisual.transform);

				GameObject tempEntity = null;
				switch (entities [i, j])
				{
				case empty:
					break;
				case box:
					tempEntity = boxTile;
					tempEntity = Instantiate(tempEntity, new Vector3(i, j, 0.0f), Quaternion.identity);
					tempEntity.transform.SetParent(puzzleVisual.transform);
					break;
				case plate:
					tempEntity = plateTile;
					tempEntity = Instantiate(tempEntity, new Vector3(i, j, 0.0f), Quaternion.identity);
					tempEntity.transform.SetParent(puzzleVisual.transform);
					break;
				}
			}
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
		
}
