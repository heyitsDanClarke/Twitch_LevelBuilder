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
    public float health = 1;
	public int maxHealth = 1;
    public GameObject shard;

    public AudioClip batHit;
	public AudioClip playerHitSound1;
	public AudioClip playerHitSound2;
	public AudioClip playerHitSound3;

    bool swooping = false;
    private Func<Rigidbody2D> treasureObjectRB;

    float originalSeekSpeed;
    float originalSwoopSpeed;

    void Start () {
        _rb = GetComponent<Rigidbody2D>();
        target = Player.Instance.gameObject;
        originalSeekSpeed = seekSpeed;
        originalSwoopSpeed = swoopSpeed;
	}
	
	void Update () {
        bool PauseMenuActive = false; // is the pause menus active in the scene

        try
        {
            PauseMenuActive = DungeonUI.Instance.transform.Find("Pause Menu").gameObject.activeSelf;
        }
        catch (NullReferenceException) { }
        if (!PauseMenuActive)
        {
            if (Vector2.Distance(transform.position, Player.Instance.transform.position) > swoopRange && !swooping)
                _rb.velocity = (target.transform.position - transform.position).normalized * seekSpeed;
            else if (!swooping)
            {
                swooping = true;
                StartCoroutine(Swoop());
            }
        }
        else
        {
            swooping = false;
            StopAllCoroutines();
        }

		if (_rb.velocity.x > 0) {
			transform.localScale = new Vector3 (-1, 1, 1);
			transform.FindChild ("Health Bar").localScale = new Vector3 (-0.3f, 0.3f, 1.0f);
		} else {
			transform.localScale = new Vector3 (1, 1, 1);
			transform.FindChild ("Health Bar").localScale = new Vector3 (0.3f, 0.3f, 1.0f);
		}
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
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            StopCoroutine(Burn());
            StopCoroutine(Freeze());
            seekSpeed = originalSeekSpeed;
            swoopSpeed = originalSwoopSpeed;
            if(Player.Instance.firePower > 0)
            {
                StartCoroutine(Burn());
            }
            if(Player.Instance.icePower > 0)
            {
                originalSeekSpeed = seekSpeed;
                originalSwoopSpeed = swoopSpeed;
                StartCoroutine(Freeze());
            }

            SoundController.Instance.RandomizeSfx(batHit);
			// show health bar of enemy
			transform.FindChild("Health Bar").gameObject.SetActive(true); 

			// damage enemy
			int totalDamage = Mathf.FloorToInt(Player.Instance.baseDamage);
			health -= totalDamage;

            if (health <= 0)
            {
                DestroyEnemy();
            }
            else
            {
				_rb.AddForce((transform.position - coll.transform.position).normalized * _rb.mass * Player.Instance.knockbackMultiplier * 5, ForceMode2D.Impulse);
            }
        }
        else if(coll.gameObject.tag == "Player")
        {
			if (coll.gameObject.tag == "Player") {
				if (Player.Instance.health > 0) {
					Player.Instance.health -= 1;
				}
			}

			Player.Instance.GetComponent<Rigidbody2D>().AddForce((coll.transform.position - transform.position).normalized * coll.gameObject.GetComponent<Rigidbody2D>().mass * 10.0f, ForceMode2D.Impulse);
			SoundController.Instance.RandomizeSfxLarge(playerHitSound1, playerHitSound2, playerHitSound3);
		}
    }

    void DestroyEnemy()
    {
        Vector3 shardPosition = new Vector3(transform.position.x, transform.position.y, 0.0f);

        // prevent shards from spawning inside walls
        while (true)
        {
            int x = Mathf.RoundToInt(shardPosition.x);
            int y = Mathf.RoundToInt(shardPosition.y);
            try
            {
                bool notOnWall = Dungeon.Instance.roomStructure[x, y].tile != Dungeon.Instance.wall; // shard position not on wall
                bool notOnBox = Dungeon.Instance.roomStructure[x, y].entity != Dungeon.Instance.box; // shard position not on boxes
                bool notOnLever = Dungeon.Instance.roomStructure[x, y].entity != Dungeon.Instance.lever; // shard position not on levers
                bool onLeverButAtEdge = Dungeon.Instance.roomStructure[x, y].entity == Dungeon.Instance.lever && Mathf.Abs(shardPosition.x - x) > 0.25f && Mathf.Abs(shardPosition.y - y) > 0.25f; // shard position not inside levers
                if (notOnWall && notOnBox && (notOnLever || onLeverButAtEdge))
                {
                    shardPosition -= (transform.position - Player.Instance.transform.position).normalized * 0.1f;
                    break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                shardPosition = Player.Instance.transform.position;
                break;
            }

            shardPosition -= (transform.position - Player.Instance.transform.position).normalized * 0.05f;
        }

        GameObject treasureObject = Instantiate(shard, shardPosition, Quaternion.identity);
        treasureObject.transform.SetParent(Dungeon.Instance.dungeonVisual.transform);

        Player.Instance.score += 20;

        Destroy(gameObject);
    }

    IEnumerator Burn()
    {
        transform.FindChild("FlamesParticleEffect").gameObject.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.5f);
        health -= Player.Instance.firePower; //TODO: rebalance for different damage values
        if(health <= 0)
        {
            DestroyEnemy();
        }
        yield return new WaitForSeconds(0.5f);
        health -= Player.Instance.firePower;
        if (health <= 0)
        {
            DestroyEnemy();
        }
        transform.FindChild("FlamesParticleEffect").gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    IEnumerator Freeze()
    {
        transform.FindChild("IceParticleEffect").gameObject.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        originalSeekSpeed = seekSpeed;
        seekSpeed -= Player.Instance.icePower;
        originalSwoopSpeed = swoopSpeed;
        seekSpeed -= Player.Instance.icePower;
        yield return new WaitForSeconds(1);
        seekSpeed = originalSeekSpeed;
        seekSpeed = originalSwoopSpeed;
        transform.FindChild("IceParticleEffect").gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
