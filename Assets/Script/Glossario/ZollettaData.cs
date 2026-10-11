using UnityEngine;

// Scheda-dati di una singola zolletta/ecosistema. Un asset per zolletta (Assets -> Create -> Giardino -> Zolletta).
// Per ora solo nome e immagine, per provare il meccanismo; in futuro si aggiungeranno qui le 3 piante
// e i 3 impollinatori caratteristici (con le rispettive icone), senza dover toccare carosello o slot.
[CreateAssetMenu(fileName = "NuovaZolletta", menuName = "Giardino/Zolletta")]
public class ZollettaData : ScriptableObject
{
    public string nome;
    public Sprite immagine;
}
