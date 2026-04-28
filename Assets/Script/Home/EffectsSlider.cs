using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class EffectsSlider : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Slider slider;
    public AudioMixer mixer;
    private void Start()
    {
        slider = GetComponent<Slider>();
        float oldValue = PlayerPrefs.GetFloat("EffectsMixerVolume");
        setNumberText(oldValue);
        slider.value = oldValue;
    }
    public void setNumberText(float newValue)
    {
        text.text = newValue.ToString();
        mixer.SetFloat("EffectsMixerVolume", -80f + (newValue * 0.8f)); //volume = valore volume (da -80 a 20) <-> (da -79 a 21)
        PlayerPrefs.SetFloat("EffectsMixerVolume", newValue); //newValue = valore slide (da 1 a 101)
    }

}
