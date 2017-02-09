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
            _visual.transform.position = Vector3.MoveTowards(_visual.transform.position, target.transform.position, speed * Time.deltaTime);
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
