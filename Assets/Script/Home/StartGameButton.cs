using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerButton : MonoBehaviour
{
    public int _gameStartScene;
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();  
    }
    public void ChangeScene()
    {
        _audioSource.Play();
        SceneManager.LoadScene(_gameStartScene);
    }
}
