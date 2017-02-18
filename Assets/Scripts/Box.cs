using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {

	private Vector2 currPos; // current integer coordinates of the box
	private Vector2 nextPos; // next integer coordinates of the box
	private Vector2 direction; // direction vector of the box

	// whether the box is moving at that direction
	private bool left;
	private bool right;
	private bool up;
	private bool down;

	private Rigidbody2D rb; // rigid body of the box

	void Start () {
		left = false;
		right = false;
		up = false;
		down = false;

		currPos = new Vector2((int) Mathf.Round(transform.position.x), (int) Mathf.Round(transform.position.y));
		nextPos = currPos;
		direction = new Vector2 (0, 0);

		rb = GetComponent<Rigidbody2D>();
		rb.mass = 1000000.0f; // mass of box
		rb.drag = 0.0f; // drag of boxs
	}
		
	void FixedUpdate () {
		if (left) {
			transform.position = new Vector3 (transform.position.x, nextPos.y, transform.position.z); // lock y-axis of box
			if (transform.position.x <= nextPos.x) { // if box arrives destination
				moveToNewLocation ();
				left = false;
			}
		} else if (right) {
			transform.position = new Vector3 (transform.position.x, nextPos.y, transform.position.z); // lock y-axis of box
			if (transform.position.x >= nextPos.x) { // if box arrives destination
				moveToNewLocation ();
				right = false;
			}
		} else if (down) {
			transform.position = new Vector3 (nextPos.x, transform.position.y, transform.position.z); // lock x-axis of box
			if (transform.position.y <= nextPos.y) { // if box arrives destination
				moveToNewLocation ();
				down = false;
			}
		} else if (up) {
			transform.position = new Vector3 (nextPos.x, transform.position.y, transform.position.z); // lock x-axis of box
			if (transform.position.y >= nextPos.y) { // if box arrives destination
				moveToNewLocation ();
				up = false;
			}
		} else { // lock box position
			moveToNewLocation ();
		}
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{
		Collider2D collider = collision.collider;

		if(collider.gameObject.CompareTag("Player"))
		{ 
			currPos = new Vector2((int) Mathf.Round(transform.position.x), (int) Mathf.Round(transform.position.y));
			Vector3 contactPoint = collider.transform.position; // contact point of the player and the box, PLAYER COLLIDER MUST BE A SQUARE

			Vector3 boxCenter = transform.position; // center coordinates of the box

			// determine the direction of the collision (collision happens in the opposite direction)
			left = contactPoint.x > boxCenter.x && Mathf.Abs(contactPoint.x - boxCenter.x) > Mathf.Abs(contactPoint.y - boxCenter.y);
			right = contactPoint.x <= boxCenter.x && Mathf.Abs(contactPoint.x - boxCenter.x) > Mathf.Abs(contactPoint.y - boxCenter.y);
			down = contactPoint.y > boxCenter.y && Mathf.Abs(contactPoint.x - boxCenter.x) <= Mathf.Abs(contactPoint.y - boxCenter.y);
			up = contactPoint.y <= boxCenter.y && Mathf.Abs(contactPoint.x - boxCenter.x) <= Mathf.Abs(contactPoint.y - boxCenter.y);

			// move the box in the corresponding direction
			if (left) {
				direction = new Vector2 (-1, 0);
			} else if (right) {
				direction = new Vector2 (1, 0);
			} else if (down) {
				direction = new Vector2 (0, -1);
			} else if (up) {
				direction = new Vector2 (0, 1);
			}

			// find terminating position of box
			int distance = 0; // box travelling distance
			nextPos = currPos;
			while (left || right || down || up) { // while the box can move
				try {
					// cannot push block if a wall or another box is in front of the box
					if (BlockPuzzle.Instance.room[(int) (nextPos.x + direction.x), (int) (nextPos.y + direction.y)] == BlockPuzzle.Instance.wall ||
						BlockPuzzle.Instance.entities[(int) (nextPos.x + direction.x), (int) (nextPos.y + direction.y)] == BlockPuzzle.Instance.box) {
						break;
					}

					// increase move distance of box
					distance += 1;
					nextPos = currPos + direction * distance;

					// stop the box if it is not on an ice tile;
					if (BlockPuzzle.Instance.room[(int) nextPos.x, (int) nextPos.y] != BlockPuzzle.Instance.ice) {
						break;
					}
				} catch (IndexOutOfRangeException) {
					break;
				}
			}

			// add force
			rb.AddForce (direction * rb.mass * 7.2f, ForceMode2D.Impulse);
		}
	}

	// move box to new location
	void moveToNewLocation() {
		// move box to pixel perfect location
		transform.position = new Vector3 (nextPos.x, nextPos.y, transform.position.z);

		// update arrays 
		BlockPuzzle.Instance.entities [(int) currPos.x, (int) currPos.y] = BlockPuzzle.Instance.empty;
		BlockPuzzle.Instance.entities [(int) nextPos.x, (int) nextPos.y] = BlockPuzzle.Instance.box;

		//update position variables and stop moving the box
		currPos = nextPos;
		direction = new Vector2 (0, 0);
		rb.velocity = new Vector2 (0, 0);
	}
}
