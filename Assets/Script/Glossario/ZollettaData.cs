using UnityEngine;

// Scheda-dati di una singola zolletta/ecosistema. Un asset per zolletta (Assets -> Create -> Giardino -> Zolletta).
[CreateAssetMenu(fileName = "NuovaZolletta", menuName = "Giardino/Zolletta")]
public class ZollettaData : ScriptableObject
{
    public string nome;
    public Sprite immagine;
    public string descrizione;

    [Header("Piante e impollinatori caratteristici (pannello More)")]
    public PiantaData[] piante;
    public ImpollinatoreData[] impollinatori;
}
