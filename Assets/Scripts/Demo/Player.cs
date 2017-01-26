using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed = 10;

    public GameObject floor;

	void Update () {
        // change of position of the player
        float horizontalMovement = speed * Time.deltaTime;
        float diagonalMovement = (float)Math.Sqrt(0.5) * speed * Time.deltaTime;

        // move the player according to the key combinations
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += new Vector3(-diagonalMovement, diagonalMovement, 0.0f);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += new Vector3(-diagonalMovement, -diagonalMovement, 0.0f);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += new Vector3(diagonalMovement, -diagonalMovement, 0.0f);
        }
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += new Vector3(diagonalMovement, diagonalMovement, 0.0f);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.position += new Vector3(0.0f, horizontalMovement, 0.0f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.position += new Vector3(0.0f, -horizontalMovement, 0.0f);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += new Vector3(-horizontalMovement, 0.0f, 0.0f);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += new Vector3(horizontalMovement, 0.0f, 0.0f);
        }

        // update camera position
        Camera.main.transform.position = new Vector3(this.transform.position[0], this.transform.position[1], Camera.main.transform.position[2]);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Button")
        {
            for (int i = 0; i < floor.transform.childCount; i++)
            {
                if (GameMaster.Instance.iceCount > GameMaster.Instance.fireCount)
                    floor.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.cyan;
                else if (GameMaster.Instance.fireCount > GameMaster.Instance.iceCount)
                    floor.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.red;
            }
            DemoPoll.Instance.ResetVote();
        }
    }
}
