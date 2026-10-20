using System;
using System.Collections;
using UnityEngine;

// Animazione "fade + scala" condivisa (overlay scuro che sfuma + pannello che sfuma e si ingrandisce
// leggermente), usata da DetailsPanelController e InfoPanelController invece di duplicare lo stesso
// ciclo coroutine in entrambi.
public static class TweenUtil
{
    public static IEnumerator FadeScala(float durata, float scalaIniziale, float da, float a,
        CanvasGroup overlay, CanvasGroup pannello, Action alTermine)
    {
        float tempo = 0f;
        while (tempo < durata)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tempo / durata));
            ImpostaValore(Mathf.Lerp(da, a, t), scalaIniziale, overlay, pannello);
            yield return null;
        }

        ImpostaValore(a, scalaIniziale, overlay, pannello);
        alTermine?.Invoke();
    }

    private static void ImpostaValore(float valore, float scalaIniziale, CanvasGroup overlay, CanvasGroup pannello)
    {
        if (overlay != null)
        {
            overlay.alpha = valore;
        }
        if (pannello != null)
        {
            pannello.alpha = valore;
            pannello.transform.localScale = Vector3.one * Mathf.Lerp(scalaIniziale, 1f, valore);
        }
    }
}
