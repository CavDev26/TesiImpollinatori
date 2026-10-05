using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZollaItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ZollaCarousel carosello;
    private Image immagine;
    private GameObject fantasma;
    private bool piazzata;

    public int Indice { get; private set; }

    public void Init(ZollaCarousel c, int indice)
    {
        carosello = c;
        Indice = indice;
        immagine = GetComponent<Image>();
    }

    public void Aggiorna(bool piazzataAdesso, bool selezionataAdesso)
    {
        piazzata = piazzataAdesso;
        if (immagine != null)
        {
            immagine.color = piazzata ? new Color(0.45f, 0.45f, 0.45f, 0.8f) : Color.white;
        }
        transform.localScale = selezionataAdesso ? Vector3.one * 1.12f : Vector3.one;
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
        rtFantasma.sizeDelta = ((RectTransform)transform).rect.size;
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
