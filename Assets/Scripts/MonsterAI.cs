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

		Vector2 velocityDifference = (targetVelocity - rb.velocity) * acceleration;
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
                if (coll.gameObject.tag == "Player")
        {
            if (Player.Instance.health > 0)
                Player.Instance.health -= 1;

            rb.AddForce((coll.transform.position - transform.position).normalized * coll.gameObject.GetComponent<Rigidbody2D>().mass * 2.5f, ForceMode2D.Impulse);
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
        if (coll.gameObject.tag == "Sword")
        {
			// show health bar of enemy
			transform.FindChild("Health Bar").gameObject.SetActive(true);

            health -= 1;
            if (health <= 0)
			{
				if (CompareTag ("Small Monster")) {
					Vector2 shardPosition = new Vector2 (transform.position.x + 1, transform.position.y + 1);
					GameObject treasureObject = Instantiate (shard, shardPosition, Quaternion.identity);
					treasureObject.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);
				} else if (CompareTag ("Large Monster")) {
					// set integer positions of the loot chest
					int x = (int) transform.position.x;
					int y = (int) transform.position.y;

					// spawn loot box
					GameObject treasureObject = Instantiate(lootBox, new Vector3 (x, y, 0.0f), transform.rotation);
					treasureObject.transform.SetParent(Dungeon.Instance.dungeonVisual.transform);

					AstarPath.active.Scan();
				}

                Destroy(gameObject);
            }
            else
            {
				rb.AddForce((transform.position - coll.transform.position).normalized * rb.mass, ForceMode2D.Impulse);
            }
        }
    }

}
