using UnityEngine;

// Lettura centralizzata dei progressi salvati dai minigiochi (le stesse chiavi PlayerPrefs gia'
// scritte dai rispettivi controller). Unico punto da cui la logica di sblocco delle zollette legge
// "cosa ha fatto il giocatore finora", senza conoscere le chiavi PlayerPrefs dei singoli giochi.
public static class ProgressiMinigiochi
{
    // Flag espliciti "ci ho giocato almeno una volta", impostati a fine partita (vinta o persa) da
    // ciascun controller. Servono per l'obiettivo aggregato HaGiocatoTutti: dedurlo dai record sopra
    // darebbe falsi negativi (es. il tempo del Gioco 3 si salva solo sulle run perfette).
    public static bool GiocatoAlmenoUnaVoltaGioco1 => PlayerPrefs.GetInt("Giocato_Gioco1", 0) == 1;
    public static bool GiocatoAlmenoUnaVoltaGioco2 => PlayerPrefs.GetInt("Giocato_Gioco2", 0) == 1;
    public static bool GiocatoAlmenoUnaVoltaGioco3 => PlayerPrefs.GetInt("Giocato_Gioco3", 0) == 1;
    public static bool GiocatoAlmenoUnaVoltaGioco4 => PlayerPrefs.GetInt("Giocato_Gioco4", 0) == 1;

    public static bool TuttiIGiochiGiocatiAlmenoUnaVolta =>
        GiocatoAlmenoUnaVoltaGioco1 && GiocatoAlmenoUnaVoltaGioco2
        && GiocatoAlmenoUnaVoltaGioco3 && GiocatoAlmenoUnaVoltaGioco4;

    public static int RecordGioco1A => PlayerPrefs.GetInt("RecordGame1.0", 0);
    public static int RecordGioco1B => PlayerPrefs.GetInt("RecordGame1.1", 0);

    // Migliori "risposte corrette su 5" ottenute nel Gioco 2.
    public static int RecordGioco2 => PlayerPrefs.GetInt("RecordGame2", 0);

    // 0 = nessun tempo salvato ancora (nessuna partita completata). Per il Gioco 3 un tempo piu'
    // basso e' un risultato migliore. Il tempo si salva solo quando la partita e' completata con
    // tutti e 4 gli abbinamenti corretti (vedi Game3Controller.OnGameOver).
    public static float TempoRecordGioco3 => PlayerPrefs.GetFloat("RecordGame3", 0f);
    public static bool HaGiocatoGioco3 => TempoRecordGioco3 > 0f;

    // Miglior numero di abbinamenti corretti ottenuti nel Gioco 3 (0-4), anche nelle partite non
    // completate perfettamente.
    public static int RecordGioco3Conteggio => PlayerPrefs.GetInt("RecordGame3Conteggio", 0);

    // Gioco 4 (memory): entrambi salvati solo sulle partite vinte, insieme (vedi
    // MemoryGameController.WinGame). 0 = nessun record ancora.
    public static float TempoRecordGioco4 => PlayerPrefs.GetFloat("RecordGame4Tempo", 0f);
    public static int MosseRecordGioco4 => PlayerPrefs.GetInt("RecordGame4Mosse", 0);
    public static bool HaGiocatoGioco4 => TempoRecordGioco4 > 0f;

    private static readonly string[] TutteLeChiavi =
    {
        "Giocato_Gioco1", "RecordGame1.0", "RecordGame1.1",
        "Giocato_Gioco2", "RecordGame2",
        "Giocato_Gioco3", "RecordGame3", "RecordGame3Conteggio",
        "Giocato_Gioco4", "RecordGame4Tempo", "RecordGame4Mosse",
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
