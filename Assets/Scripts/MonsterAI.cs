using UnityEngine;
using System;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class MonsterAI : MonoBehaviour
{
    public GameObject shard;
	public GameObject lootBox;
    public AudioClip largeMonsterHitSound;

    // What to chase?
    public Transform target;

    // How many times each second we will update our path
    public float updateRate = 2f;

    // Caching
    Seeker seeker;
    Rigidbody2D rb;

    // The calculated path
    public Path path;

	public float speed = 3f; // The AI's speed per second
	public float acceleration = 20f; // The AI's acceleration

    [HideInInspector]
    public bool pathIsEnded = false;

    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    bool aggressive;

    public int health;
	public int maxHealth;

    float originalSpeed;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

		/*
        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());*/
    }

	void Start() {
		target = GameObject.Find("Player").transform;
		if (target == null)
		{
			return;
		}
        originalSpeed = speed;
	}

    IEnumerator UpdatePath()
    {
        if (target == null)
        {
            //TODO: Insert a player search here.
           //return false;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            //TODO: Insert a player search here.
            return;
        }

        //TODO: Always look at player?

        if (Vector2.Distance(transform.position, target.transform.position) <= 7 && !aggressive)
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);

            StartCoroutine(UpdatePath());
            aggressive = true;
        }

        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;

            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

		bool PauseMenuActive = false; // is the pause menus active in the scene

		try {
			PauseMenuActive = DungeonUI.Instance.transform.Find ("Pause Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}

		Vector2 targetVelocity = new Vector2 (0, 0);
		if (!PauseMenuActive) { // if the pause menu is not active
			targetVelocity = (path.vectorPath [currentWaypoint] - transform.position).normalized * speed; // target velocity of monster
		}

		Vector2 velocityDifference = (targetVelocity - rb.velocity) * acceleration * rb.mass;
		rb.AddForce (velocityDifference);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    void CollisionEnter2D(Collision2D coll)
    {
		if (coll.gameObject.tag == "Player") {
			if (Player.Instance.health > 0) {
				Player.Instance.health -= 1;
			}
		}
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
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            StopCoroutine(Burn());
            StopCoroutine(Freeze());
            if (Player.Instance.firePower > 0)
            {
                StartCoroutine(Burn());
            }
            if (Player.Instance.icePower > 0)
            {
                originalSpeed = speed;
                StartCoroutine(Freeze());
            }
            SoundController.Instance.RandomizeSfxLarge(largeMonsterHitSound);

            // show health bar of enemy
            transform.FindChild("Health Bar").gameObject.SetActive(true); 

			// damage enemy
			int totalDamage = Mathf.FloorToInt(Player.Instance.baseDamage * (1.0f + (Player.Instance.firePower + Player.Instance.icePower) / 10.0f));
			health -= totalDamage;

            if (health <= 0)
			{
				if (CompareTag ("Small Monster")) {
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
							shardPosition = Player.Instance.transform.position;
							break;
						}

						shardPosition -= (transform.position - Player.Instance.transform.position).normalized * 0.05f;
					}

					GameObject treasureObject = Instantiate (shard, shardPosition, Quaternion.identity);
					treasureObject.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);

					Player.Instance.score += 10;
				} else if (CompareTag ("Large Monster")) {

					// spawn loot box
					GameObject treasureObject = Instantiate(lootBox, new Vector3 (transform.position.x, transform.position.y, 0.0f), transform.rotation);
					treasureObject.transform.SetParent(Dungeon.Instance.dungeonVisual.transform);

					AstarPath.active.Scan();

					Player.Instance.score += 50;
				}

                Destroy(gameObject);
            }
            else
            {
				rb.AddForce((transform.position - coll.transform.position).normalized * rb.mass * Player.Instance.knockbackMultiplier * 10, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator Burn()
    {
        transform.FindChild("FlamesParticleEffect").gameObject.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.5f);
        health -= Player.Instance.firePower; //TODO: rebalance for different damage values
        if (health <= 0)
        {
            //DestroyEnemy();
        }
        yield return new WaitForSeconds(0.5f);
        health -= Player.Instance.firePower;
        if (health <= 0)
        {
            //DestroyEnemy();
        }
        transform.FindChild("FlamesParticleEffect").gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator Freeze()
    {
        transform.FindChild("IceParticleEffect").gameObject.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        originalSpeed = speed;
        speed -= Player.Instance.icePower;
        yield return new WaitForSeconds(1);
        speed = originalSpeed;
        transform.FindChild("IceParticleEffect").gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

}
