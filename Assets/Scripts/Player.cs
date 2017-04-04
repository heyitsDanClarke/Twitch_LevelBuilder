﻿using System;
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
	public int maxHealth; // max hit points
	public int health; // current hit points
	public int healthRegeneration; // health regeneration speed
	public int maxCharges; // max charges
	public int charges; // current charges
	public float fireResistance; // current fire resistance
	public float maxFireResistance; // max fire resistance
	public float fireResistanceCooldown; // cooldown for fire resistance meter to regenerate if the player was on lava
	public float maxFireResistanceCooldown; // max cooldown time
	public float fireDamageCooldown; // fire damagee cooldown
	public float maxFireDamageCooldown; // max fire damage cooldown time
	public bool onFire;
	public int coins;
	public int icePower;
	public int firePower;
    public int[] weaponShards = new int[4]; //(0, default) (1, hammer) (2, whip) (3, dagger) 
	public int boxes; // number of boxes pushed to correct places
	public int maxBoxes; // number of boxes in the puzzle
	public int levers; // number of switches left to switch
	public int maxLevers; // number of switches in the puzzle
    public GameObject coin;
	public GameObject gem;
    public AudioClip playerAttackSound;
    public AudioClip playerHitSound1;
    public AudioClip playerHitSound2;
    public AudioClip playerHitSound3;
    public AudioClip exitFoundSound;
    public AudioClip treasureFoundSound;

	//for attack speed
	public float attackRate = 2.0F;
	private float nextAttack = 0.0F;

	//attack collider
	public GameObject attackCollider;

	/* weapon type attribute: 0 - sword ;  1-spear ; 2-polearm ; 3-dagger */
	public int weaponType;

    [HideInInspector]
	public Rigidbody2D rb; // rigid body of playersprite
    [HideInInspector]
    public Animator anim;



	public RuntimeAnimatorController Spear_RAC;
	public RuntimeAnimatorController Polearm_RAC;
	public RuntimeAnimatorController Sword_RAC;
	public RuntimeAnimatorController Dagger_RAC;


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

		//anim.runtimeAnimatorController = Resources.Load ("Assets/Animations/Player/SwordAC") as RuntimeAnimatorController;


		//anim.runtimeAnimatorController = Sword_RAC; 

		rb = GetComponent<Rigidbody2D>();
		rb.mass = 1.0f; // mass of player
		rb.drag = 0.0f; // drag of player
		speed = defaultSpeed;
		acceleration = defaultAcceleration;

		maxHealth = 42;
		maxCharges = 10;
		maxFireResistance = 1.0f;
		maxFireResistanceCooldown = 0.5f;
		maxFireDamageCooldown = 0.5f;
		health = maxHealth;
		fireResistance = maxFireResistance;
		fireDamageCooldown = maxFireDamageCooldown;
		onFire = false;

		weaponType = 0;
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

		
        //HandleMovementAnimations();

		HandleMovement ();
        if (rb.velocity.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (rb.velocity.x > 0)
            transform.localScale = new Vector3(1, 1, 1);

		if (Input.GetKey ("j") && Time.time > nextAttack) {
			//can add switch case for different weapons

			StopAllCoroutines ();
			StartCoroutine (MeleeAttack ());
			nextAttack = Time.time + attackRate;
			SoundController.instance.RandomizeSfxLarge (playerAttackSound);
		} 


		if (Input.GetKey ("r") ) {
			StopAllCoroutines ();
			switch (weaponType) {
			case 0:
				anim.runtimeAnimatorController = Sword_RAC; 
				break;
			case 1:
				anim.runtimeAnimatorController = Spear_RAC; 
				break;
			case 2:
				anim.runtimeAnimatorController = Polearm_RAC; 
				break;
			case 3:
				anim.runtimeAnimatorController = Dagger_RAC; 
				break;
			}

		}
        

	}

	// Update is called once per frame
	void Update ()
	{		
		// update camera position
		Camera.main.transform.position = new Vector3 (transform.position[0], transform.position[1] + Mathf.Tan(Mathf.Deg2Rad * -20.0f) * 20.0f, Camera.main.transform.position[2]);

	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Loot") {
			// show health bar of loot box
			coll.transform.FindChild("Health Bar").gameObject.SetActive(true);

			coll.gameObject.GetComponent<LootBoxController>().health -= 1;

			if (coll.gameObject.GetComponent<LootBoxController>().health <= 0) {
				// spawn gems
				Vector3 boxPosition = coll.gameObject.transform.position;
				bool firstGemOnLeft = Random.Range (0.0f, 1.0f) > 0.5f;
				bool firstGemIsFireGem = Dungeon.Instance.currentFireVotes >= Dungeon.Instance.currentIceVotes; // determine element of first gem
				bool secondGemIsFireGem = Dungeon.Instance.currentFireVotes > Dungeon.Instance.currentIceVotes; // determine element of second gem
				GameObject firstGem = Instantiate (gem, new Vector3(boxPosition.x + (firstGemOnLeft? -0.6f : 0.6f), boxPosition.y, 0.0f), Quaternion.identity); // spawn first gem
				firstGem.GetComponent<GemController> ().firePower = firstGemIsFireGem? 1 : 0;
				firstGem.GetComponent<GemController> ().icePower = firstGemIsFireGem? 0 : 1;
				firstGem.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);
				GameObject secondGem = Instantiate (gem, new Vector3(boxPosition.x + (firstGemOnLeft? 0.6f : -0.6f), boxPosition.y, 0.0f), Quaternion.identity); // spawn second gem
				secondGem.GetComponent<GemController> ().firePower = secondGemIsFireGem? 1 : 0;
				secondGem.GetComponent<GemController> ().icePower = secondGemIsFireGem? 0 : 1;
				secondGem.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);

				// destroy loot box
				Destroy (coll.gameObject);

				AstarPath.active.Scan ();
			}
		}

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
		if (coll.gameObject.tag == "Gem") {
			SoundController.instance.PlaySingle(treasureFoundSound);
			firePower += coll.gameObject.GetComponent<GemController>().firePower;
			icePower += coll.gameObject.GetComponent<GemController>().icePower;
			Destroy(coll.gameObject);
		} else if (coll.gameObject.tag == "Shard") {
			charges += 1;
			int shardType = coll.gameObject.GetComponent<ShardController>().weaponType;

			// activate next weapon panel for 3.0 seconds
			if (charges >= maxCharges) {
				charges = 0;
				PlayerUI.Instance.nextWeaponPanelCountdown = 3.0f;
			}

			Destroy(coll.gameObject);
		} else if (coll.gameObject.tag == "Exit") {
			if (boxes == maxBoxes && levers == maxLevers) { // if all puzzles are being solved
				DungeonUI.Instance.showNextLevelMenu ();
				SoundController.instance.PlaySingle(exitFoundSound);
			}
		} else if (coll.gameObject.CompareTag("Small Monster") || coll.gameObject.CompareTag("Large Monster"))
        {

            if (health > 0)
                health -= 1;
            Vector3 enemyPosition = coll.transform.position;
            //Vector3 coinPosition = transform.position + Random.Range(1.5f, 4.0f) * (enemyPosition - transform.position);
            //Destroy(coll.gameObject);
            //Instantiate(coin, coinPosition, Quaternion.identity);
			rb.AddForce((transform.position - coll.transform.position).normalized * coll.gameObject.GetComponent<Rigidbody2D>().mass * 2.5f, ForceMode2D.Impulse);
            SoundController.instance.RandomizeSfxLarge(playerHitSound1, playerHitSound2, playerHitSound3);

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

	void HandleMovement()
	{
		
		float input_x = Input.GetAxisRaw("Horizontal");
		float input_y = Input.GetAxisRaw("Vertical");

		bool isWalking = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0.01f;

		anim.SetBool ("isWalking", isWalking);

		if (isWalking) {
			anim.SetFloat ("x", input_x);
			anim.SetFloat ("y", input_y);
		}

		if (Input.GetKeyDown (KeyCode.I)) {

			//anim.transform.localScale += new Vector3 (6.3f, 6.3f, 6.3f);
			anim.SetTrigger ("meleeAttack");
		}
	}

    IEnumerator MeleeAttack()
    {

		//initiate the collider and its position is right at the player's position
		GameObject _attackCollider = (GameObject)Instantiate (attackCollider);
		_attackCollider.transform.position = transform.position;

		//Size of the collider (can be changed base on weapon with Switch Case)
		_attackCollider.transform.localScale = new Vector3 (_attackCollider.transform.localScale.x * 2, _attackCollider.transform.localScale.y * 2, _attackCollider.transform.localScale.z);

		//Direction of the attack base on where the character's orientation
		float x = anim.GetFloat ("x");
		float y = anim.GetFloat ("y");

		if (x == 1) {
			_attackCollider.transform.localEulerAngles = new Vector3 (0, 0, 0);
		} else if (x == -1) {
			_attackCollider.transform.localEulerAngles = new Vector3 (0, 0, 180);
		} else if (y == 1) {
			_attackCollider.transform.localEulerAngles = new Vector3 (0, 0, 90);
		} else {
			_attackCollider.transform.localEulerAngles = new Vector3 (0, 0, -90);
		}
			
        transform.GetChild(0).gameObject.SetActive(true);
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(0.25f);
        transform.GetChild(0).gameObject.SetActive(false);
        //charges -= 1;

		//destroy collider, avoid memery leaking
		Destroy (_attackCollider);
    }
}