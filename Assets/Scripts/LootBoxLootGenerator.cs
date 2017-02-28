using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootBoxLootGenerator : MonoBehaviour
{

    public GameObject coin;
    public GameObject gem;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "Player")
        {
            if (Random.Range(0.0f, 1.0f) > 0.8f)
            {
                GameObject treasureObject = Instantiate(coin, transform.position, Quaternion.identity);
				treasureObject.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);
                Object.Destroy(this.gameObject);
            }
            else
            {
				GameObject treasureObject = Instantiate(gem, transform.position, Quaternion.identity);
				treasureObject.transform.SetParent (Dungeon.Instance.dungeonVisual.transform);
                Object.Destroy(this.gameObject);
            }
        }
    }
}
