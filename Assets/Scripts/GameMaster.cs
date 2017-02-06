using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance;
    [HideInInspector]
    public int iceCount;
    [HideInInspector]
    public int fireCount;

	public float music;
	public float sfx;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	void Start ()
    {
		if (Instance != null) {
			Destroy (this.gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
    }
}
