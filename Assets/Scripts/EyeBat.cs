using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBat : MonoBehaviour {

    Rigidbody2D _rb;
    GameObject target;

    public float seekSpeed = 2;
    public float swoopSpeed = 5;
    public int health = 1;
	public int maxHealth = 1;
    public GameObject shard;
	
	void Start () {
        _rb = GetComponent<Rigidbody2D>();
        target = Player.Instance.gameObject;
	}
	
	void Update () {
        _rb.velocity = (target.transform.position - transform.position).normalized * seekSpeed;
        if (_rb.velocity.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        /*
        if(coll.gameObject.tag == "Player" && target == null)
        {
            target = coll.transform;
            seeker.StartPath(transform.position, target.position, OnPathComplete);

            StartCoroutine(UpdatePath());
        }*/ //for demo we are removing the line of sight mechanics, making it a distance check
        if (coll.gameObject.tag == "Sword")
        {
            health -= 1;
            if (health <= 0)
            {
                Vector2 shardPosition = new Vector2(transform.position.x + 1, transform.position.y);
                GameObject treasureObject = Instantiate(shard, shardPosition, Quaternion.identity);
                treasureObject.transform.SetParent(Dungeon.Instance.dungeonVisual.transform);
                Destroy(gameObject);
            }
            else
            {
                _rb.AddForce((transform.position - coll.transform.position).normalized * _rb.mass, ForceMode2D.Impulse);
            }
        }
    }
}
