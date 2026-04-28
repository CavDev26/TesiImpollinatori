using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGlossarioScript : MonoBehaviour
{
    public int scelta;
    public static int paginaScelta;
    public AudioClip clip;

    public GameObject glossario;
    public GameObject[] pagine;

    private void Start()
    {
        paginaScelta = -1;
    }

    public void onScelta()
    {
        paginaScelta = scelta;
        GameAudioController.PlayClip(clip);
        glossario.SetActive(false);
        pagine[scelta].SetActive(true);
    }
}
