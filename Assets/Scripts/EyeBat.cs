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
        else if(!swooping){
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
        _rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(2);
        _rb.velocity = (target.transform.position - transform.position).normalized * swoopSpeed;
        yield return new WaitForSeconds(1);
        swooping = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WeaponCollider")
        {
            SoundController.Instance.RandomizeSfx(batHit);
            health -= 1;
            if (health <= 0)
            {
                Vector3 shardPosition = new Vector3(transform.position.x, transform.position.y, 0.0f);

				// prevent shards from spawning inside walls
				while (true) {
					int x = Mathf.RoundToInt (shardPosition.x);
					int y = Mathf.RoundToInt (shardPosition.y);
					try {
						bool notOnWall = Dungeon.Instance.roomStructure [x, y].tile != Dungeon.Instance.wall; // shard position not on wall
						bool notOnBox = Dungeon.Instance.roomStructure [x, y].entity != Dungeon.Instance.box; // shard position not on boxes
						bool notOnLever = Dungeon.Instance.roomStructure [x, y].entity != Dungeon.Instance.lever; // shard position not on levers
						bool onLeverButAtEdge = Dungeon.Instance.roomStructure[x, y].entity == Dungeon.Instance.lever && Mathf.Abs(shardPosition.x - x) > 0.25f && Mathf.Abs(shardPosition.y - y) > 0.25f; // shard position not inside levers
						if (notOnWall && notOnBox && (notOnLever || onLeverButAtEdge)) {
							shardPosition -= (transform.position - Player.Instance.transform.position).normalized * 0.1f;
							break;
						}
					} catch (IndexOutOfRangeException) {
						break;
					}

					shardPosition -= (transform.position - Player.Instance.transform.position).normalized * 0.05f;
				}

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
