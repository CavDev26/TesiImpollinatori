using UnityEngine;

// Calcola a che dimensione disegnare uno sprite perche' il suo contenuto VISIBILE (non trasparente)
// riempia al meglio un riquadro larghezza x altezza, invece di basarsi sulla dimensione del file.
// Serve perche' i disegni delle zollette hanno margini trasparenti diversi tra loro: usando solo
// "preserve aspect" sulla dimensione piena del file, zollette con piu' margine risultano piu' piccole
// di quelle con meno margine, anche se il riquadro e' lo stesso.
public static class AdattatoreSprite
{
    // Quanto puo' al massimo sporgere dal riquadro rispetto alla dimensione che ci starebbe
    // perfettamente dentro. 1.2 = al massimo il 20% in piu'.
    private const float SporgenzaMassima = 1.2f;

    public static Vector2 DimensioneResa(Sprite sprite, float larghezza, float altezza)
    {
        if (sprite == null || larghezza <= 0f || altezza <= 0f)
        {
            return new Vector2(larghezza, altezza);
        }

        Vector2 contenuto = sprite.bounds.size;
        Vector2 intero = sprite.rect.size / sprite.pixelsPerUnit;

        // Se lo sprite non e' importato come "Tight" (o e' completamente trasparente), bounds
        // coincide con l'intero rettangolo: in quel caso ci si comporta come un normale "preserve aspect".
        if (contenuto.x <= 0f || contenuto.y <= 0f)
        {
            contenuto = intero;
        }

        // Due modi di calcolare la scala:
        // - "contenimento": il contenuto non esce mai dal riquadro, ma forme molto diverse tra loro
        //   (es. un cespuglio alto e stretto contro uno tondeggiante) finiscono per occupare aree
        //   molto diverse, anche se "riempiono al meglio" allo stesso modo.
        // - "area": punta a far occupare al contenuto la stessa area visiva indipendentemente dalla
        //   forma, ma puo' sporgere leggermente dal riquadro per le forme piu' estreme.
        // Si usa una via di mezzo tra le due: le zollette risultano piu' uniformi in dimensione,
        // senza sporgere troppo.
        float scalaContenimento = Mathf.Min(larghezza / contenuto.x, altezza / contenuto.y);
        float scalaArea = Mathf.Sqrt((larghezza * altezza) / (contenuto.x * contenuto.y));
        float scala = (scalaContenimento + scalaArea) / 2f;

        // Tetto massimo: non puo' comunque superare di troppo il riquadro, anche per le forme piu'
        // estreme (es. un contenuto quasi quadrato dentro un riquadro stretto e alto).
        scala = Mathf.Min(scala, scalaContenimento * SporgenzaMassima);

        return intero * scala;
    }
}
