using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class Game3Controller : MonoBehaviour
{
    public static bool isGameOver;
    public GameObject GameScene;
    public GameObject Flowers;
    public GameObject GameOver;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI RecordText;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI SubTitleText;
    public GameObject NewRecordText;

    private ImpollinatoriPerFiore impPerFiore;

    public Sprite[] ImpSpriteArray; //0ape //1bomb //2-3col //4-5farf //6fal //7sirf //8mosc
    public Sprite[] FioreSpriteArray; //0-1ape //2-4bomb // 5-7col // 8-9farf //10fal //11-13sirf //14mos
    public SpriteRenderer[] ImpSprite;
    public SpriteRenderer[] FioreSprite;
    public GameObject[] Boxes;
    public TextMeshProUGUI[] NomiFiori;

    public static float timePassed;
    public static float record;

    public static int[] Accoppiamenti = { -1, -1, -1, -1 };
    private string[] nomiFiori = {
            "Iris", "Ginestrino",  //API
            "Pomodoro", "Robinia", "Linaria", //BOMBI
            "Rosa", "Cisto", "Euforbia", //COLEOTTERI
            "Garofano", "Margherita", //FARFALLA DIURNA
            "Trifoglio", //FALENA
            "Cisto", "Malva", "Sambuco", //SIRFIDE
            "Veronica" //MOSCA --> 15 fiori in totale
        };
    private int[] impollinatori;
    HashSet<int> gruppiUsati = new HashSet<int>();

    private void Start()
    {
        isGameOver = true;
        timePassed = 0;
        record = (float)System.Math.Round(PlayerPrefs.GetFloat("RecordGame3"), 2);
        int[] indiciFiori = FioriInBox();
        impPerFiore = new ImpollinatoriPerFiore(indiciFiori);
        SpawnImpollinatori();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            timePassed += Time.deltaTime;
            if (CheckBoxes())
            {
                OnGameOver();
            }
        }
    }

    private bool CheckBoxes()
    {
        foreach(var box in Boxes)
        {
            if (box.CompareTag("Libero"))
            {
                return false;
            }
        }
        return true;
    }

    private void OnGameOver()
    {
        isGameOver = true;
        int score = 0;
        for(int i = 0; i < Accoppiamenti.Length; i++)
        {
            int gruppoFiore = impPerFiore.GetImpIndice(i);
            int gruppoImp = GetImpIndice(impollinatori[Accoppiamenti[i]]);
            Debug.Log("INDICE=" + i + " / F=" + gruppoFiore + " / I=" + gruppoImp);
            if (gruppoFiore == gruppoImp) score++;
        }
        Debug.Log(score);
        if (score == 4)
        {
            //Tutto corretto, mostro il tempo
            ScoreText.text = System.Math.Round(timePassed, 2).ToString();
            TitleText.text = "COMPLIMENTI";
            if (timePassed < record || record == 0)
            {
                PlayerPrefs.SetFloat("RecordGame3", timePassed);
                RecordText.color = Color.yellow;
                NewRecordText.SetActive(true);
            }
        }
        else
        {
            TitleText.text = "PECCATO";
        }
        SubTitleText.text = score + "/4 CORRETTI";
        RecordText.text = System.Math.Round(PlayerPrefs.GetFloat("RecordGame3"), 2).ToString();


        Flowers.SetActive(false);
        GameScene.SetActive(false);
        GameOver.SetActive(true);
    }

    public static void ImpInBox(int indiceImp, int indiceBox)
    {
        Accoppiamenti.SetValue(indiceImp, indiceBox);
    }

    private int[] FioriInBox()
    {
        int[] indici = new int[4];

        for (int i = 0; i < 4; i++)
        {
            int gruppoIndice;
            do
            {
                gruppoIndice = Random.Range(0, 6);
            }
            while (gruppiUsati.Contains(gruppoIndice));

            switch (gruppoIndice) //0-1ape //2-4bomb // 5-7col // 8-9farf //10fal //11-13sirf //14mos
            {
                case 0:
                    indici[i] = Random.Range(0, 2);
                    gruppiUsati.Add(0);
                    break;
                case 1:
                    indici[i] = Random.Range(2, 5);
                    gruppiUsati.Add(1);
                    break;
                case 2:
                    indici[i] = Random.Range(5, 8);
                    gruppiUsati.Add(2);
                    break;
                case 3:
                    indici[i] = Random.Range(8, 10);
                    gruppiUsati.Add(3);
                    break;
                case 4:
                    indici[i] = 10;
                    gruppiUsati.Add(4);
                    break;
                case 5:
                    indici[i] = Random.Range(11, 14);
                    gruppiUsati.Add(5);
                    break;
                case 6:
                    indici[i] = 14;
                    gruppiUsati.Add(6);
                    break;
            }
            FioreSprite[i].sprite = FioreSpriteArray[indici[i]];
            NomiFiori[i].text = nomiFiori[indici[i]];
        }
        return indici;
    }

    void SpawnImpollinatori()
    {
        impollinatori = new int[4];
        HashSet<int> indiciUsati = new HashSet<int>();
        for (int i = 0; i < 4; i++)
        {
            int gruppoIndice;
            do
            {
                gruppoIndice = Random.Range(0, 4);
            }
            while (indiciUsati.Contains(gruppoIndice));
            indiciUsati.Add(gruppoIndice);
            switch (gruppiUsati.ToArray<int>()[gruppoIndice]) //0ape //1bomb //2-3col //4-5farf //6fal //7sirf //8mosc
            {
                case 0:
                    impollinatori[i] = 0;
                    break;
                case 1:
                    impollinatori[i] = 1;
                    break;
                case 2:
                    impollinatori[i] = Random.Range(2, 4);
                    break;
                case 3:
                    impollinatori[i] = Random.Range(4, 6);
                    break;
                case 4:
                    impollinatori[i] = 6;
                    break;
                case 5:
                    impollinatori[i] = 7;
                    break;
                case 6:
                    impollinatori[i] = 8;
                    break;
            }
            ImpSprite[i].sprite = ImpSpriteArray[impollinatori[i]];
            Debug.Log("Indice " + i + "= impollinatore " + impollinatori[i]);
        }
    }
    private int GetImpIndice(int indice)
    {
        if (indice == 0)
        {
            return 0; //api
        }
        if (indice == 1)
        {
            return 1; //bombi
        }
        if (indice <= 3)
        {
            return 2; //coleotteri
        }
        if (indice <= 5)
        {
            return 3; //farfalle
        }
        if (indice == 6)
        {
            return 4; //falena
        }
        if(indice == 7)
        {
            return 5; //sirfide
        }
        return 6; //mosca
    }

    private class ImpollinatoriPerFiore
    {
        private int[] indici;

        public ImpollinatoriPerFiore(int[] indici)
        {
            this.indici = indici;
        }

        public int GetImpIndice(int indiceBox)
        {
            return GetFioreIndice(indici[indiceBox]);
        }

        private int GetFioreIndice(int indice) //0-1ape //2-4bomb // 5-7col // 8-9farf //10fal //11-13sirf //14mos
        {
            if(indice <= 1)
            {
                return 0; //fiore per le api
            }
            if (indice <= 4)
            {
                return 1; //fiore per le api
            }
            if (indice <= 7)
            {
                return 2; //fiore per i coleotteri
            }
            if( indice <= 9)
            {
                return 3; //fiore per le farfalle
            }
            if(indice <= 10)
            {
                return 4; //fiore per le falene
            }
            if (indice <= 13)
            {
                return 5; //fiore per i sirfidi
            }
            return 6; //fiore per le mosche
        }
    }
}
