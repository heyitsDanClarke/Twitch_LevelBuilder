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
}
