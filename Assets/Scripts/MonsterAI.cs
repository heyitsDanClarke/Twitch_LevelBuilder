using UnityEngine;
using System;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class MonsterAI : MonoBehaviour
{
    public GameObject coin;

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
	public float acceleration = 4f; // The AI's acceleration

    [HideInInspector]
    public bool pathIsEnded = false;

    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        /*target = GameObject.Find("Player").transform;

        if (target == null)
        {
            return;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());*/
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

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Player" && target == null)
        {
            target = coll.transform;
            seeker.StartPath(transform.position, target.position, OnPathComplete);

            StartCoroutine(UpdatePath());
        }
        else if (coll.gameObject.tag == "Sword")
        {
            Vector2 coinPosition = new Vector2(transform.position.x + 1, transform.position.y);
            Instantiate(coin, coinPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
