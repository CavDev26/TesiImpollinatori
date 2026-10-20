using UnityEngine;
using UnityEngine.UI;

// Apertura/chiusura del pannello Info: overlay scuro + fade/scala condivisi con DetailsPanelController
// tramite TweenUtil.
public class InfoPanelController : MonoBehaviour
{
    [Header("Riferimenti principali")]
    public GameObject overlay;
    public GameObject infoPanel;
    public Button openButton;
    public Button closeButton;
    [Tooltip("Se assegnato, si chiude automaticamente quando si apre questo pannello (altrimenti resta visibile e cliccabile sotto).")]
    public ZollaCarousel carosello;

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

        if (carosello != null)
        {
            carosello.Chiudi();
        }

        if (overlay != null)
        {
            overlay.SetActive(true);
        }
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }

        animazioneInCorso = StartCoroutine(TweenUtil.FadeScala(durata, scalaIniziale, 0f, 1f, overlayGroup, panelGroup,
            () => animazioneInCorso = null));
    }

    public void Chiudi()
    {
        if (animazioneInCorso != null)
        {
            StopCoroutine(animazioneInCorso);
        }

        animazioneInCorso = StartCoroutine(TweenUtil.FadeScala(durata, scalaIniziale, 1f, 0f, overlayGroup, panelGroup,
            () => { animazioneInCorso = null; ChiudiImmediato(); }));
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
}
