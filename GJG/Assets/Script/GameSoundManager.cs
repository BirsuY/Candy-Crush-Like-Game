// by Ceren Birsu YILMAZ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource Music;
    private bool isMuted = false;

    // Plays a random destruction noise from the destroyNoise array.
    public void PlayRandom(){
        int clipIndex = Random.Range(0, destroyNoise.Length);
        destroyNoise[clipIndex].Play();
    }

    // Toggles the mute state for both background music and sound effects.
    // When muted, sets the volume to 0; otherwise, restores to defined volume levels.
    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            Music.volume = 0f;
            destroyNoise[0].volume = 0f;
            destroyNoise[1].volume = 0f;
        }
        else
        {
            Music.volume = 1;
            destroyNoise[0].volume = 0.8f;
            destroyNoise[1].volume = 0.8f;
        }

    }

    
}
