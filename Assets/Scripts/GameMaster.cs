using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance;

    public int iceCount;
    public int fireCount;

	void Awake () {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }
}
