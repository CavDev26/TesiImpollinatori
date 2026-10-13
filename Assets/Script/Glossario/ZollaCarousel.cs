using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZollaCarousel : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("Il prefab CaroselloZollette messo in scena. Viewport, frecce e X vengono trovati da soli dentro di lui.")]
    public GameObject pannello;
    public Button pulsantePiu;
    [Tooltip("Testo che mostra il nome della sola zolletta al centro del carosello.")]
    public TMP_Text nomeCentrale;

    [Header("Zollette disponibili")]
    [Tooltip("Una scheda (ScriptableObject) per ogni zolletta: Assets -> Create -> Giardino -> Zolletta.")]
    public ZollettaData[] zollette;

    [Header("Carosello")]
    [Min(1)] public int zollePerPagina = 3;
    public float durataScorrimento = 0.25f;

    private RectTransform viewport;
    private RectTransform contenuto;
    private readonly List<ZollaItem> items = new List<ZollaItem>();
    private readonly List<ZollaSlot> slotGriglia = new List<ZollaSlot>();
    private bool[] piazzate;
    private int selezionata = -1;
    private float posizione;
    private float destinazione;
    private bool costruito;
    private bool caricamentoInCorso;
    private Coroutine scorrimentoInCorso;

    public int ZollaSelezionata => selezionata;
    public bool Aperto => pannello != null && pannello.activeSelf;

    void Start()
    {
        if (pannello == null)
        {
            Debug.LogWarning("ZollaCarousel: assegna il prefab CaroselloZollette nel campo Pannello.");
            return;
        }

        Transform t = pannello.transform;
        viewport = t.Find("Viewport") as RectTransform;
        contenuto = t.Find("Viewport/Contenuto") as RectTransform;

        CollegaBottone(t.Find("FrecciaSinistra"), () => Scorri(-1));
        CollegaBottone(t.Find("FrecciaDestra"), () => Scorri(1));
        CollegaBottone(t.Find("ChiudiCarosello"), Chiudi);

        foreach (Text testo in pannello.GetComponentsInChildren<Text>(true))
        {
            if (testo.font == null)
            {
                testo.font = FontPredefinito();
            }
        }

        if (pulsantePiu != null)
        {
            pulsantePiu.onClick.AddListener(Alterna);
        }
        CreaBottoneTestGiocato();

        // Le zollette si costruiscono subito, all'avvio della scena, invece che al primo click sul "+".
        Costruisci();

        pannello.SetActive(false);
    }

    private static void CollegaBottone(Transform oggetto, UnityEngine.Events.UnityAction azione)
    {
        if (oggetto != null && oggetto.TryGetComponent(out Button bottone))
        {
            bottone.onClick.AddListener(azione);
        }
    }

    public void Alterna()
    {
        if (pannello == null)
        {
            return;
        }

        if (Aperto)
        {
            Chiudi();
            return;
        }

        pannello.transform.localScale = Vector3.one;
        pannello.SetActive(true);
    }

    public void Chiudi()
    {
        if (pannello == null)
        {
            return;
        }

        selezionata = -1;
        AggiornaItems();
        pannello.SetActive(false);
    }

    public void Seleziona(int indice)
    {
        if (piazzate == null || indice < 0 || indice >= piazzate.Length || piazzate[indice])
        {
            return;
        }
        if (!EZollettaSbloccata(indice))
        {
            return;
        }

        selezionata = selezionata == indice ? -1 : indice;
        AggiornaItems();
    }

    public void PiazzaInSlot(ZollaSlot slot, int indice)
    {
        if (slot == null || piazzate == null || indice < 0 || indice >= piazzate.Length || piazzate[indice])
        {
            return;
        }
        if (!EZollettaSbloccata(indice))
        {
            return;
        }

        if (slot.Occupata)
        {
            int vecchia = slot.Svuota();
            if (vecchia >= 0)
            {
                piazzate[vecchia] = false;
            }
        }

        slot.Riempi(indice, zollette[indice]);
        piazzate[indice] = true;

        if (selezionata == indice)
        {
            selezionata = -1;
        }
        AggiornaItems();
        SalvaStatoPiazzamenti();
    }

    public void Libera(int indice)
    {
        if (piazzate != null && indice >= 0 && indice < piazzate.Length)
        {
            piazzate[indice] = false;
        }
        AggiornaItems();
        SalvaStatoPiazzamenti();
    }

    private bool EZollettaSbloccata(int indice)
    {
        return zollette != null && indice >= 0 && indice < zollette.Length
            && zollette[indice] != null && GiardinoSalvataggio.EZollettaSbloccata(zollette[indice].name);
    }

    public bool EPiazzata(int indice)
    {
        return piazzate != null && indice >= 0 && indice < piazzate.Length && piazzate[indice];
    }

    public void ResettaPiazzamenti()
    {
        if (piazzate != null)
        {
            for (int i = 0; i < piazzate.Length; i++)
            {
                piazzate[i] = false;
            }
        }
        selezionata = -1;
        AggiornaItems();
    }

    private void Costruisci()
    {
        if (costruito)
        {
            return;
        }
        if (viewport == null || contenuto == null)
        {
            Debug.LogWarning("ZollaCarousel: nel prefab manca Viewport/Contenuto.");
            return;
        }
        if (zollette == null || zollette.Length == 0)
        {
            Debug.LogWarning("ZollaCarousel: assegna almeno una scheda in 'Zollette'.");
            return;
        }

        // Impostato solo ora che siamo sicuri di costruire davvero: se uno dei controlli sopra
        // fallisce (ad es. perche' Start() del carosello non e' ancora stato eseguito), un prossimo
        // tentativo deve poter ritentare invece di restare bloccato per sempre.
        costruito = true;
        Canvas.ForceUpdateCanvases();

        piazzate = new bool[zollette.Length];

        float larghezzaItem = viewport.rect.width / zollePerPagina;
        float altezza = viewport.rect.height;

        for (int i = 0; i < zollette.Length; i++)
        {
            ZollettaData dato = zollette[i];

            GameObject go = new GameObject("Zolla_" + i, typeof(RectTransform));
            go.transform.SetParent(contenuto, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(larghezzaItem * 0.9f, altezza * 0.9f);

            ZollaItem item = go.AddComponent<ZollaItem>();
            item.Init(this, i, dato != null ? dato.immagine : null);
            items.Add(item);
        }

        AggiornaPosizioni();
        AggiornaItems();
    }

    private static Font FontPredefinito()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        return font;
    }

    // --- Registro degli slot della griglia e salvataggio/caricamento (vedi GiardinoSalvataggio) ---

    public void SvuotaRegistroSlot()
    {
        slotGriglia.Clear();
    }

    public void RegistraSlot(ZollaSlot slot)
    {
        if (slot != null)
        {
            slotGriglia.Add(slot);
        }
    }

    public void CaricaPiazzamentiSalvati()
    {
        if (!costruito)
        {
            Costruisci();
        }
        if (zollette == null || piazzate == null)
        {
            return;
        }

        caricamentoInCorso = true;
        Dictionary<int, string> salvati = GiardinoSalvataggio.CaricaPiazzamenti();
        foreach (KeyValuePair<int, string> coppia in salvati)
        {
            ZollaSlot slot = TrovaSlotPerIndiceGriglia(coppia.Key);
            int indiceZolletta = TrovaIndiceZolletta(coppia.Value);
            if (slot != null && indiceZolletta >= 0 && !piazzate[indiceZolletta])
            {
                slot.Riempi(indiceZolletta, zollette[indiceZolletta]);
                piazzate[indiceZolletta] = true;
            }
        }
        caricamentoInCorso = false;

        AggiornaItems();
    }

    private ZollaSlot TrovaSlotPerIndiceGriglia(int indiceGriglia)
    {
        foreach (ZollaSlot slot in slotGriglia)
        {
            if (slot != null && slot.IndiceGriglia == indiceGriglia)
            {
                return slot;
            }
        }
        return null;
    }

    private int TrovaIndiceZolletta(string id)
    {
        if (zollette == null)
        {
            return -1;
        }
        for (int i = 0; i < zollette.Length; i++)
        {
            if (zollette[i] != null && zollette[i].name == id)
            {
                return i;
            }
        }
        return -1;
    }

    private void SalvaStatoPiazzamenti()
    {
        if (caricamentoInCorso)
        {
            return;
        }

        Dictionary<int, string> piazzamenti = new Dictionary<int, string>();
        foreach (ZollaSlot slot in slotGriglia)
        {
            if (slot != null && slot.Occupata && slot.Dato != null)
            {
                piazzamenti[slot.IndiceGriglia] = slot.Dato.name;
            }
        }
        GiardinoSalvataggio.SalvaPiazzamenti(piazzamenti);
    }

    // --- Bottoni temporanei di test: sbloccano/bloccano tutte le zollette, uno a sinistra e uno a
    // destra del "+", stessa misura e stile. ---

    private void CreaBottoneTestGiocato()
    {
        CreaBottoneTest("TestHaGiocato", "TEST:\nHa giocato", -1, new Color(0.3f, 0.55f, 0.3f),
            () => GiardinoSalvataggio.SegnaGiocato());

        CreaBottoneTest("TestBloccaTutto", "TEST:\nBlocca tutto", 1, new Color(0.55f, 0.3f, 0.3f),
            () =>
            {
                GiardinoSalvataggio.ResettaGiocato();
                RimuoviTuttiIPiazzamenti();
            });
    }

    // Toglie dalla griglia tutte le zollette piazzate (usato dal bottone di test "Blocca tutto",
    // cosi' si riparte puliti invece di avere piazzamenti non piu' sbloccati rimasti sugli esagoni).
    private void RimuoviTuttiIPiazzamenti()
    {
        foreach (ZollaSlot slot in slotGriglia)
        {
            if (slot != null && slot.Occupata)
            {
                slot.Svuota();
            }
        }
        ResettaPiazzamenti();
        SalvaStatoPiazzamenti();
    }

    private void CreaBottoneTest(string nome, string testoBottone, int direzione, Color coloreSfondo, System.Action azione)
    {
        if (pulsantePiu == null)
        {
            return;
        }

        RectTransform rifRect = pulsantePiu.GetComponent<RectTransform>();
        if (rifRect == null || rifRect.parent == null)
        {
            return;
        }

        GameObject go = new GameObject(nome, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(rifRect.parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rifRect.anchorMin;
        rt.anchorMax = rifRect.anchorMax;
        rt.pivot = rifRect.pivot;
        rt.sizeDelta = rifRect.sizeDelta * 0.6f;
        rt.anchoredPosition = rifRect.anchoredPosition
            + new Vector2(direzione * (rifRect.sizeDelta.x * 0.5f + rt.sizeDelta.x * 0.5f + 20f), 0f);

        Image sfondo = go.GetComponent<Image>();
        sfondo.color = coloreSfondo;

        GameObject testoGo = new GameObject("Testo", typeof(RectTransform), typeof(Text));
        testoGo.transform.SetParent(go.transform, false);
        RectTransform trt = testoGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;

        Text testo = testoGo.GetComponent<Text>();
        testo.text = testoBottone;
        testo.alignment = TextAnchor.MiddleCenter;
        testo.fontSize = 18;
        testo.color = Color.white;
        testo.font = FontPredefinito();

        Button bottone = go.GetComponent<Button>();
        bottone.targetGraphic = sfondo;
        bottone.onClick.AddListener(() =>
        {
            azione();
            AggiornaItems();
        });
    }

    // Scorrimento circolare: le zollette stanno su un anello. Dall'ultima si torna alla prima.
    private void Scorri(int passo)
    {
        if (!costruito || items.Count == 0)
        {
            return;
        }

        destinazione += passo;
        if (scorrimentoInCorso != null)
        {
            StopCoroutine(scorrimentoInCorso);
        }
        scorrimentoInCorso = StartCoroutine(AnimaScorrimento(destinazione));
    }

    private IEnumerator AnimaScorrimento(float arrivo)
    {
        float partenza = posizione;
        float tempo = 0f;
        while (tempo < durataScorrimento)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tempo / durataScorrimento));
            posizione = Mathf.Lerp(partenza, arrivo, t);
            AggiornaPosizioni();
            yield return null;
        }

        int n = items.Count;
        posizione = Mathf.Repeat(arrivo, n);
        destinazione = posizione;
        AggiornaPosizioni();
        scorrimentoInCorso = null;
    }

    // Quanto si rimpicciolisce una zolletta mano a mano che si allontana dal centro del carosello
    // (distanza 0 = al centro, scala piena; distanza 1 o piu' = ai lati, scala minima).
    private const float ScalaCentro = 1.12f;
    private const float ScalaBordo = 0.82f;

    private void AggiornaPosizioni()
    {
        int n = items.Count;
        if (n == 0 || viewport == null)
        {
            return;
        }

        float larghezzaItem = viewport.rect.width / zollePerPagina;
        int indiceCentrale = -1;
        float distanzaMigliore = float.MaxValue;

        foreach (ZollaItem item in items)
        {
            float d = item.Indice - posizione;
            d = Mathf.Repeat(d + n / 2f, n) - n / 2f;

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((d + 0.5f) * larghezzaItem, 0f);

            // Gli slot sono ancorati al bordo sinistro del viewport: con 3 zollette per pagina, lo
            // slot visivamente al centro e' quello con d = 1 (non d = 0, che e' lo slot a sinistra).
            float distanzaDalCentro = Mathf.Abs(d - (zollePerPagina - 1) / 2f);
            float vicinanza = 1f - Mathf.Clamp01(distanzaDalCentro);
            item.ImpostaScalaPosizione(Mathf.Lerp(ScalaBordo, ScalaCentro, vicinanza));

            if (distanzaDalCentro < distanzaMigliore)
            {
                distanzaMigliore = distanzaDalCentro;
                indiceCentrale = item.Indice;
            }
        }

        if (nomeCentrale != null)
        {
            ZollettaData dato = indiceCentrale >= 0 && zollette != null && indiceCentrale < zollette.Length
                ? zollette[indiceCentrale]
                : null;
            nomeCentrale.text = dato != null ? dato.nome : string.Empty;
        }
    }

    private void AggiornaItems()
    {
        foreach (ZollaItem item in items)
        {
            bool piazzata = EPiazzata(item.Indice);
            bool bloccata = !EZollettaSbloccata(item.Indice);
            item.Aggiorna(piazzata, bloccata, item.Indice == selezionata);
        }
    }
}
