using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerberus : MonoBehaviour {

    Rigidbody2D _rb;
    Animator _anim;
    float counter;
    float spawnCounter;
    int moveReset;
    bool paused;
    bool moveLeft;

	float originalSpeed;
    public float speed;
    public float damage;
	public int health = 8;
	public int maxHealth = 8;

    public GameObject fireball;
    public GameObject iceball;

    public GameObject exit;

    public GameObject eyebat;

    void Start()
    {
		moveReset = UnityEngine.Random.Range(1, 3);
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _rb.velocity = ChooseDirection();
    }

    void Update()
    {
        bool PauseMenuActive = false; // is the pause menus active in the scene

        try
        {
            PauseMenuActive = DungeonUI.Instance.transform.Find("Pause Menu").gameObject.activeSelf;
        }
        catch (NullReferenceException) { }

        if (!PauseMenuActive)
        {
            if (!paused)
            {
                spawnCounter += Time.deltaTime;
                counter += Time.deltaTime;
                if (counter >= moveReset)
                {
                    StartCoroutine(PauseFire());
                    moveLeft = !moveLeft;
                    moveReset = UnityEngine.Random.Range(1, 3);
                    counter = 0;
                }
                if(spawnCounter >= 4)
                {
                        Instantiate(eyebat, new Vector3(0, 2, 0), transform.rotation);
                        Instantiate(eyebat, new Vector3(15, 2, 0), transform.rotation);
                    spawnCounter = 0;
                }
            }
        }
        else
        {
            paused = false;
            StopAllCoroutines();
        }
        if (_rb.velocity.x != 0 || _rb.velocity.y != 0)
            _anim.SetBool("Moving", true);
        else
            _anim.SetBool("Moving", false);

    }

    Vector2 ChooseDirection()
    {
        float x;
        if (moveLeft)
			x = -speed;
        else
			x = speed;
        return new Vector2(x, 0);
    }

    IEnumerator PauseFire()
    {
        _rb.velocity = Vector2.zero;
        paused = true;
        yield return new WaitForSeconds(0.5f);
        GameObject tempFire = Instantiate(fireball, new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), transform.rotation);
        GameObject tempIce = Instantiate(iceball, new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f), transform.rotation);
        tempFire.transform.SetParent (Dungeon.Instance.enemyVisual.transform);
        tempFire.GetComponent<Rigidbody2D>().velocity = (Player.Instance.transform.position - transform.position).normalized * 5;
        tempIce.transform.SetParent(Dungeon.Instance.enemyVisual.transform);
        tempIce.GetComponent<Rigidbody2D>().velocity = (Player.Instance.transform.position - transform.position).normalized * 5;
        _rb.velocity = ChooseDirection();
        paused = false;
    }

	void DestroyEnemy () {
		GameObject tempExit = Instantiate(exit, transform.position, transform.rotation);
		tempExit.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);

		Player.Instance.score += 1000;

		Destroy(this.gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "WeaponCollider")
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

			// damage enemy
			int totalDamage = Mathf.FloorToInt(Player.Instance.baseDamage * (1.0f + (Player.Instance.firePower + Player.Instance.icePower) / 20.0f));
			health -= totalDamage;

            if (health <= 0)
            {
				DestroyEnemy ();               
            }
        }
    }

	IEnumerator Burn()
	{
		transform.FindChild("FlamesParticleEffect").gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		health -= Player.Instance.firePower * 5;
		if (health <= 0)
		{
			DestroyEnemy ();
		}
		yield return new WaitForSeconds(0.5f);
		health -= Player.Instance.firePower * 5;
		if (health <= 0)
		{
			DestroyEnemy ();
		}
		transform.FindChild("FlamesParticleEffect").gameObject.SetActive(false);
	}

	IEnumerator Freeze()
	{
		transform.FindChild("IceParticleEffect").gameObject.SetActive(true);
		float slowFactor = Mathf.Max(1 - Player.Instance.icePower / 10.0f, 0);
		gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp (Color.blue, Color.white, slowFactor);
		originalSpeed = speed;
		speed *= slowFactor;
		yield return new WaitForSeconds(1);
		speed = originalSpeed;
		transform.FindChild("IceParticleEffect").gameObject.SetActive(false);
		gameObject.GetComponent<SpriteRenderer>().color = Color.white;
	}
}
