using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance;

    public int iceCount;
    public int fireCount;
	public float roomTemperature;

	public int hammerCount;
	public int daggerCount;
	public int whipCount;
	public int currentWeapon;
	public int nextWeapon;

	[HideInInspector] public int defaultSword = 0;
	[HideInInspector] public int hammer = 1;
	[HideInInspector] public int dagger = 2;
	[HideInInspector] public int whip = 3;


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

	// reset weapon to default
	public void ResetWeapon () {
		currentWeapon = defaultSword;
		nextWeapon = defaultSword;
	}
}