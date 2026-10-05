using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZollaSlot : MonoBehaviour, IPointerClickHandler
{
    private ZollaCarousel carosello;
    private DetailsPanelController dettagli;
    private Image immagine;
    private Sprite spriteVuoto;
    private int indiceZolla = -1;

    public bool Occupata => indiceZolla >= 0;

    public void Init(ZollaCarousel c, DetailsPanelController d)
    {
        carosello = c;
        dettagli = d;
        immagine = GetComponent<Image>();
        if (immagine != null)
        {
            spriteVuoto = immagine.sprite;
            immagine.preserveAspect = true;
        }
    }

    public void Riempi(int indice, Sprite sprite)
    {
        indiceZolla = indice;
        if (immagine != null)
        {
            immagine.sprite = sprite;
        }
    }

    public int Svuota()
    {
        int vecchia = indiceZolla;
        indiceZolla = -1;
        if (immagine != null)
        {
            immagine.sprite = spriteVuoto;
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
}
