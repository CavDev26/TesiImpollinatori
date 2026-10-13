using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZollaSlot : MonoBehaviour, IPointerClickHandler
{
    private ZollaCarousel carosello;
    private DetailsPanelController dettagli;
    private Image immagineVuota;
    private Color coloreVuotoOriginale;
    private Image immaginePiazzata;
    private int indiceZolla = -1;
    private ZollettaData dato;
    private Vector3 scalaArtworkTarget = Vector3.one;
    private Coroutine animazionePop;

    public bool Occupata => indiceZolla >= 0;
    public ZollettaData Dato => dato;
    public int IndiceGriglia { get; private set; } = -1;

    private const float ScalaArtworkPiazzata = 1.15f;
    // Molte immagini hanno il disegno vero e proprio concentrato nella meta' inferiore, con spazio
    // vuoto sopra: spostando un po' il riquadro verso l'alto, a sconfinare e' soprattutto la parte
    // vuota (in alto) invece del fiore (in basso), evitando sovrapposizioni brutte con la cella sotto.
    private const float ScostamentoVerticale = 0.32f;

    // Schiacciamento verticale applicato dalla griglia (HexGridGenerator.scalaVerticale), passato
    // esplicitamente: va compensato solo questo, non l'intera scala ereditata (che include anche
    // il fattore automatico del Canvas Scaler, uguale su X e Y e quindi da non toccare).
    private float scalaVerticaleGriglia = 0.9f;

    public void Init(ZollaCarousel c, DetailsPanelController d, float scalaVerticaleGriglia = 1f, int indiceGriglia = -1)
    {
        carosello = c;
        dettagli = d;
        this.scalaVerticaleGriglia = scalaVerticaleGriglia != 0f ? scalaVerticaleGriglia : 1f;
        IndiceGriglia = indiceGriglia;
        immagineVuota = GetComponent<Image>();
        if (immagineVuota != null)
        {
            coloreVuotoOriginale = immagineVuota.color;
        }
    }

    public void Riempi(int indice, ZollettaData datoZolletta)
    {
        indiceZolla = indice;
        dato = datoZolletta;

        // L'immagine vuota resta attiva e cliccabile (occupa sempre esattamente la cella): la si
        // rende solo invisibile, cosi' l'area cliccabile dell'esagono non cambia mai, anche se
        // l'immagine della zolletta piazzata sopra sconfina un po' per sembrare tutte della stessa
        // dimensione.
        if (immagineVuota != null)
        {
            Color c = coloreVuotoOriginale;
            c.a = 0f;
            immagineVuota.color = c;
        }

        Sprite sprite = datoZolletta != null ? datoZolletta.immagine : null;
        CreaImmaginePiazzataSeServe();

        immaginePiazzata.sprite = sprite;
        immaginePiazzata.gameObject.SetActive(true);

        if (animazionePop != null)
        {
            StopCoroutine(animazionePop);
        }
        animazionePop = StartCoroutine(AnimaPop());
    }

    public int Svuota()
    {
        int vecchia = indiceZolla;
        indiceZolla = -1;
        dato = null;

        if (animazionePop != null)
        {
            StopCoroutine(animazionePop);
            animazionePop = null;
        }
        if (immaginePiazzata != null)
        {
            immaginePiazzata.gameObject.SetActive(false);
        }
        if (immagineVuota != null)
        {
            immagineVuota.color = coloreVuotoOriginale;
        }

        return vecchia;
    }

    public void Rimuovi()
    {
        int vecchia = Svuota();
        if (vecchia >= 0 && carosello != null)
        {
            carosello.Libera(vecchia);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool caroselloAperto = carosello != null && carosello.Aperto;

        if (Occupata)
        {
            // Con il carosello aperto non si aprono i dettagli, per non sovrapporre i pannelli
            if (dettagli != null && !caroselloAperto)
            {
                dettagli.ApriDettagliPerSlot(this);
            }
            return;
        }

        if (caroselloAperto && carosello.ZollaSelezionata >= 0)
        {
            carosello.PiazzaInSlot(this, carosello.ZollaSelezionata);
        }
    }

    private void CreaImmaginePiazzataSeServe()
    {
        if (immaginePiazzata != null)
        {
            return;
        }

        GameObject go = new GameObject("ArtworkPiazzata", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, ScostamentoVerticale);
        rt.anchorMax = new Vector2(1f, 1f + ScostamentoVerticale);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        immaginePiazzata = go.GetComponent<Image>();
        immaginePiazzata.preserveAspect = true;
        // Solo visiva: anche se sconfina dalla cella, non deve rubare i click a cio' che sta intorno.
        immaginePiazzata.raycastTarget = false;

        // Un po' piu' grande della cella esagonale: l'esagono e' inscritto nel rettangolo, quindi
        // un'immagine grande esattamente quanto il rettangolo sembra comunque piccola dentro l'esagono.
        // Compensiamo solo lo schiacciamento verticale della griglia (scalaVerticale), cosi' l'artwork
        // piazzato resta scalato in modo uniforme invece di risultare schiacciato due volte.
        float scalaVerticaleArtwork = ScalaArtworkPiazzata / scalaVerticaleGriglia;
        scalaArtworkTarget = new Vector3(ScalaArtworkPiazzata, scalaVerticaleArtwork, ScalaArtworkPiazzata);
        rt.localScale = scalaArtworkTarget;
    }

    // Piccolo effetto "pop" quando si piazza una zolletta: parte da zero e supera un po' la
    // dimensione finale prima di assestarsi, invece di comparire di colpo alla dimensione piena.
    private IEnumerator AnimaPop()
    {
        const float durata = 0.22f;
        const float overshoot = 1.70158f;
        RectTransform rt = immaginePiazzata.rectTransform;

        float tempo = 0f;
        while (tempo < durata)
        {
            tempo += Time.deltaTime;
            float p = Mathf.Clamp01(tempo / durata);
            float pm1 = p - 1f;
            float ease = 1f + (overshoot + 1f) * pm1 * pm1 * pm1 + overshoot * pm1 * pm1;
            rt.localScale = scalaArtworkTarget * ease;
            yield return null;
        }

        rt.localScale = scalaArtworkTarget;
        animazionePop = null;
    }
}
