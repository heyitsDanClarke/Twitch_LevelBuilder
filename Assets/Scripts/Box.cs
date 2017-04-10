using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {

	private Vector2 currPos; // current integer coordinates of the box
	private Vector2 nextPos; // next integer coordinates of the box
	private Vector2 scanPos; // integer coordinates of the box where AstarPath.active.Scan() has been called 
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
		scanPos = currPos;
		direction = new Vector2 (0, 0);

		rb = GetComponentInParent<Rigidbody2D>();
		rb.mass = 1000000.0f; // mass of box
		rb.drag = 0.0f; // drag of boxs
	}
		
	void FixedUpdate () {
		// do A* path scan repeatedly when the box is moving
		Vector2 boxPosition = new Vector2((int) Mathf.Round(transform.position.x), (int) Mathf.Round(transform.position.y));
		if (left || right || down || up) {
			if (boxPosition != scanPos) {
				AstarPath.active.Scan();
				scanPos = boxPosition;
			}
		}

		// does not allow box to wander off the route
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

	void OnCollisionStay2D(Collision2D collision) 
	{
		OnCollisionEnter2D(collision);
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{
		Collider2D collider = collision.collider;

		bool nextLevelMenuActive = false; // is the next level menu active in the scene
		bool pauseMenuActive = false; // is the pause menu active in the scene
		bool deathMenuActive = false; // is the death menu active in the scene
		try {
			nextLevelMenuActive = DungeonUI.Instance.transform.Find ("Next Level Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}
		try {
			pauseMenuActive = DungeonUI.Instance.transform.Find ("Pause Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}
		try {
			deathMenuActive = DungeonUI.Instance.transform.Find ("Death Menu").gameObject.activeSelf;
		} catch (NullReferenceException) {}

		// if player touches the block and the player is holding down the space button
		if(collider.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.Space) && !nextLevelMenuActive && !pauseMenuActive && !deathMenuActive)
		{
			currPos = new Vector2((int) Mathf.Round(transform.position.x), (int) Mathf.Round(transform.position.y));
			Vector3 contactPoint = collider.transform.position; // contact point of the player and the box, PLAYER COLLIDER MUST BE A SQUARE OR CIRCLE

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
					// cannot push block if a wall or another box is in front of the box, or if it is out of the puzzle area
					if (Dungeon.Instance.roomStructure[(int) (nextPos.x + direction.x), (int) (nextPos.y + direction.y)].tile == Dungeon.Instance.wall ||
						Dungeon.Instance.roomStructure[(int) (nextPos.x + direction.x), (int) (nextPos.y + direction.y)].tile == Dungeon.Instance.air ||
						Dungeon.Instance.roomStructure[(int) (nextPos.x + direction.x), (int) (nextPos.y + direction.y)].tile == Dungeon.Instance.water ||
						Dungeon.Instance.roomStructure[(int) (nextPos.x + direction.x), (int) (nextPos.y + direction.y)].tile == Dungeon.Instance.lava ||
						Dungeon.Instance.roomStructure[(int) (nextPos.x + direction.x), (int) (nextPos.y + direction.y)].entity == Dungeon.Instance.box) {
						break;
					}

					// increase move distance of box
					distance += 1;
					nextPos = currPos + direction * distance;

					// stop the box if it is not on an ice tile;
					if (Dungeon.Instance.roomStructure[(int) nextPos.x, (int) nextPos.y].tile != Dungeon.Instance.ice) {
						break;
					}
				} catch (IndexOutOfRangeException) {
					break;
				}
			}

			// add force if box is not moving
			if (rb.velocity.magnitude < Player.Instance.speed / 100.0) {
				rb.AddForce (direction * rb.mass * Player.Instance.speed * 1.2f, ForceMode2D.Impulse);
			}
		}
	}

	// move box to new location
	void moveToNewLocation() {
		// move box to pixel perfect location
		transform.position = new Vector3 (nextPos.x, nextPos.y, transform.position.z);

		// update arrays 
		Dungeon.Instance.roomStructure [(int) currPos.x, (int) currPos.y].entity = Dungeon.Instance.empty;
		Dungeon.Instance.roomStructure [(int) nextPos.x, (int) nextPos.y].entity = Dungeon.Instance.box;

		//update position variables and stop moving the box
		currPos = nextPos;
		direction = new Vector2 (0, 0);
		rb.velocity = new Vector2 (0, 0);
	}
}
