using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Game2Controller : MonoBehaviour
{
    public static bool isGameOver = true;
    public static bool cambioTurno = false;
    public static string sceltaString = "";
    Dictionary<int, string[]> proposte = new Dictionary<int, string[]>();
    Dictionary<int, string[]> risposteCorrette = new Dictionary<int, string[]>();
    static string[] colori = { "Soprattutto blu, viola, giallo", "Verde o bianco", "Brillante", "Spesso bianco o pallido", "Luminoso", "Indifferente" };
    static string[] profumi = { "Fresco, intenso", "Vario, intenso", "Fresco, delicato", "Dolce, intenso", "Debole" };
    static string[] periodi = { "Giorno", "Giorno o notte", "Notte" };
    static string[] corolle = { "Zigomorfa", "Aperta", "Piattaforma di atterraggio", "Profonda", "Attinomorfa", "Racchiusa" };
    static string[] ricompense = { "Nettare e/o polline", "Solo nettare", "Solo polline" };
    string[] turni = { "Colore", "Profumo", "Periodo di fioritura", "Corolla", "Ricompensa all'impollinatore", "" };

    int impollinatore;
    int turno;
    int scelte;

    public TextMeshProUGUI[] textButtons;
    public TextMeshProUGUI[] recaps;
    public TextMeshProUGUI turnoText;

    public GameObject TurnCanvas;
    public GameObject RecapCanvas;
    public GameObject GameOverCanvas;

    public TextMeshProUGUI score;
    public SpriteRenderer[] spriteRenderers;
    public Sprite[] spriteArray;
    Dictionary<int, int[]> fioriPerImpoll = new Dictionary<int, int[]>();
    //Farfalle
    static int[] fiori0 = { 0, 8, 3 }; //Margherita // Primula // Garofano
    static int[] fiori1 = { 0, 8, 3 }; //Margherita // Primula // Garofano
    //Api
    static int[] fiori2 = { 6, 4, 1 }; //Iris // Salvia // Ginestrino
    //Bombi
    static int[] fiori3 = { 11, 5, 7 }; // Pomodoro // Robinia // Linaria
    //Coleotteri
    static int[] fiori4 = { 10, 2, 14 }; //Rosa // Cisto // Euforbia 
    //Falene
    static int[] fiori5 = { 4, 12 }; //Salvia // Trifoglio
    //Sirfidi
    static int[] fiori6 = { 2, 9, 13 }; // Cisto // Malva // Sambuco
    //Bombilidi
    static int[] fiori7 = { 4, 15, 8 }; // Salvia // Veronica // Primula
    public TextMeshProUGUI[] nomeFioriText;
    string[] nomeFiori = { "Margherita", "Ginestrino", "Cisto", "Garofano", "Salvia", "Robinia", "Iris", "Linaria", "Primula",
        "Malva", "Rosa", "Pomodoro", "Trifoglio", "Sambuco", "Euforbia", "Veronica" };
    private void Start()
    {
        turno = 0;
        scelte = 0;
        fioriPerImpoll[0] = fiori0;
        fioriPerImpoll[1] = fiori1;
        fioriPerImpoll[2] = fiori2;
        fioriPerImpoll[3] = fiori3;
        fioriPerImpoll[4] = fiori4;
        fioriPerImpoll[5] = fiori5;
        fioriPerImpoll[6] = fiori6;
        fioriPerImpoll[7] = fiori7;
        proposte[0] = colori;
        proposte[1] = profumi;
        proposte[2] = periodi;
        proposte[3] = corolle;
        proposte[4] = ricompense;
        risposteCorrette[0] = new List<string>() { colori[2], profumi[2], periodi[0], corolle[2], ricompense[1] }.ToArray();
        risposteCorrette[1] = new List<string>() { colori[2], profumi[2], periodi[0], corolle[2], ricompense[1] }.ToArray();
        risposteCorrette[2] = new List<string>() { colori[0], profumi[0], periodi[0], corolle[0], ricompense[0] }.ToArray();
        risposteCorrette[3] = new List<string>() { colori[0], profumi[0], periodi[0], corolle[0], ricompense[0] }.ToArray();
        risposteCorrette[4] = new List<string>() { colori[1], profumi[1], periodi[1], corolle[1], ricompense[2] }.ToArray();
        risposteCorrette[5] = new List<string>() { colori[3], profumi[3], periodi[2], corolle[3], ricompense[1] }.ToArray();
        risposteCorrette[6] = new List<string>() { colori[4], profumi[4], periodi[0], corolle[4], ricompense[0] }.ToArray();
        risposteCorrette[7] = new List<string>() { colori[5], profumi[2], periodi[0], corolle[5], ricompense[1] }.ToArray();
    }

    void Update()
    {
        impollinatore = StartGame2.impollinatore;
        if (!isGameOver)
        {
            if(cambioTurno)
            {
                cambioTurno = false;
                //controllo la scelta fatta, cambio i button, aggiorno recap
                if (turno != 0)
                {
                    if (sceltaString == risposteCorrette[impollinatore][turno - 1])
                    {
                        scelte++;
                        Debug.Log("CORRETTO");
                    }
                }
                turnoText.text = turni[turno];
                turno++;
                if (turno == 6)
                {
                    OnGameOver();
                }
                else
                {
                    SetupScelte();//cambio i button delle risposte del turno scelto. Per 2,3,4,5 cambio il recap del turno appena fatto(-1)
                    SetupRecaps();
                }     
            }
        }
    }
    void SetupRecaps()
    {
        if (turno >= 2 && turno <= 5)
        {
            recaps[turno - 2].text = sceltaString;
        }
    }
    void SetupScelte()
    {
        int sceltaCorretta = Random.Range(0, 29) % 3;
        int secondaScelta = Random.Range(0, 29) % 3;
        string rispostaCorretta = risposteCorrette[impollinatore][turno - 1];
        Debug.Log(rispostaCorretta);
        string secondaString = "";
        string terzaString = "";
        while (secondaScelta == sceltaCorretta)
        {
            secondaScelta = Random.Range(0, 29) % 3;
        }
        int terzaScelta = 0;
        if (sceltaCorretta == 0 && secondaScelta == 1) terzaScelta = 2;
        if (sceltaCorretta == 1 && secondaScelta == 0) terzaScelta = 2;
        if (sceltaCorretta == 0 && secondaScelta == 2) terzaScelta = 1;
        if (sceltaCorretta == 2 && secondaScelta == 0) terzaScelta = 1;
        if (sceltaCorretta == 1 && secondaScelta == 2) terzaScelta = 0;
        if (sceltaCorretta == 2 && secondaScelta == 1) terzaScelta = 0;

        int length = proposte[turno - 1].Length;
        do
        {
            secondaString = proposte[turno - 1][Random.Range(0, 44) % length];
        } while (secondaString == rispostaCorretta);
        do
        {
            terzaString = proposte[turno - 1][Random.Range(0, 44) % length];
        } while (terzaString == rispostaCorretta || terzaString == secondaString);
        textButtons[sceltaCorretta].text = rispostaCorretta;
        textButtons[secondaScelta].text = secondaString;
        textButtons[terzaScelta].text = terzaString;
    }

    void OnGameOver()
    {
        isGameOver = true;
        TurnCanvas.SetActive(false);
        RecapCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
        score.text = scelte.ToString() + "/5 scelte corrette";
        int[] fiori;
        fioriPerImpoll.TryGetValue(impollinatore, out fiori);
        for(int i = 0; i< fiori.Length; i++)
        {
            spriteRenderers[i].sprite = spriteArray[fiori[i]];
            nomeFioriText[i].text = nomeFiori[fiori[i]];
        }
    }
}
