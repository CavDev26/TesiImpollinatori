using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioController : MonoBehaviour
{
    static AudioSource mixer;
    void Start()
    {
        mixer = GetComponent<AudioSource>();
    }

    public static void PlayClip(AudioClip clip)
    {
        mixer.PlayOneShot(clip);
    }
}
