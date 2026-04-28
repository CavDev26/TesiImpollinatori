using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoseFlower : MonoBehaviour
{
    public int flowerChosen;
    public static int FlowerChosen;
    public GameObject startGame;

    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;

    public static int record;
    public TextMeshProUGUI RecordText;

    public AudioClip clip;

    public void OnFlowerChosen()
    {
        GameAudioController.PlayClip(clip);
        FlowerChosen = flowerChosen;
        Game1Controller.isGameOver = false;
        TimerScript.TimerOn = true;
        startGame.SetActive(false);
        spriteRenderer.sprite = spriteArray[FlowerChosen];
        if(flowerChosen == 0)
        {
            record = Game1Controller.record0;
        } else
        {
            record = Game1Controller.record1;
        }
        RecordText.text = record.ToString();
    }
}
