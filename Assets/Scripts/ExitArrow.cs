using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitArrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// make arrow visual follows the player
		if (Dungeon.Instance.arrowVisual != null) {
			Dungeon.Instance.arrowVisual.transform.position = Player.Instance.transform.position;
			Dungeon.Instance.arrowVisual.transform.rotation = Player.Instance.transform.rotation;
		}

		// get player and exit position
		Vector3 playerPosition = Player.Instance.transform.position;
		Vector3 exitPosition = Dungeon.Instance.exitPosition;

		// set rotation and position of arrow
		transform.localRotation = Quaternion.FromToRotation(Vector3.right, playerPosition - exitPosition); // arrow pointing to exit from player;
		float arrowDistance = rectangleDistance (Camera.main.aspect * 4.8f - 0.8f, 4.8f - 0.8f, transform.eulerAngles.z);
		transform.localPosition = (exitPosition - playerPosition).normalized * arrowDistance;
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -3.0f);

		// make arrow blink if the exit is far from the player and the puzzle is being solved, else hide it
		bool puzzleSolved = Player.Instance.boxes == Player.Instance.maxBoxes && Player.Instance.levers == Player.Instance.maxLevers;
		bool exitFarFromPlayer = (exitPosition - playerPosition).magnitude > arrowDistance * 1.4f;
		if (puzzleSolved && exitFarFromPlayer) {
			GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, Mathf.PingPong (Time.time * 2.5f, 1.0f));
		} else {
			GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
		}

	}

	// returns the distance between the arrow and the player
	float rectangleDistance (float width, float height, float angle) {
		if (Mathf.Abs(Mathf.Tan (Mathf.Deg2Rad * angle)) <= height / width) {
			return width / Mathf.Abs (Mathf.Cos(Mathf.Deg2Rad * angle));
		} else {
			return height / Mathf.Abs (Mathf.Sin(Mathf.Deg2Rad * angle));
		}
	}
}
