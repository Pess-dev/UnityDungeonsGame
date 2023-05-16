using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayTrigger : MonoBehaviour
{
    AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audioSource.Play();
    }
}
