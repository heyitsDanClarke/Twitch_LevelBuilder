using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance;

    public int iceCount;
    public int fireCount;
	public float music;
	public float sfx;

	void Start () {
		if (Instance != null) {
			Destroy (this.gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
    }
}
