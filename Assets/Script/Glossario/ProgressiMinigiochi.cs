using UnityEngine;

// Lettura E scrittura centralizzata dei progressi salvati dai minigiochi. Unico punto che conosce
// le chiavi PlayerPrefs: i controller dei giochi chiamano i metodi Segna*/AggiornaRecord* invece di
// scrivere PlayerPrefs.Set* direttamente, e la logica di sblocco delle zollette legge da qui invece
// di conoscere le chiavi a sua volta.
public static class ProgressiMinigiochi
{
    private const string ChiaveGiocatoGioco1 = "Giocato_Gioco1";
    private const string ChiaveGiocatoGioco2 = "Giocato_Gioco2";
    private const string ChiaveGiocatoGioco3 = "Giocato_Gioco3";
    private const string ChiaveGiocatoGioco4 = "Giocato_Gioco4";

    private const string ChiaveRecordGioco1A = "RecordGame1.0";
    private const string ChiaveRecordGioco1B = "RecordGame1.1";
    private const string ChiaveRecordGioco2 = "RecordGame2";
    private const string ChiaveTempoGioco3 = "RecordGame3";
    private const string ChiaveRecordGioco3Conteggio = "RecordGame3Conteggio";
    private const string ChiaveTempoGioco4 = "RecordGame4Tempo";
    private const string ChiaveMosseGioco4 = "RecordGame4Mosse";

    // Flag espliciti "ci ho giocato almeno una volta", impostati a fine partita (vinta o persa) da
    // ciascun controller. Servono per l'obiettivo aggregato HaGiocatoTutti: dedurlo dai record sotto
    // darebbe falsi negativi (es. il tempo del Gioco 3 si salva solo sulle run perfette).
    public static bool GiocatoAlmenoUnaVoltaGioco1 => PlayerPrefs.GetInt(ChiaveGiocatoGioco1, 0) == 1;
    public static bool GiocatoAlmenoUnaVoltaGioco2 => PlayerPrefs.GetInt(ChiaveGiocatoGioco2, 0) == 1;
    public static bool GiocatoAlmenoUnaVoltaGioco3 => PlayerPrefs.GetInt(ChiaveGiocatoGioco3, 0) == 1;
    public static bool GiocatoAlmenoUnaVoltaGioco4 => PlayerPrefs.GetInt(ChiaveGiocatoGioco4, 0) == 1;

    public static bool TuttiIGiochiGiocatiAlmenoUnaVolta =>
        GiocatoAlmenoUnaVoltaGioco1 && GiocatoAlmenoUnaVoltaGioco2
        && GiocatoAlmenoUnaVoltaGioco3 && GiocatoAlmenoUnaVoltaGioco4;

    public static int RecordGioco1A => PlayerPrefs.GetInt(ChiaveRecordGioco1A, 0);
    public static int RecordGioco1B => PlayerPrefs.GetInt(ChiaveRecordGioco1B, 0);

    // Migliori "risposte corrette su 5" ottenute nel Gioco 2.
    public static int RecordGioco2 => PlayerPrefs.GetInt(ChiaveRecordGioco2, 0);

    // 0 = nessun tempo salvato ancora (nessuna partita completata). Per il Gioco 3 un tempo piu'
    // basso e' un risultato migliore. Il tempo si salva solo quando la partita e' completata con
    // tutti e 4 gli abbinamenti corretti (vedi Game3Controller.OnGameOver).
    public static float TempoRecordGioco3 => PlayerPrefs.GetFloat(ChiaveTempoGioco3, 0f);
    public static bool HaGiocatoGioco3 => TempoRecordGioco3 > 0f;

    // Miglior numero di abbinamenti corretti ottenuti nel Gioco 3 (0-4), anche nelle partite non
    // completate perfettamente.
    public static int RecordGioco3Conteggio => PlayerPrefs.GetInt(ChiaveRecordGioco3Conteggio, 0);

    // Gioco 4 (memory): entrambi salvati solo sulle partite vinte, insieme (vedi
    // MemoryGameController.WinGame). 0 = nessun record ancora.
    public static float TempoRecordGioco4 => PlayerPrefs.GetFloat(ChiaveTempoGioco4, 0f);
    public static int MosseRecordGioco4 => PlayerPrefs.GetInt(ChiaveMosseGioco4, 0);
    public static bool HaGiocatoGioco4 => TempoRecordGioco4 > 0f;

    // --- Scrittura, chiamata dai controller dei minigiochi a fine partita. Nessuno di questi fa il
    // proprio PlayerPrefs.Save(): il chiamante lo fa una sola volta dopo aver chiamato tutti quelli
    // che gli servono, cosi' una partita finita scrive su disco una volta sola invece di piu' volte. ---

    public static void SegnaGiocato(int numeroGioco)
    {
        string chiave = numeroGioco switch
        {
            1 => ChiaveGiocatoGioco1,
            2 => ChiaveGiocatoGioco2,
            3 => ChiaveGiocatoGioco3,
            4 => ChiaveGiocatoGioco4,
            _ => null
        };
        if (chiave != null)
        {
            PlayerPrefs.SetInt(chiave, 1);
        }
    }

    // Aggiorna il record solo se il nuovo valore e' migliore. piuAltoVince=true per i punteggi
    // (es. risposte corrette), false per i tempi (dove un valore piu' basso e' un risultato migliore).
    public static void AggiornaRecordSeMigliore(string chiave, int valore, bool piuAltoVince)
    {
        int attuale = PlayerPrefs.GetInt(chiave, 0);
        if (piuAltoVince ? valore > attuale : (attuale == 0 || valore < attuale))
        {
            PlayerPrefs.SetInt(chiave, valore);
        }
    }

    public static void AggiornaRecordSeMigliore(string chiave, float valore, bool piuAltoVince)
    {
        float attuale = PlayerPrefs.GetFloat(chiave, 0f);
        if (piuAltoVince ? valore > attuale : (attuale == 0f || valore < attuale))
        {
            PlayerPrefs.SetFloat(chiave, valore);
        }
    }

    // Nomi delle chiavi sopra, esposti ai controller dei minigiochi per passarli a
    // AggiornaRecordSeMigliore senza duplicare la stringa letterale.
    public static class Chiavi
    {
        public const string RecordGioco1A = ChiaveRecordGioco1A;
        public const string RecordGioco1B = ChiaveRecordGioco1B;
        public const string RecordGioco2 = ChiaveRecordGioco2;
        public const string TempoGioco3 = ChiaveTempoGioco3;
        public const string RecordGioco3Conteggio = ChiaveRecordGioco3Conteggio;
        public const string TempoGioco4 = ChiaveTempoGioco4;
        public const string MosseGioco4 = ChiaveMosseGioco4;
    }

    private static readonly string[] TutteLeChiavi =
    {
        ChiaveGiocatoGioco1, ChiaveRecordGioco1A, ChiaveRecordGioco1B,
        ChiaveGiocatoGioco2, ChiaveRecordGioco2,
        ChiaveGiocatoGioco3, ChiaveTempoGioco3, ChiaveRecordGioco3Conteggio,
        ChiaveGiocatoGioco4, ChiaveTempoGioco4, ChiaveMosseGioco4,
    };

    // Cancella tutte le statistiche salvate dai minigiochi, come se l'utente non avesse mai
    // giocato. Usato dal bottone di test "Blocca tutto" nel giardino.
    public static void ResettaTutto()
    {
        foreach (string chiave in TutteLeChiavi)
        {
            PlayerPrefs.DeleteKey(chiave);
        }
        PlayerPrefs.Save();
    }
}
