using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {

	public static Player Instance;

	public float speed; // speed of player
	public float acceleration; // acceleration of player;
	[HideInInspector]
	public float defaultSpeed = 6.0f; // default speed of player
	[HideInInspector]
	public float defaultAcceleration = 10.0f; // default acceleration of player
	public int damage; // base damage
	public int health = 8; // base hit points
	public int healthRegeneration; // health regeneration speed
	public int coins;
    public int currentMercuryBladeShardLevel; //
    public int[] weaponShards = new int[4]; //(0, default) (1, hammer) (2, whip) (3, dagger) 
	public int boxes; // number of boxes left to push
	public int levers; // number of switches left to switch
    public GameObject coin;
    public GameObject gem;

    [HideInInspector]
	public Rigidbody2D rb; // rigid body of playersprite
    [HideInInspector]
    public Animator anim;

	void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		rb.mass = 1.0f; // mass of player
		rb.drag = 0.0f; // drag of player
		speed = defaultSpeed;
		acceleration = defaultAcceleration;
        //weaponShards[0] = 0;
        //weaponShards[1] = 1;
        //weaponShards[2] = 2;
        //weaponShards[3] = 3;
	}

    void FixedUpdate ()
	{
		float moveHorizontal = 0.0f;
		float moveVertictal = 0.0f;

		bool menusActive = false; // is there any menus active in the scene

		try {
			if (DungeonUI.Instance != null) {
				foreach (Transform child in DungeonUI.Instance.transform) {
					if (child.gameObject.name == "Next Level Menu" || child.gameObject.name == "Pause Menu")
					menusActive = menusActive || child.gameObject.activeSelf;
				}
			}
		} catch (NullReferenceException) {}


		if (!menusActive) { // if no menus are active
			moveHorizontal = Input.GetAxisRaw ("Horizontal");
			moveVertictal = Input.GetAxisRaw ("Vertical");
		}

		Vector2 targetVelocity = new Vector3 (moveHorizontal, moveVertictal).normalized * speed; // target velocity of player

		Vector2 velocityDifference = (targetVelocity - rb.velocity) * acceleration;
		rb.AddForce (velocityDifference);

		
        HandleMovementAnimations();

        if (Input.GetKey("j"))
        {
            StopAllCoroutines();
            StartCoroutine(MeleeAttack());
        }
        

	}

	// Update is called once per frame
	void Update ()
	{		
		// update camera position
		Camera.main.transform.position = new Vector3 (transform.position[0], transform.position[1] + Mathf.Tan(Mathf.Deg2Rad * -20.0f) * 20.0f, Camera.main.transform.position[2]);

        /*
		float input_x = Input.GetAxisRaw("Horizontal");
		float input_y = Input.GetAxisRaw("Vertical");

		bool isWalking = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;

		anim.SetBool ("isWalking", isWalking);

		if (isWalking) {
			anim.SetFloat ("x", input_x);
			anim.SetFloat ("y", input_y);

			transform.position += new Vector3 (input_x, input_y, 0).normalized * Time.deltaTime;
		}

		if (Input.GetKeyDown (KeyCode.I)) {

			//anim.transform.localScale += new Vector3 (6.3f, 6.3f, 6.3f);
			anim.SetTrigger ("meleeAttack");


		}
        */
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Exit")
		{
			DungeonUI.Instance.showNextLevelMenu ();
		}

		if (coll.gameObject.tag == "Coin") {
			coins += 1;
			Destroy (coll.gameObject);
            
		}
        if (coll.gameObject.tag == "Gem")
        {
            coins += 5;
            Destroy(coll.gameObject);
        }
        if (coll.gameObject.tag == "Loot")
        {
			GameObject treasureObject = Instantiate(gem, coll.gameObject.transform.position, Quaternion.identity);
			treasureObject.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);
            Destroy(coll.gameObject);
        }
        if (coll.gameObject.tag == "Shard")
        {
            currentMercuryBladeShardLevel += 1;
            int shardType = coll.gameObject.GetComponent<ShardController>().weaponType;

            Destroy(coll.gameObject);
        }

        if(coll.gameObject.tag == "Enemy")
        {
            if (health > 0)
                health -= 1;
            Vector3 enemyPosition = coll.transform.position;
            //Vector3 coinPosition = transform.position + Random.Range(1.5f, 4.0f) * (enemyPosition - transform.position);
            //Destroy(coll.gameObject);
            //Instantiate(coin, coinPosition, Quaternion.identity);
            rb.AddForce((transform.position - coll.transform.position).normalized * coll.gameObject.GetComponent<Rigidbody2D>().mass * 2.5f, ForceMode2D.Impulse);
        }

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            if (health > 0)
                health -= 1;
            Vector3 enemyPosition = coll.transform.position;
            //Vector3 coinPosition = transform.position + Random.Range(1.5f, 4.0f) * (enemyPosition - transform.position);
            //Destroy(coll.gameObject);
            //Instantiate(coin, coinPosition, Quaternion.identity);
			rb.AddForce((transform.position - coll.transform.position).normalized * coll.gameObject.GetComponent<Rigidbody2D>().mass * 2.5f, ForceMode2D.Impulse);
        }


    }

    void HandleMovementAnimations()
    {
        if((rb.velocity.x > -0.1 && rb.velocity.x <0.1) && (rb.velocity.y > -0.1 && rb.velocity.y < 0.1))
            anim.SetBool("Moving", false);
        else
            anim.SetBool("Moving", true);
        anim.SetInteger("VerticalMovement", (int)Input.GetAxisRaw("Vertical"));
        anim.SetInteger("HorizontalMovement", (int)Input.GetAxisRaw("Horizontal"));
    }

    IEnumerator MeleeAttack()
    {
        rb.velocity = Vector2.zero;
        transform.GetChild(0).gameObject.SetActive(true);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.25f);
        transform.GetChild(0).gameObject.SetActive(false);
        currentMercuryBladeShardLevel -= 1;
    }
}