using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public AudioMixer Mixer;
    private Slider _slider;
    private void Start()
    {
        _slider = GetComponent<Slider>();
        float oldValue = oldValue = PlayerPrefs.GetFloat("MusicMixerVolume");
        setNumberText(oldValue);
        _slider.value = oldValue;

    }
    public void setNumberText(float newValue)
    {
        Text.text = newValue.ToString();
        Mixer.SetFloat("MusicMixerVolume", -80f + (newValue * 0.8f));
        PlayerPrefs.SetFloat("MusicMixerVolume", newValue);
    }

}
