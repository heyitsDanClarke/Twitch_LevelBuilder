using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {

    public GameObject _wall;
    public GameObject _tile;
    public GameObject _button;
    public int x = 5;
    public int y = 5;

	void Start()
    {
        Instantiate(_button, new Vector3(3, 3, 0), transform.rotation);
        Instantiate(_wall, new Vector3(-1, -1, 0), transform.rotation);
        Instantiate(_wall, new Vector3(x + 1, -1, 0), transform.rotation);
        Instantiate(_wall, new Vector3(-1, y + 1, 0), transform.rotation);
        Instantiate(_wall, new Vector3(x + 1, y + 1, 0), transform.rotation);
        for (int i = 0; i<= x; i++)
        {
            Instantiate(_wall, new Vector3(i, -1, 0), transform.rotation);
            Instantiate(_wall, new Vector3(i, y + 1, 0), transform.rotation);
            for (int j = 0; j <= y; j++)
            {
                if (i == 0)
                {
                    Instantiate(_wall, new Vector3(-1, j, 0), transform.rotation);
                    Instantiate(_wall, new Vector3(x + 1, j, 0), transform.rotation);
                }
                    GameObject tileTemp = Instantiate(_tile, new Vector3(i, j, 0), transform.rotation);
                tileTemp.transform.SetParent(transform);
            }
        }
    }
}
