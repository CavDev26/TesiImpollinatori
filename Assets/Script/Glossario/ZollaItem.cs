using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZollaItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ZollaCarousel carosello;
    private Image immagine;
    private RectTransform immagineRect;
    private Text nomeTesto;
    private GameObject fantasma;
    private bool piazzata;

    public int Indice { get; private set; }

    // Frazione verticale del contenitore riservata all'immagine (il resto, sotto, e' per il nome).
    // Spostata un po' piu' in basso (non arriva fino a 1) cosi' l'immagine resta vicina al nome
    // invece che attaccata al bordo superiore.
    private const float AreaArtworkMinY = 0.16f;
    private const float AreaArtworkMaxY = 0.92f;

    public void Init(ZollaCarousel c, int indice, Sprite sprite, string nome)
    {
        carosello = c;
        Indice = indice;

        RectTransform contenitoreRect = (RectTransform)transform;
        float larghezzaArea = contenitoreRect.rect.width;
        float altezzaArea = contenitoreRect.rect.height * (AreaArtworkMaxY - AreaArtworkMinY);

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
        immagineRect.anchorMin = new Vector2(0.5f, (AreaArtworkMinY + AreaArtworkMaxY) / 2f);
        immagineRect.anchorMax = immagineRect.anchorMin;
        immagineRect.pivot = new Vector2(0.5f, 0.5f);
        immagineRect.sizeDelta = AdattatoreSprite.DimensioneResa(sprite, larghezzaArea, altezzaArea);

        immagine = artworkGo.GetComponent<Image>();
        immagine.sprite = sprite;
        immagine.preserveAspect = true;
        immagine.raycastTarget = false;

        GameObject nomeGo = new GameObject("Nome", typeof(RectTransform), typeof(Text));
        nomeGo.transform.SetParent(transform, false);
        RectTransform nomeRect = nomeGo.GetComponent<RectTransform>();
        nomeRect.anchorMin = new Vector2(0f, 0f);
        nomeRect.anchorMax = new Vector2(1f, 0.22f);
        nomeRect.offsetMin = Vector2.zero;
        nomeRect.offsetMax = Vector2.zero;

        nomeTesto = nomeGo.GetComponent<Text>();
        nomeTesto.text = nome;
        nomeTesto.alignment = TextAnchor.MiddleCenter;
        nomeTesto.fontSize = 22;
        nomeTesto.color = new Color(0.3f, 0.2f, 0.1f);
        nomeTesto.horizontalOverflow = HorizontalWrapMode.Wrap;
        nomeTesto.verticalOverflow = VerticalWrapMode.Truncate;
        nomeTesto.raycastTarget = false;
        nomeTesto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (nomeTesto.font == null)
        {
            nomeTesto.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

    public void Aggiorna(bool piazzataAdesso, bool selezionataAdesso)
    {
        piazzata = piazzataAdesso;
        if (immagine != null)
        {
            immagine.color = piazzata ? new Color(0.45f, 0.45f, 0.45f, 0.8f) : Color.white;
        }
        if (nomeTesto != null)
        {
            nomeTesto.color = piazzata ? new Color(0.5f, 0.5f, 0.5f, 0.8f) : new Color(0.3f, 0.2f, 0.1f);
        }
        transform.localScale = selezionataAdesso ? Vector3.one * 1.08f : Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (carosello != null)
        {
            carosello.Seleziona(Indice);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (piazzata || immagine == null)
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
