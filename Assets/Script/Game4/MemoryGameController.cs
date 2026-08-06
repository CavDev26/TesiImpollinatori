using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoryGameController : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject rulesPanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject datiPanel;
    public GameObject buttonsPanel;

    [Header("Griglia Carte")]
    public GameObject cardGrid;

    [Header("Impostazioni Griglia (Righe e Colonne)")]
    public int columns = 3;
    public int rows = 4;

    [Header("Testi UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI timeWon;
    public TextMeshProUGUI movesWon;

    [Header("Impostazioni Gioco")]
    public float initialTime = 60f;
    public int totalPairs = 6;

    [HideInInspector] public bool canPlay = false;
    private bool isGameOver = true;
    private int moves = 0;
    private int pairsFound = 0;
    private float timeLeft;

    private MemoryCard firstCard;
    private MemoryCard secondCard;

    private void Start()
    {
        rulesPanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        datiPanel.SetActive(false);
        buttonsPanel.SetActive(false);

        if (cardGrid != null) cardGrid.SetActive(false);
        isGameOver = true;
        canPlay = false;

        UpdateUI();
    }

    public void StartGame()
    {
        timeLeft = initialTime;
        moves = 0;
        pairsFound = 0;
        firstCard = null;
        secondCard = null;
        UpdateUI();

        rulesPanel.SetActive(false);
        datiPanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        if (cardGrid != null)
        {
            cardGrid.SetActive(true);
            ShuffleCards();
            ResetAllCards();
        }

        isGameOver = false;
        canPlay = true;
    }

    private void Update()
    {
        if (!isGameOver && canPlay)
        {
            timeLeft -= Time.deltaTime;
            timeText.text = "Tempo: " + Mathf.Ceil(timeLeft).ToString() + "s";

            if (timeLeft <= 0)
            {
                LoseGame();
            }
        }
    }

    private void ShuffleCards()
    {
        List<Transform> cards = new List<Transform>();

        foreach (Transform child in cardGrid.transform)
        {
            cards.Add(child);
        }

        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            Transform temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetSiblingIndex(i);
        }
    }

    private void ResetAllCards()
    {
        MemoryCard[] cards = cardGrid.GetComponentsInChildren<MemoryCard>();
        foreach (MemoryCard card in cards)
        {
            card.ResetCardState();
        }
    }

    public void CardRevealed(MemoryCard card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            moves++;
            UpdateUI();

            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        canPlay = false; // Blocca ulteriori click durante la verifica

        // Tempo d'attesa per consentire il completamento dell'animazione di apertura della seconda carta
        yield return new WaitForSeconds(0.4f);

        if (firstCard.cardID == secondCard.cardID)
        {
            // COPPIA CORRETTA: rimangono a faccia in su e non sono più cliccabili
            firstCard.SetMatched();
            secondCard.SetMatched();

            pairsFound++;
            if (pairsFound >= totalPairs)
            {
                yield return new WaitForSeconds(0.8f);
                WinGame();
            }
        }
        else
        {
            // COPPIA ERRATA: attesa visiva e rigiro
            yield return new WaitForSeconds(0.8f);
            firstCard.HideCard();
            secondCard.HideCard();
        }

        firstCard = null;
        secondCard = null;
        if (!isGameOver) canPlay = true;
    }

    private void UpdateUI()
    {
        movesText.text = "Mosse: " + moves.ToString();
        timeText.text = "Tempo: " + Mathf.Ceil(timeLeft).ToString() + " s";
    }

    private void WinGame()
    {
        isGameOver = true;
        canPlay = false;
        cardGrid.SetActive(false);
        datiPanel.SetActive(false);
        // Update the win panel with the final stats
        timeWon.text = Mathf.Ceil(initialTime - timeLeft).ToString() + " s";
        movesWon.text = moves.ToString();
        winPanel.SetActive(true);
        buttonsPanel.SetActive(true);
    }

    private void LoseGame()
    {
        isGameOver = true;
        canPlay = false;
        cardGrid.SetActive(false);
        datiPanel.SetActive(false);
        losePanel.SetActive(true);
        buttonsPanel.SetActive(true);
    }
}