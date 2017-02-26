using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerberus : MonoBehaviour {

    Rigidbody2D _rb;
    Animator _anim;

    public GameObject fireball;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        _rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
}
