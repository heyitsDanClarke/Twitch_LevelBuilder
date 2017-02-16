using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterAI : MonoBehaviour
{
    public int speed;

    GameObject _visual; //visual component for the monster

    GameObject target;

    void Start ()
    {
        _visual = transform.parent.gameObject;
	}


	void Update ()
    {
		if(target!= null)
        {
			// pause player while pause menu is active
			try {
				if (DungeonUI.Instance != null) {
					if (DungeonUI.Instance.transform.GetChild (1).gameObject.activeSelf) { // if pause menu is active
					} else { // if pause menu is not active
						_visual.transform.position = Vector3.MoveTowards(_visual.transform.position, target.transform.position, speed * Time.deltaTime);
					}
				}
			} catch (NullReferenceException) {}
        }
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            target = coll.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            target = null;
        }
    }
}
