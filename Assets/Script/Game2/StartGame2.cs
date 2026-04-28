using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartGame2 : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    public static int impollinatore;
    public TextMeshProUGUI NomeImpoll;
    float TimeLeft;
    public GameObject gameStartPanel;
    public AudioClip clip;

    public SpriteRenderer spriteRendererRecap;
    public TextMeshProUGUI NomeImpollRecap;
    public SpriteRenderer spriteRendererGameOver;
    public TextMeshProUGUI NomeImpollGameOver;

    public GameObject TurnCanvas;
    public GameObject RecapCanvas;
    // Start is called before the first frame update
    void Start()
    {
        impollinatore = Random.Range(0, 69) % 8;
        //Aglais urticae(0) = Farfalla
        //Iphiclides(1) = Farfalla

        //Apis(2) = Ape
        //Bombus(3) = Bombo

        //Cetonia(4) = Coleottero

        //Autographa(5) = Falena

        //Syrphus(6) = Sirfide

        //Bomilius(7) = Bombilide
        spriteRenderer.sprite = spriteArray[impollinatore];
        switch (impollinatore)
        {
            case 0:
                NomeImpoll.text = "Farfalle";
                break;
            case 1:
                NomeImpoll.text = "Farfalle";
                break;
            case 2:
                NomeImpoll.text = "Api da miele";
                break;
            case 3:
                NomeImpoll.text = "Bombi";
                break;
            case 4:
                NomeImpoll.text = "Coleotteri";
                break;
            case 5:
                NomeImpoll.text = "Falene";
                break;
            case 6:
                NomeImpoll.text = "Mosche sirfidi";
                break;
            case 7:
                NomeImpoll.text = "Mosche bombilidi";
                break;
        }
        TimeLeft = 12;
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
        Game2Controller.isGameOver = false;
        Game2Controller.cambioTurno = true;
        gameStartPanel.SetActive(false);
        TurnCanvas.SetActive(true);
        RecapCanvas.SetActive(true);
        GameAudioController.PlayClip(clip);
        SetupRecap();
    }

    private void SetupRecap()
    {
        spriteRendererRecap.sprite = spriteArray[impollinatore];
        NomeImpollRecap.text = NomeImpoll.text;

        spriteRendererGameOver.sprite = spriteArray[impollinatore];
        NomeImpollGameOver.text = NomeImpoll.text;
    }
}
