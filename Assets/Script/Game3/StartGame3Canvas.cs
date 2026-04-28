using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame3Canvas : MonoBehaviour
{
    public GameObject GameScene;
    public GameObject Flowers;
    float TimeLeft;
    public GameObject GameStartPanel;
    public AudioClip Clip;
    // Start is called before the first frame update
    void Start()
    {
        TimeLeft = 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            onGameStart();
        }
        if (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
        }
        else
        {
            onGameStart();
        }
    }

    void onGameStart()
    {
        Game3Controller.isGameOver = false;
        GameStartPanel.SetActive(false);
        GameScene.SetActive(true);
        Flowers.SetActive(true);
        GameAudioController.PlayClip(Clip);
    }
}
