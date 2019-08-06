using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioSource musicSource;
    // [TO DO]
    // add volume

    public AudioSource soundEffectsSource;
    // [TO DO]
    // add volume

    public void PlaySoundEffect(AudioClip _clip) {
        if(_clip != null) {
            soundEffectsSource?.PlayOneShot(_clip);
        } 
    }
}
