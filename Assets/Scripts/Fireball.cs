using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
    public GameObject shard;

    void Update () {
        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else if(coll.gameObject.tag == "WeaponCollider")
        {
            Vector3 shardPosition = new Vector3(transform.position.x, transform.position.y, 0.0f);
            GameObject treasureObject = Instantiate(shard, shardPosition, Quaternion.identity);
            treasureObject.transform.SetParent(Dungeon.Instance.dungeonVisual.transform);
            Destroy(gameObject);
        }
        else if(coll.gameObject.tag == "Player")
        {
            if (Player.Instance.health > 0)
                Player.Instance.health -= 1;
            Player.Instance.GetComponent<Rigidbody2D>().AddForce(
                (Player.Instance.transform.position - transform.position).normalized * coll.gameObject.GetComponent<Rigidbody2D>().mass * 1.0f, 
                ForceMode2D.Impulse);
            Destroy(gameObject);
        }
    }
}
