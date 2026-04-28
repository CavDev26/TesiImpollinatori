using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InsettoBehavior : MonoBehaviour
{
    bool toLeft;
    int impollinatore;
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    public AudioClip[] clips;
    //BELLIS(0) = farfalle diurne, api, mosche
    //SOLANUM(1) = api (inclusi i bombi) e le farfalle
    public int[][] insettiPerFiore = { 
        new int[] { 0, 1, 4, 6 }, 
        new int[] { 0, 1, 2, 4 } 
    };
    // Start is called before the first frame update
    void Start()
    {
        if (transform.position.x == 3.3f)
        {
            transform.Rotate(new Vector3(0, 0, 90));
            toLeft = true;
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, -90));
            toLeft = false;
        }
        impollinatore = UnityEngine.Random.Range(0, 69) % 7;
        //Aglais urticae(0) = Farfalla
        //Apis(1) = Ape
        //Bombus(2) = Ape
        //Cetonia(3) = Coleottero
        //Iphiclides(4) = Farfalla
        //Autographa(5) = Falena
        //Syrphus(6) = Mosca
        spriteRenderer.sprite = spriteArray[impollinatore];
    }

    // Update is called once per frame
    void Update()
    {
        if(!Game1Controller.isGameOver)
        {
            if (toLeft)
            {
                transform.position = new Vector2(
                transform.position.x - 2f * Time.deltaTime,
                transform.position.y);
            }
            else
            {
                transform.position = new Vector2(
                transform.position.x + 2f * Time.deltaTime,
                transform.position.y);
            }
        }
        

        if (transform.position.x < -3.4 || transform.position.x > 3.4 || Game1Controller.isGameOver)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (!Game1Controller.isGameOver)
        {
            if (insettiPerFiore[ChoseFlower.FlowerChosen].Contains(impollinatore))
            {
                OnClickCorretto();
            }
            else
            {
                OnClickErrato();
            }
            
            Destroy(gameObject);
        }
            
    }

    private void OnClickCorretto()
    {
        Game1Controller.score++;
        GameAudioController.PlayClip(clips[0]);
        TimerScript.TimeLeft += 2;   
    }

    private void OnClickErrato()
    {
        GameAudioController.PlayClip(clips[1]);   
        TimerScript.TimeLeft -= 3;
    }
}
