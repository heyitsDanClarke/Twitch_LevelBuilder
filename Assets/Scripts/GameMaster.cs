using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance;

    [HideInInspector]
    public int iceCount;
    [HideInInspector]
    public int fireCount;

    public int hammerCount;
    public int daggerCount;
    public int whipCount;

	public float music; // music volume
	public float sfx; // sfx volume
	public string username; // twitch username

	void Awake ()
    {
		if (Instance != null) {
			Destroy (gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		music = 0.4f;
		sfx = 0.8f;
		username = "uldrendan";
    }
}