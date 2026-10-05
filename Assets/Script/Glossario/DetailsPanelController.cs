using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DetailsPanelController : MonoBehaviour
{
    [Header("Riferimenti principali")]
    public GameObject overlay;
    public GameObject detailsPanel;
    public Button closeButton;

    [Header("Pagina 'More' (dettagli aggiuntivi)")]
    public GameObject morePanel;
    public Button nextButton;
    public Button prevButton;
    public Button moreCloseButton;

    [Header("Zolletta")]
    public Button rimuoviButton;

    [Header("Animazione apertura/chiusura")]
    public float durata = 0.22f;
    [Range(0.5f, 1f)] public float scalaIniziale = 0.9f;

    [Header("Animazione slide tra pagine")]
    public float durataSlide = 0.3f;
    [Tooltip("Quanto si spostano le pagine, come frazione della loro larghezza. Basso = restano quasi ferme e si dissolvono, alto = si spostano di piu'.")]
    [Range(0.1f, 0.6f)] public float frazioneSpostamento = 0.25f;

    private CanvasGroup overlayGroup;
    private CanvasGroup detailsGroup;
    private CanvasGroup moreGroup;
    private CanvasGroup gruppoCorrente;

    private RectTransform detailsRect;
    private RectTransform moreRect;

    private Coroutine animazioneInCorso;
    private Coroutine slideInCorso;

    private ZollaSlot slotAperto;

    void Start()
    {
        if (rimuoviButton == null && detailsPanel != null)
        {
            rimuoviButton = CreaRimuoviButton(detailsPanel.GetComponent<RectTransform>());
        }
        if (rimuoviButton != null)
        {
            rimuoviButton.onClick.AddListener(RimuoviZolla);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ChiudiDettagli);
        }
        if (moreCloseButton != null)
        {
            moreCloseButton.onClick.AddListener(ChiudiDettagli);
        }
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(VaiAMore);
        }
        if (prevButton != null)
        {
            prevButton.onClick.AddListener(TornaADettagli);
        }

        if (overlay != null)
        {
            overlay.TryGetComponent(out overlayGroup);
            if (overlay.TryGetComponent(out Button overlayButton))
            {
                overlayButton.onClick.AddListener(ChiudiDettagli);
            }
        }

        if (detailsPanel != null)
        {
            detailsPanel.TryGetComponent(out detailsGroup);
            detailsRect = detailsPanel.GetComponent<RectTransform>();
        }
        if (morePanel != null)
        {
            morePanel.TryGetComponent(out moreGroup);
            moreRect = morePanel.GetComponent<RectTransform>();
        }

        ChiudiImmediato();
    }

    private static Button CreaRimuoviButton(RectTransform genitore)
    {
        GameObject go = new GameObject("RimuoviButton", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(genitore, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(300f, 90f);
        rt.anchoredPosition = new Vector2(0f, 60f);

        Image sfondo = go.GetComponent<Image>();
        sfondo.color = new Color(0.75f, 0.3f, 0.25f);

        GameObject testoGo = new GameObject("Testo", typeof(RectTransform), typeof(Text));
        testoGo.transform.SetParent(go.transform, false);
        RectTransform trt = testoGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;

        Text t = testoGo.GetComponent<Text>();
        t.text = "Rimuovi";
        t.alignment = TextAnchor.MiddleCenter;
        t.fontSize = 48;
        t.color = Color.white;
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        t.font = font;

        Button bottone = go.GetComponent<Button>();
        bottone.targetGraphic = sfondo;
        return bottone;
    }

    public void ApriDettagli()
    {
        Apri(null);
    }

    public void ApriDettagliPerSlot(ZollaSlot slot)
    {
        Apri(slot);
    }

    public void RimuoviZolla()
    {
        if (slotAperto != null)
        {
            slotAperto.Rimuovi();
            slotAperto = null;
        }
        ChiudiDettagli();
    }

    private void Apri(ZollaSlot slot)
    {
        slotAperto = slot;

        if (animazioneInCorso != null)
        {
            StopCoroutine(animazioneInCorso);
        }
        if (slideInCorso != null)
        {
            StopCoroutine(slideInCorso);
        }

        if (morePanel != null)
        {
            morePanel.SetActive(false);
        }
        if (detailsRect != null)
        {
            detailsRect.anchoredPosition = new Vector2(0f, detailsRect.anchoredPosition.y);
        }
        gruppoCorrente = detailsGroup;

        if (overlay != null)
        {
            overlay.SetActive(true);
        }
        if (detailsPanel != null)
        {
            detailsPanel.SetActive(true);
        }

        animazioneInCorso = StartCoroutine(Anima(0f, 1f, null));
    }

    public void ChiudiDettagli()
    {
        if (animazioneInCorso != null)
        {
            StopCoroutine(animazioneInCorso);
        }

        animazioneInCorso = StartCoroutine(Anima(1f, 0f, ChiudiImmediato));
    }

    private void ChiudiImmediato()
    {
        if (overlay != null)
        {
            overlay.SetActive(false);
        }
        if (detailsPanel != null)
        {
            detailsPanel.SetActive(false);
        }
        if (morePanel != null)
        {
            morePanel.SetActive(false);
        }
    }

    public void VaiAMore()
    {
        if (slideInCorso != null)
        {
            StopCoroutine(slideInCorso);
        }
        gruppoCorrente = moreGroup;
        slideInCorso = StartCoroutine(Slide(detailsRect, detailsGroup, moreRect, moreGroup, -1f));
    }

    public void TornaADettagli()
    {
        if (slideInCorso != null)
        {
            StopCoroutine(slideInCorso);
        }
        gruppoCorrente = detailsGroup;
        slideInCorso = StartCoroutine(Slide(moreRect, moreGroup, detailsRect, detailsGroup, 1f));
    }

    // direzione -1: la pagina in uscita si sposta verso sinistra, quella in entrata arriva da destra (Next)
    // direzione  1: la pagina in uscita si sposta verso destra, quella in entrata arriva da sinistra (Prev)
    // Le pagine non escono mai dallo schermo: si spostano solo di una frazione della loro larghezza
    // mentre si dissolvono in dissolvenza incrociata, un effetto piu' morbido tipo "voltare pagina".
    private IEnumerator Slide(RectTransform uscita, CanvasGroup gruppoUscita, RectTransform entrata, CanvasGroup gruppoEntrata, float direzione)
    {
        if (uscita == null || entrata == null)
        {
            yield break;
        }

        float spostamento = uscita.rect.width * frazioneSpostamento;
        float yUscita = uscita.anchoredPosition.y;
        float yEntrata = entrata.anchoredPosition.y;

        entrata.gameObject.SetActive(true);
        entrata.anchoredPosition = new Vector2(-direzione * spostamento, yEntrata);
        entrata.localScale = Vector3.one;
        if (gruppoEntrata != null)
        {
            gruppoEntrata.alpha = 0f;
        }

        float tempo = 0f;
        while (tempo < durataSlide)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tempo / durataSlide));

            uscita.anchoredPosition = new Vector2(Mathf.Lerp(0f, direzione * spostamento, t), yUscita);
            entrata.anchoredPosition = new Vector2(Mathf.Lerp(-direzione * spostamento, 0f, t), yEntrata);

            if (gruppoUscita != null)
            {
                gruppoUscita.alpha = Mathf.Lerp(1f, 0f, t);
            }
            if (gruppoEntrata != null)
            {
                gruppoEntrata.alpha = Mathf.Lerp(0f, 1f, t);
            }

            yield return null;
        }

        entrata.anchoredPosition = new Vector2(0f, yEntrata);
        entrata.localScale = Vector3.one;
        if (gruppoEntrata != null)
        {
            gruppoEntrata.alpha = 1f;
        }

        uscita.gameObject.SetActive(false);
        uscita.anchoredPosition = new Vector2(0f, yUscita);
        uscita.localScale = Vector3.one;
        if (gruppoUscita != null)
        {
            gruppoUscita.alpha = 1f;
        }

        slideInCorso = null;
    }

    private IEnumerator Anima(float da, float a, Action alTermine)
    {
        float tempo = 0f;
        while (tempo < durata)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tempo / durata));
            ImpostaValore(Mathf.Lerp(da, a, t));
            yield return null;
        }

        ImpostaValore(a);
        animazioneInCorso = null;
        alTermine?.Invoke();
    }

    private void ImpostaValore(float valore)
    {
        if (overlayGroup != null)
        {
            overlayGroup.alpha = valore;
        }
        if (gruppoCorrente != null)
        {
            gruppoCorrente.alpha = valore;
            gruppoCorrente.transform.localScale = Vector3.one * Mathf.Lerp(scalaIniziale, 1f, valore);
        }
    }
}
