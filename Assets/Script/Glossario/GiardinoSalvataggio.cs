using System;
using System.Collections.Generic;
using UnityEngine;

// Salvataggio locale (PlayerPrefs) dello stato del giardino: quali zollette sono sbloccate e quali
// sono piazzate in quale esagono. Classe statica, nessuna scena/istanza richiesta.
public static class GiardinoSalvataggio
{
    private const string ChiaveHaGiocato = "Giardino_HaGiocato";
    private const string ChiavePiazzamenti = "Giardino_Piazzamenti";

    public static bool HaGiocato => PlayerPrefs.GetInt(ChiaveHaGiocato, 0) == 1;

    public static void SegnaGiocato()
    {
        PlayerPrefs.SetInt(ChiaveHaGiocato, 1);
        PlayerPrefs.Save();
    }

    public static void ResettaGiocato()
    {
        PlayerPrefs.SetInt(ChiaveHaGiocato, 0);
        PlayerPrefs.Save();
    }

    // HaGiocato resta un override "sblocca tutto" per i bottoni di test: se attivo, salta la
    // condizione specifica della zolletta. Altrimenti si valuta la sua CondizioneSblocco
    // (vedi CondizioneSblocco.cs) contro i progressi reali salvati nei minigiochi.
    //
    // "tutteLeZollette" e' opzionale e serve solo per il tipo TutteLeAltreSbloccate (l'unico che
    // deve guardare lo stato delle altre zollette, non solo i progressi dei minigiochi): se non
    // viene passato, quel tipo di condizione non puo' essere valutata e risulta sempre bloccata.
    public static bool EZollettaSbloccata(ZollettaData zolletta, ZollettaData[] tutteLeZollette = null)
    {
        if (zolletta == null)
        {
            return false;
        }
        if (HaGiocato)
        {
            return true;
        }
        if (zolletta.condizioneSblocco == null)
        {
            return false;
        }

        if (zolletta.condizioneSblocco.tipo == TipoCondizioneSblocco.TutteLeAltreSbloccate)
        {
            return TutteLeAltreSonoSbloccate(zolletta, tutteLeZollette);
        }
        return zolletta.condizioneSblocco.EValida();
    }

    private static bool TutteLeAltreSonoSbloccate(ZollettaData zolletta, ZollettaData[] tutteLeZollette)
    {
        if (tutteLeZollette == null)
        {
            return false;
        }

        foreach (ZollettaData altra in tutteLeZollette)
        {
            if (altra == null || altra == zolletta)
            {
                continue;
            }
            // Non passiamo "tutteLeZollette" qui: questa condizione e' pensata per una sola
            // zolletta "finale", non deve dipendere a sua volta da un'altra con lo stesso tipo
            // (eviterebbe un ciclo infinito).
            if (!EZollettaSbloccata(altra))
            {
                return false;
            }
        }
        return true;
    }

    [Serializable]
    private class Piazzamento
    {
        public int slot;
        public string zollettaId;
    }

    [Serializable]
    private class ElencoPiazzamenti
    {
        public List<Piazzamento> voci = new List<Piazzamento>();
    }

    public static void SalvaPiazzamenti(Dictionary<int, string> piazzamenti)
    {
        ElencoPiazzamenti elenco = new ElencoPiazzamenti();
        foreach (KeyValuePair<int, string> coppia in piazzamenti)
        {
            elenco.voci.Add(new Piazzamento { slot = coppia.Key, zollettaId = coppia.Value });
        }
        PlayerPrefs.SetString(ChiavePiazzamenti, JsonUtility.ToJson(elenco));
        PlayerPrefs.Save();
    }

    public static Dictionary<int, string> CaricaPiazzamenti()
    {
        Dictionary<int, string> risultato = new Dictionary<int, string>();
        string json = PlayerPrefs.GetString(ChiavePiazzamenti, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return risultato;
        }

        ElencoPiazzamenti elenco = JsonUtility.FromJson<ElencoPiazzamenti>(json);
        if (elenco != null && elenco.voci != null)
        {
            foreach (Piazzamento voce in elenco.voci)
            {
                risultato[voce.slot] = voce.zollettaId;
            }
        }
        return risultato;
    }
}
