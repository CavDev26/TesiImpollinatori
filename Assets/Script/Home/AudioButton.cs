using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AudioButton : MonoBehaviour
{
    public AudioMixer EffectsMixer;
    public AudioMixer MusicMixer;
    public UnityEngine.UI.Slider[] sliders;
    public TextMeshProUGUI[] texts;

    public void OnChangeVolume()
    {
        PlayerPrefs.SetFloat("EffectsMixerVolume", 0);
        PlayerPrefs.SetFloat("MusicMixerVolume", 0);
        EffectsMixer.SetFloat("EffectsMixerVolume", -80f);
        MusicMixer.SetFloat("MusicMixerVolume", -80f);
        ResetSliders();
    }
    private void ResetSliders()
    {
        sliders[0].value = 0;
        sliders[1].value = 0;
        texts[0].text = "0";
        texts[1].text = "0";
    }
}
