using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;

public class SaveAudio : MonoBehaviour
{
    public static SaveAudio instance;
    public AudioMixer EffectsMixer;
    public AudioMixer MusicMixer;

    private void Start()
    {
        EffectsMixer.SetFloat("EffectsMixerVolume", -80f + (PlayerPrefs.GetFloat("EffectsMixerVolume") * 0.8f));
        MusicMixer.SetFloat("MusicMixerVolume", -80f + (PlayerPrefs.GetFloat("MusicMixerVolume") * 0.8f));
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(transform.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}
