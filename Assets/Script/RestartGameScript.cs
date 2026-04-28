using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameScript : MonoBehaviour
{
    public int GameScreen;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void gameRestart()
    {
        audioSource.Play();
        SceneManager.LoadScene(GameScreen);
    }
}
