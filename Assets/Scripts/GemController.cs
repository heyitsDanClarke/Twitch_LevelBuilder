using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour {
	public int firePower; // the fire power of the gem
	public int icePower; // the ice power of the gem

	public Sprite fireGemSprite;
	public Sprite iceGemSprite;

    public float speed = 0.5f;

    // Use this for initialization
    void Start () {
		if (firePower == 1) {
			GetComponent<SpriteRenderer>().sprite = fireGemSprite;
		} else {
			GetComponent<SpriteRenderer>().sprite = iceGemSprite;
		}
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, Player.Instance.transform.position, speed * Time.deltaTime);
    }
}
