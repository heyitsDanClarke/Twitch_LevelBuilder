using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {
    public int speed = 2;
    public float pullRadius = 2;
    public float pullForce = 10;
    // Use this for initialization
    //[HideInInspector]
    public Rigidbody2D rb2d;

    void Start () {

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.mass = 1.0f; // mass of coin
        rb2d.drag = 0.0f; // drag of coin

    }
	
	// Update is called once per frame
	void Update () {
        //Vector3 forceDirection = transform.position - Player.Instance.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, Player.Instance.transform.position, speed * Time.deltaTime);
        //rb2d.AddForce(forceDirection.normalized * pullForce * Time.fixedDeltaTime);
        
    }
}
