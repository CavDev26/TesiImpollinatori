using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    [Header("Identità Carta")]
    public int cardID;

    [Header("Grafica")]
    public GameObject cardBack; // L'oggetto UI del dorso della carta

    [Header("Riferimenti")]
    public MemoryGameController controller;
    public Button cardButton;

    [HideInInspector] public bool isFaceUp = false;
    [HideInInspector] public bool isMatched = false;

    private bool isFlipping = false; // Impedisce doppi click durante la rotazione

    private void Awake()
    {
        if (cardButton == null)
            cardButton = GetComponent<Button>();
    }

    public void OnCardClicked()
    {
        // Interagibile solo se coperta, non abbinata, non in fase di animazione e se il gioco lo permette
        if (!isFaceUp && !isMatched && !isFlipping && controller.canPlay)
        {
            StartCoroutine(FlipRoutine(true));
            controller.CardRevealed(this);
        }
    }

    public void HideCard()
    {
        if (isFaceUp && !isMatched && !isFlipping)
        {
            StartCoroutine(FlipRoutine(false));
        }
    }

    // Reset istantaneo all'avvio partita
    public void ResetCardState()
    {
        StopAllCoroutines();
        isFlipping = false;
        isFaceUp = false;
        isMatched = false;

        transform.localRotation = Quaternion.identity; // Reset rotazione a (0,0,0)
        if (cardBack != null) cardBack.SetActive(true);
        if (cardButton != null) cardButton.interactable = true;
    }

    public void SetMatched()
    {
        isMatched = true;
        isFaceUp = true;
        if (cardButton != null) cardButton.interactable = false;
    }

    // Coroutine per la rotazione fluida a 2 fasi (0° -> 90° -> 0°)
    private IEnumerator FlipRoutine(bool showFace)
    {
        isFlipping = true;
        float duration = 0.15f; // Durata di ciascuna mezza rotazione
        float elapsed = 0f;

        Quaternion startRotation = Quaternion.Euler(0, 0, 0);
        Quaternion midRotation = Quaternion.Euler(0, 90, 0);

        // Fase 1: Ruota da 0° a 90° (carta di taglio)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Slerp(startRotation, midRotation, t);
            yield return null;
        }

        // Punto medio: cambia visibilità del dorso
        if (cardBack != null) cardBack.SetActive(!showFace);
        isFaceUp = showFace;

        // Fase 2: Ruota da 90° a 0° (torna frontale senza specchiare i testi)
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Slerp(midRotation, startRotation, t);
            yield return null;
        }

        transform.localRotation = Quaternion.identity;
        isFlipping = false;
    }
}