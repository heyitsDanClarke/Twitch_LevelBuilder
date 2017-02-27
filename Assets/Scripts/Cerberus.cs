﻿using System.Collections;
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
    public float health; //todo: add damage and when health = 0 make an exit

    public GameObject fireball;

    void Start()
    {
        moveReset = Random.Range(1, 3);
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _rb.velocity = ChooseDirection();
    }

    void Update()
    {
        if (!paused)
        {
            counter += Time.deltaTime;
            if (counter >= moveReset)
            {
                StartCoroutine(PauseFire());
                moveLeft = !moveLeft;
                moveReset = Random.Range(1, 3);
                counter = 0;
            }
        }
        else
        {

        }
        if (_rb.velocity.x != 0 || _rb.velocity.y != 0)
            _anim.SetBool("Moving", true);
        else
            _anim.SetBool("Moving", false);

        if (Input.GetKeyDown("j"))
        {
            GameObject tempFire = Instantiate(fireball, transform.position, transform.rotation);
            tempFire.GetComponent<Rigidbody2D>().velocity = Vector2.down;
        }

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
        GameObject tempFire = Instantiate(fireball, new Vector2(transform.position.x, transform.position.y-0.5f), transform.rotation);
        tempFire.GetComponent<Rigidbody2D>().velocity = Player.Instance.transform.position - transform.position;
        _rb.velocity = ChooseDirection();
        paused = false;
    }
}
