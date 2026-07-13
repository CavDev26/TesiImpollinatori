using System.Collections;
using UnityEngine;
using TMPro; // Serve per usare i testi moderni della UI

public class MemoryGameController : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject rulesPanel; // Schermata iniziale con le regole
    public GameObject winPanel;   // Schermata di vittoria
    public GameObject losePanel;  // Schermata di sconfitta

    [Header("Testi UI")]
    public TextMeshProUGUI timeText;  // Testo per il tempo rimanente
    public TextMeshProUGUI movesText; // Testo per le mosse fatte

    [Header("Impostazioni Gioco")]
    public float timeLeft = 60f; // 60 secondi a disposizione
    public int totalPairs = 6;   // Quante coppie ci sono nel gioco? (es. 6 coppie = 12 carte)

    // Variabili di stato interne (nascoste dall'Inspector)
    [HideInInspector] public bool canPlay = false; // Controlla se l'utente può cliccare
    private bool isGameOver = true;
    private int moves = 0;
    private int pairsFound = 0;

    // Memoria per le due carte girate
    private MemoryCard firstCard;
    private MemoryCard secondCard;

    private void Start()
    {
        // All'inizio, attiviamo le regole e blocchiamo il gioco
        rulesPanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        isGameOver = true;
        canPlay = false;
        
        UpdateUI();
    }

    // Questa funzione va messa sul bottone "Inizia" del pannello delle regole
    public void StartGame()
    {
        rulesPanel.SetActive(false); // Nascondi le regole
        isGameOver = false;
        canPlay = true; // Permetti i click
    }

    private void Update()
    {
        // Se il gioco è in corso, fai scendere il timer
        if (!isGameOver && canPlay)
        {
            timeLeft -= Time.deltaTime; // Sottrae i secondi
            
            // Aggiorna il testo arrotondando il numero
            timeText.text = "Tempo: " + Mathf.Ceil(timeLeft).ToString() + "s";

            if (timeLeft <= 0)
            {
                LoseGame();
            }
        }
    }

    // Viene chiamata dalla singola carta quando viene cliccata
    public void CardRevealed(MemoryCard card)
    {
        if (firstCard == null)
        {
            firstCard = card; // Salviamo la prima carta
        }
        else
        {
            secondCard = card; // Salviamo la seconda carta
            moves++; // Aggiungiamo una mossa
            UpdateUI();
            
            // Blocchiamo i click e controlliamo se c'è un match
            StartCoroutine(CheckMatch()); 
        }
    }

    // Coroutine per aspettare un secondo prima di rigirare le carte
    IEnumerator CheckMatch()
    {
        canPlay = false; // Blocca i click temporaneamente

        if (firstCard.cardID == secondCard.cardID)
        {
            // COPPIA TROVATA!
            pairsFound++;
            if (pairsFound >= totalPairs)
            {
                WinGame();
            }
        }
        else
        {
            // COPPIA SBAGLIATA! Aspetta 1 secondo e coprile di nuovo
            yield return new WaitForSeconds(1f);
            firstCard.HideCard();
            secondCard.HideCard();
        }

        // Resetta la memoria e sblocca i click
        firstCard = null;
        secondCard = null;
        if (!isGameOver) canPlay = true;
    }

    private void UpdateUI()
    {
        movesText.text = "Mosse: " + moves.ToString();
    }

    private void WinGame()
    {
        isGameOver = true;
        canPlay = false;
        winPanel.SetActive(true);
        
        // QUI IN FUTURO AGGIUNGEREMO IL SALVATAGGIO DEI PLAYERPREFS
    }

    private void LoseGame()
    {
        isGameOver = true;
        canPlay = false;
        losePanel.SetActive(true);
    }
}