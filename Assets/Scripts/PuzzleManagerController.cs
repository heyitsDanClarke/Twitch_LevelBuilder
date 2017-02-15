using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleManagerController : MonoBehaviour {

	//public GameObject tile;
	public GameObject[] floorTiles;
	public GameObject wall;
    public GameObject gameTile;
    public GameObject goalTile;
    public GameObject player;



    //public int rows;
    //public int columns;
    private int rows = 3;
    private int columns = 3;

    // Use this for initialization
    void Start () {


		for (int i = -1*(columns/2); i < (columns - columns/2); i++) {
			for (int j = -1*(rows/2); j < (rows - rows/2); j++) {
				
				Instantiate (floorTiles[Random.Range(0, floorTiles.Length)], new Vector3 (i, j, 0), Quaternion.identity);
			}
		}


		for (int i = -1 * (columns / 2)-1; i < (columns - columns / 2)+1; i++) {
			Instantiate (wall, new Vector3 (i, -1*(rows/2)-1, 0), Quaternion.identity);
			Instantiate (wall, new Vector3 (i, rows - rows/2, 0), Quaternion.identity);
		}

		for (int j = -1 * (rows / 2); j < (rows - rows / 2); j++) {
			Instantiate (wall, new Vector3 (-1 * (columns / 2)-1, j, 0), Quaternion.identity);
		    Instantiate (wall, new Vector3 ((columns - columns / 2), j, 0), Quaternion.identity);
		}

        Instantiate(gameTile, new Vector3(0, -1, 0), Quaternion.identity);
        Instantiate(gameTile, new Vector3(1, -1, 0), Quaternion.identity);
        Instantiate(gameTile, new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(gameTile, new Vector3(1, 0, 0), Quaternion.identity);
        Instantiate(gameTile, new Vector3(-1, 1, 0), Quaternion.identity);

        Instantiate(goalTile, new Vector3(1, 1, 0), Quaternion.identity);

        Instantiate(player, new Vector3(-1, -1, 0), Quaternion.identity);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
