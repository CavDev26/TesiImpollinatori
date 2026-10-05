using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZollaCarousel : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("Il prefab CaroselloZollette messo in scena. Viewport, frecce e X vengono trovati da soli dentro di lui.")]
    public GameObject pannello;
    public Button pulsantePiu;

    [Header("Zollette disponibili (placeholder)")]
    [Tooltip("Immagine usata per tutte le zollette finche' non ci sono gli artwork veri. Se 'Zolle Sprites' e' vuoto, viene ripetuta 'Numero Default' volte.")]
    public Sprite spriteDefault;
    [Min(1)] public int numeroDefault = 8;
    public Sprite[] zolleSprites;

    [Header("Carosello")]
    [Min(1)] public int zollePerPagina = 3;
    public float durataScorrimento = 0.25f;

    private RectTransform viewport;
    private RectTransform contenuto;
    private readonly List<ZollaItem> items = new List<ZollaItem>();
    private Sprite[] sprites;
    private bool[] piazzate;
    private int selezionata = -1;
    private float posizione;
    private float destinazione;
    private bool costruito;
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

        pannello.SetActive(true);
        if (!costruito)
        {
            Costruisci();
        }
    }

    public void Chiudi()
    {
        if (pannello == null)
        {
            return;
        }

        selezionata = -1;
        pannello.SetActive(false);
        AggiornaItems();
    }

    public void Seleziona(int indice)
    {
        if (piazzate == null || indice < 0 || indice >= piazzate.Length || piazzate[indice])
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

        if (slot.Occupata)
        {
            int vecchia = slot.Svuota();
            if (vecchia >= 0)
            {
                piazzate[vecchia] = false;
            }
        }

        slot.Riempi(indice, sprites[indice]);
        piazzate[indice] = true;

        if (selezionata == indice)
        {
            selezionata = -1;
        }
        AggiornaItems();
    }

    public void Libera(int indice)
    {
        if (piazzate != null && indice >= 0 && indice < piazzate.Length)
        {
            piazzate[indice] = false;
        }
        AggiornaItems();
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

    private Sprite[] ZolleDaMostrare()
    {
        if (zolleSprites != null && zolleSprites.Length > 0)
        {
            return zolleSprites;
        }

        Sprite[] lista = new Sprite[numeroDefault];
        for (int i = 0; i < lista.Length; i++)
        {
            lista[i] = spriteDefault;
        }
        return lista;
    }

    private void Costruisci()
    {
        costruito = true;
        if (viewport == null || contenuto == null)
        {
            Debug.LogWarning("ZollaCarousel: nel prefab manca Viewport/Contenuto.");
            return;
        }

        Canvas.ForceUpdateCanvases();

        sprites = ZolleDaMostrare();
        piazzate = new bool[sprites.Length];

        float larghezzaItem = viewport.rect.width / zollePerPagina;
        float altezza = viewport.rect.height;

        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject go = new GameObject("Zolla_" + i, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(contenuto, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(larghezzaItem * 0.9f, altezza * 0.9f);

            Image immagine = go.GetComponent<Image>();
            immagine.sprite = sprites[i];
            immagine.preserveAspect = true;

            ZollaItem item = go.AddComponent<ZollaItem>();
            item.Init(this, i);
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

    private void AggiornaPosizioni()
    {
        int n = items.Count;
        if (n == 0 || viewport == null)
        {
            return;
        }

        float larghezzaItem = viewport.rect.width / zollePerPagina;
        foreach (ZollaItem item in items)
        {
            float d = item.Indice - posizione;
            d = Mathf.Repeat(d + n / 2f, n) - n / 2f;

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((d + 0.5f) * larghezzaItem, 0f);
        }
    }

    private void AggiornaItems()
    {
        foreach (ZollaItem item in items)
        {
            bool piazzata = EPiazzata(item.Indice);
            item.Aggiorna(piazzata, item.Indice == selezionata);
        }
    }
}
