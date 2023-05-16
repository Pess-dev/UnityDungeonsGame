using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    [SerializeField]
    private List<Sound> Sounds = new List<Sound>();

    [System.Serializable]
    public class Sound
    {
        public List<AudioClip> SoundVariants;
    }

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int id)
    {
        PlayRandom(Sounds[id].SoundVariants);
    }
    public void PlaySoundAtPoint(int id)
    {
        PlayRandom(Sounds[id].SoundVariants, true);
    }

    private void PlayRandom(List<AudioClip> variants, bool atPoint = false)
    {
        if (variants.Count == 0 || audioSource == null)
            return;
        int random = Random.Range(0,variants.Count);

        if (atPoint)
            AudioSource.PlayClipAtPoint(variants[random], transform.position);
        else
        {
            audioSource.Stop();
            audioSource.clip = variants[random];
            audioSource.Play();
        }
    }
}
