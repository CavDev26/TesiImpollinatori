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
    private RectTransform rettangoloProprio;
    private int indiceZolla = -1;
    private ZollettaData dato;

    public bool Occupata => indiceZolla >= 0;
    public ZollettaData Dato => dato;

    public void Init(ZollaCarousel c, DetailsPanelController d)
    {
        carosello = c;
        dettagli = d;
        immagineVuota = GetComponent<Image>();
        rettangoloProprio = GetComponent<RectTransform>();
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

        if (rettangoloProprio != null)
        {
            Vector2 dimensione = AdattatoreSprite.DimensioneResa(sprite, rettangoloProprio.rect.width, rettangoloProprio.rect.height);
            immaginePiazzata.rectTransform.sizeDelta = dimensione;
        }
    }

    public int Svuota()
    {
        int vecchia = indiceZolla;
        indiceZolla = -1;
        dato = null;

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
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        immaginePiazzata = go.GetComponent<Image>();
        immaginePiazzata.preserveAspect = true;
        // Solo visiva: anche se sconfina dalla cella, non deve rubare i click a cio' che sta intorno.
        immaginePiazzata.raycastTarget = false;
    }
}
