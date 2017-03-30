using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour {
	public int firePower; // the fire power of the gem
	public int icePower; // the ice power of the gem

    public float speed = 0.5f;

    // Use this for initialization
    void Start () {
		if (Dungeon.Instance.currentIceVotes > Dungeon.Instance.currentFireVotes) {
			firePower = 0;
			icePower = 2;
		} else if (Dungeon.Instance.currentIceVotes < Dungeon.Instance.currentFireVotes) {
			firePower = 2;
			icePower = 0;
		} else {
			firePower = 1;
			icePower = 1;
		}
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, Player.Instance.transform.position, speed * Time.deltaTime);
    }
}
