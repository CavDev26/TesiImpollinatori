using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToHomeScript : MonoBehaviour
{
    public int homeScreen;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void HomeScreen()
    {
        audioSource.Play();
        SceneManager.LoadScene(homeScreen);
    }

}
