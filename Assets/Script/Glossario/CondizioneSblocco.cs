using System;
using UnityEngine;

// I tipi di statistica su cui si puo' basare lo sblocco di una zolletta. Uno nuovo per ogni
// minigioco/traguardo aggiunto, man mano che i controller dei giochi iniziano a tracciare progressi.
public enum TipoCondizioneSblocco
{
    SempreSbloccata,
    RecordGioco1A,
    RecordGioco1B,
    RecordGioco2,
    TempoGioco3B,
    HaGiocatoTutti,
    // Aggiunto in coda (non in mezzo) per non spostare i valori numerici di quelli sopra, gia'
    // eventualmente salvati in qualche ZollettaData.
    RecordGioco3A,
    TempoGioco4,
    MosseGioco4,
    // Caso speciale: dipende dallo stato delle altre zollette, non da una statistica di un
    // minigioco. Non valutabile da EValida() da sola (non conosce le altre zollette): gestita a
    // parte in GiardinoSalvataggio.EZollettaSbloccata.
    TutteLeAltreSbloccate,
}

// Condizione di sblocco di una zolletta: quale statistica guardare e quale soglia raggiungere.
// Per i punteggi (RecordGioco1A/B, RecordGioco2, RecordGioco3A) serve "almeno" la soglia; per i
// tempi (TempoGioco3B) serve "al massimo" la soglia, visto che un tempo piu' basso e' un risultato
// migliore.
[Serializable]
public class CondizioneSblocco
{
    public TipoCondizioneSblocco tipo = TipoCondizioneSblocco.SempreSbloccata;
    public float soglia;

    public bool EValida()
    {
        switch (tipo)
        {
            case TipoCondizioneSblocco.SempreSbloccata:
                return true;
            case TipoCondizioneSblocco.RecordGioco1A:
                return ProgressiMinigiochi.RecordGioco1A >= soglia;
            case TipoCondizioneSblocco.RecordGioco1B:
                return ProgressiMinigiochi.RecordGioco1B >= soglia;
            case TipoCondizioneSblocco.RecordGioco2:
                return ProgressiMinigiochi.RecordGioco2 >= soglia;
            case TipoCondizioneSblocco.TempoGioco3B:
                return ProgressiMinigiochi.HaGiocatoGioco3 && ProgressiMinigiochi.TempoRecordGioco3 <= soglia;
            case TipoCondizioneSblocco.HaGiocatoTutti:
                return ProgressiMinigiochi.TuttiIGiochiGiocatiAlmenoUnaVolta;
            case TipoCondizioneSblocco.RecordGioco3A:
                return ProgressiMinigiochi.RecordGioco3Conteggio >= soglia;
            case TipoCondizioneSblocco.TempoGioco4:
                return ProgressiMinigiochi.HaGiocatoGioco4 && ProgressiMinigiochi.TempoRecordGioco4 <= soglia;
            case TipoCondizioneSblocco.MosseGioco4:
                return ProgressiMinigiochi.HaGiocatoGioco4 && ProgressiMinigiochi.MosseRecordGioco4 <= soglia;
            case TipoCondizioneSblocco.TutteLeAltreSbloccate:
                // Gestita in GiardinoSalvataggio.EZollettaSbloccata, che conosce le altre zollette.
                // Se EValida() viene chiamata direttamente per questo tipo, non c'e' modo di saperlo.
                return false;
            default:
                return false;
        }
    }
}
