using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZollaItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ZollaCarousel carosello;
    private Image immagine;
    private RectTransform immagineRect;
    private GameObject fantasma;
    private bool piazzata;
    private bool bloccata;

    public int Indice { get; private set; }

    // Frazione verticale del contenitore riservata all'immagine (il resto, sotto, e' per il nome).
    // Spostata un po' piu' in basso (non arriva fino a 1) cosi' l'immagine resta vicina al nome
    // invece che attaccata al bordo superiore.
    private const float AreaArtworkMinY = 0.16f;
    private const float AreaArtworkMaxY = 0.92f;
    private const float ScalaArtwork = 1.25f;

    private float scalaPosizione = 1f;
    private bool selezionata;

    // Materiale condiviso che desatura l'immagine (vedi Assets/Shaders/UIGrayscale.shader): usato
    // per le zollette non ancora sbloccate, cosi' sono davvero in bianco e nero e non solo piu' scure.
    private static Material materialeGrigio;

    private static Material OttieniMaterialeGrigio()
    {
        if (materialeGrigio == null)
        {
            Shader shader = Shader.Find("UI/Grayscale");
            if (shader != null)
            {
                materialeGrigio = new Material(shader);
            }
        }
        return materialeGrigio;
    }

    public void Init(ZollaCarousel c, int indice, Sprite sprite)
    {
        carosello = c;
        Indice = indice;

        // Zona cliccabile dedicata, sempre grande esattamente quanto la cella: l'immagine sopra puo'
        // sconfinare un po' per sembrare tutte della stessa dimensione, senza rubare i click a quello
        // che sta intorno (altre zollette, pulsanti...).
        GameObject zonaClicGo = new GameObject("ZonaClic", typeof(RectTransform), typeof(Image));
        zonaClicGo.transform.SetParent(transform, false);
        RectTransform zonaClicRect = zonaClicGo.GetComponent<RectTransform>();
        zonaClicRect.anchorMin = Vector2.zero;
        zonaClicRect.anchorMax = Vector2.one;
        zonaClicRect.offsetMin = Vector2.zero;
        zonaClicRect.offsetMax = Vector2.zero;
        Image zonaClicImmagine = zonaClicGo.GetComponent<Image>();
        zonaClicImmagine.color = Color.clear;

        GameObject artworkGo = new GameObject("Artwork", typeof(RectTransform), typeof(Image));
        artworkGo.transform.SetParent(transform, false);
        immagineRect = artworkGo.GetComponent<RectTransform>();
        immagineRect.anchorMin = new Vector2(0f, AreaArtworkMinY);
        immagineRect.anchorMax = new Vector2(1f, AreaArtworkMaxY);
        immagineRect.offsetMin = Vector2.zero;
        immagineRect.offsetMax = Vector2.zero;

        immagine = artworkGo.GetComponent<Image>();
        immagine.sprite = sprite;
        immagine.preserveAspect = true;
        immagine.raycastTarget = false;
        immagineRect.localScale = Vector3.one * ScalaArtwork;
    }

    public void Aggiorna(bool piazzataAdesso, bool bloccataAdesso, bool selezionataAdesso)
    {
        piazzata = piazzataAdesso;
        bloccata = bloccataAdesso;
        selezionata = selezionataAdesso;

        if (immagine != null)
        {
            if (bloccata)
            {
                immagine.material = OttieniMaterialeGrigio();
                immagine.color = Color.white;
            }
            else
            {
                immagine.material = null;
                immagine.color = piazzata ? new Color(0.45f, 0.45f, 0.45f, 0.8f) : Color.white;
            }
        }
        AggiornaScalaVisiva();
    }

    // Scala "di posizione" (quanto e' vicina al centro del carosello, impostata da ZollaCarousel)
    // ed enfasi da selezione si moltiplicano insieme, invece di sovrascriversi a vicenda.
    public void ImpostaScalaPosizione(float scala)
    {
        scalaPosizione = scala;
        AggiornaScalaVisiva();
    }

    private void AggiornaScalaVisiva()
    {
        transform.localScale = Vector3.one * scalaPosizione * (selezionata ? 1.08f : 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (bloccata)
        {
            if (carosello != null)
            {
                carosello.MostraDettagliBloccata(Indice);
            }
            return;
        }
        if (carosello != null)
        {
            carosello.Seleziona(Indice);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bloccata || piazzata || immagine == null)
        {
            return;
        }

        Canvas canvasRadice = GetComponentInParent<Canvas>().rootCanvas;
        fantasma = new GameObject("Fantasma", typeof(RectTransform), typeof(Image));
        fantasma.transform.SetParent(canvasRadice.transform, false);
        fantasma.transform.SetAsLastSibling();

        Image imgFantasma = fantasma.GetComponent<Image>();
        imgFantasma.sprite = immagine.sprite;
        imgFantasma.preserveAspect = true;
        imgFantasma.raycastTarget = false;

        RectTransform rtFantasma = fantasma.GetComponent<RectTransform>();
        rtFantasma.sizeDelta = immagineRect.rect.size;
        rtFantasma.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (fantasma != null)
        {
            fantasma.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (fantasma != null)
        {
            Destroy(fantasma);
            fantasma = null;
        }

        if (carosello == null || piazzata)
        {
            return;
        }

        ZollaSlot slot = TrovaSlot(eventData);
        if (slot != null)
        {
            carosello.PiazzaInSlot(slot, Indice);
        }
    }

    private static ZollaSlot TrovaSlot(PointerEventData eventData)
    {
        if (EventSystem.current == null)
        {
            return null;
        }

        List<RaycastResult> risultati = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, risultati);
        foreach (RaycastResult risultato in risultati)
        {
            if (risultato.gameObject != null && risultato.gameObject.TryGetComponent(out ZollaSlot slot))
            {
                return slot;
            }
        }
        return null;
    }
}
