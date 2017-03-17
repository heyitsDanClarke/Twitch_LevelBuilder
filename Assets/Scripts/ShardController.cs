using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShardController : MonoBehaviour {
    public int weaponType; //(0, default) (1, hammer) (2, whip) (3, dagger)
	// Use this for initialization
	void Start () {
        weaponType = (int)Random.Range(0, 4.0f);
        


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
