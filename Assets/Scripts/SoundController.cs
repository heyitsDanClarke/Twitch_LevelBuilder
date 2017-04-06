using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;
    public float veryLowPitchRange = 0.90f;
    public float veryHighPitchRange = 1.10f;

    public AudioSource efxSource;
    public AudioSource musicSource;
    public AudioSource deathMusicSource;
    public AudioSource lavaSizzleSource;
	public AudioSource mainMenuSettingSource;
    public static SoundController Instance = null;

    void Awake()
    {
		if (Instance != null) {
			Destroy (gameObject);
		} else {
			Instance = this;
		}

    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();

    }


    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }

    public void RandomizeSfxLarge(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(veryLowPitchRange, veryHighPitchRange);
        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }


    // Update is called once per frame

}