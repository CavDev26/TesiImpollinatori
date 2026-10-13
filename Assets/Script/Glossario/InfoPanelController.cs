using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Apertura/chiusura del pannello Info, stesso schema (overlay scuro + fade/scala) gia' usato per
// i dettagli delle zollette in DetailsPanelController.
public class InfoPanelController : MonoBehaviour
{
    [Header("Riferimenti principali")]
    public GameObject overlay;
    public GameObject infoPanel;
    public Button openButton;
    public Button closeButton;

    [Header("Animazione apertura/chiusura")]
    public float durata = 0.22f;
    [Range(0.5f, 1f)] public float scalaIniziale = 0.9f;

    private CanvasGroup overlayGroup;
    private CanvasGroup panelGroup;
    private Coroutine animazioneInCorso;

    void Start()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(Apri);
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Chiudi);
        }

        if (overlay != null)
        {
            overlay.TryGetComponent(out overlayGroup);
            if (overlay.TryGetComponent(out Button overlayButton))
            {
                overlayButton.onClick.AddListener(Chiudi);
            }
        }
        if (infoPanel != null)
        {
            infoPanel.TryGetComponent(out panelGroup);
        }

        ChiudiImmediato();
    }

    public void Apri()
    {
        if (animazioneInCorso != null)
        {
            StopCoroutine(animazioneInCorso);
        }

        if (overlay != null)
        {
            overlay.SetActive(true);
        }
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }

        animazioneInCorso = StartCoroutine(Anima(0f, 1f, null));
    }

    public void Chiudi()
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
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
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
        if (panelGroup != null)
        {
            panelGroup.alpha = valore;
            panelGroup.transform.localScale = Vector3.one * Mathf.Lerp(scalaIniziale, 1f, valore);
        }
    }
}
