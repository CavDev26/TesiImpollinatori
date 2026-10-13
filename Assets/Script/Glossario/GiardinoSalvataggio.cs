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

    // Per ora lo sblocco e' tutto o niente (basta aver giocato una volta). In futuro qui si
    // controllera' l'elenco specifico delle zollette sbloccate in base ai punteggi dei minigiochi.
    public static bool EZollettaSbloccata(string idZolletta)
    {
        return HaGiocato;
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
