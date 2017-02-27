using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

	void Update () {
        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else if(coll.gameObject.tag == "Player")
        {
            Player.Instance.health -= 1;
            Destroy(gameObject);
        }
    }
}
