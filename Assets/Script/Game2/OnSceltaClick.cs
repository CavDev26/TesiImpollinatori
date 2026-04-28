using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnSceltaClick : MonoBehaviour
{
    public int indiceScelta;
    public TextMeshProUGUI scelta;
    public AudioClip clip;

    public void onClickScelta()
    {
        Game2Controller.sceltaString = scelta.text;
        Game2Controller.cambioTurno = true;
        GameAudioController.PlayClip(clip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
