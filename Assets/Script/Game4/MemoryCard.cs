using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    [Header("Identità Carta")]
    public int cardID; // L'ID della carta (es. 0 per Tarassaco e Ape, 1 per Margherita e Farfalla...)
    
    [Header("Grafica")]
    public GameObject cardBack; // Il dorso della carta (quello che nasconde l'immagine)
    
    [Header("Riferimento al Controller")]
    public MemoryGameController controller; // Il "cervello" del gioco a cui comunicare i click

    // Questa funzione andrà collegata all'evento OnClick() del bottone della carta
    public void OnCardClicked()
    {
        // Se la carta è coperta e il gioco permette di cliccare
        if (cardBack.activeSelf && controller.canPlay)
        {
            cardBack.SetActive(false); // Scopri la carta
            controller.CardRevealed(this); // Avvisa il controller che questa carta è stata scoperta
        }
    }

    // Funzione chiamata dal controller se le carte sono sbagliate
    public void HideCard()
    {
        cardBack.SetActive(true); // Ricopre la carta
    }
}