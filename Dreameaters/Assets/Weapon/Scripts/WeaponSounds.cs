using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponSounds : MonoBehaviour {

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip shotSound;
    [SerializeField]
    private AudioClip cockSound;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
    }

    private void loadClipAndPlay(AudioClip clip) {
        audioSource.clip = clip;
        audioSource.Play();
    }
	
	public void PlayShotSound() {
        loadClipAndPlay(shotSound);
    }

    public void PlayCockSound() {
        loadClipAndPlay(cockSound);
    }
}
