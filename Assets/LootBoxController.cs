using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxController : MonoBehaviour {
    public GameObject gem;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            //Vector3 gemPosition = (transform.position + new Vector3 (0, 0, 5));
            //Instantiate(gem, gemPosition, Quaternion.identity);
        }

    }
}
