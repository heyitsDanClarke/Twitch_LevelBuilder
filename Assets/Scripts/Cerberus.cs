using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerberus : MonoBehaviour {

    Rigidbody2D _rb;
    Animator _anim;
    float counter;
    int moveReset;
    bool paused;
    bool moveLeft;

    public float speed;
    public float damage;
	public int health = 8;
	public int maxHealth = 8;

    public GameObject fireball;
    public GameObject iceball;

    public GameObject exit;

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
                counter += Time.deltaTime;
                if (counter >= moveReset)
                {
                    StartCoroutine(PauseFire());
                    moveLeft = !moveLeft;
                    moveReset = UnityEngine.Random.Range(1, 3);
                    counter = 0;
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
            x = -1;
        else
            x = 1;
        return new Vector2(x, 0);
    }

    IEnumerator PauseFire()
    {
        _rb.velocity = Vector2.zero;
        paused = true;
        yield return new WaitForSeconds(1);
        GameObject tempFire = Instantiate(fireball, new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), transform.rotation);
        GameObject tempIce = Instantiate(iceball, new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f), transform.rotation);
        tempFire.transform.SetParent (Dungeon.Instance.enemyVisual.transform);
        tempFire.GetComponent<Rigidbody2D>().velocity = (Player.Instance.transform.position - transform.position).normalized * 5;
        tempIce.transform.SetParent(Dungeon.Instance.enemyVisual.transform);
        tempIce.GetComponent<Rigidbody2D>().velocity = (Player.Instance.transform.position - transform.position).normalized * 5;
        _rb.velocity = ChooseDirection();
        paused = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "WeaponCollider")
        {
			// damage enemy
			int totalDamage = Mathf.FloorToInt(Player.Instance.baseDamage * (1.0f + (Player.Instance.firePower + Player.Instance.icePower) / 10.0f));
			health -= totalDamage;

            if (health <= 0)
            {
                GameObject tempExit = Instantiate(exit, transform.position, transform.rotation);
				tempExit.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);

				Player.Instance.score += 1000;

                Destroy(this.gameObject);
               
            }
        }
    }
}
