using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBat : MonoBehaviour {

    Rigidbody2D _rb;
    GameObject target;

    public float seekSpeed = 2;
    public float swoopSpeed = 5;
    public float swoopRange = 4;
    public int health = 1;
	public int maxHealth = 1;
    public GameObject shard;
    public AudioClip batHit;

    bool swooping = false;
    private Func<Rigidbody2D> treasureObjectRB;

    void Start () {
        _rb = GetComponent<Rigidbody2D>();
        target = Player.Instance.gameObject;
	}
	
	void Update () {
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) > swoopRange && !swooping)
            _rb.velocity = (target.transform.position - transform.position).normalized * seekSpeed;
        else {
            swooping = true;
            StartCoroutine(Swoop());
        }
        if (_rb.velocity.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
	}

    IEnumerator Swoop()
    {
        yield return new WaitForSeconds(2);
        _rb.velocity = (target.transform.position - transform.position).normalized * swoopSpeed;
        yield return new WaitForSeconds(1);
        swooping = false;
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
        if (coll.gameObject.tag == "WeaponCollider")
        {
            SoundController.instance.RandomizeSfx(batHit);
            health -= 1;
            if (health <= 0)
            {
                Vector2 shardPosition = new Vector2(transform.position.x, transform.position.y);
                GameObject treasureObject = Instantiate(shard, transform.position, Quaternion.identity);
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
