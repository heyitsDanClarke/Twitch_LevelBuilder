using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleManagerController : MonoBehaviour {

	//public GameObject tile;
	public GameObject[] floorTiles;
	public GameObject wall;

	public int rows;
	public int columns;


	// Use this for initialization
	void Start () {


		for (int i = -1*(columns/2); i < (columns - columns/2); i++) {
			for (int j = -1*(rows/2); j < (rows - rows/2); j++) {
				
				Instantiate (floorTiles[Random.Range(0, floorTiles.Length)], new Vector3 (i, j, 0), Quaternion.identity);
			}
		}


		for (int i = -1 * (columns / 2)-1; i < (columns - columns / 2)+1; i++) {
			Instantiate (wall, new Vector3 (i, -1*(rows/2), 0), Quaternion.identity);
			Instantiate (wall, new Vector3 (i, rows - rows/2, 0), Quaternion.identity);
		}

		for (int j = -1 * (rows / 2); j < (rows - rows / 2); j++) {
			Instantiate (wall, new Vector3 (-1 * (columns / 2), j, 0), Quaternion.identity);
			Instantiate (wall, new Vector3 ((columns - columns / 2), j, 0), Quaternion.identity);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
