using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Hexagon Graphic")]
public class HexagonGraphic : MaskableGraphic
{
    [Header("Bordo (rilievo)")]
    [Range(0f, 0.3f)] public float spessoreBordo = 0.08f;
    public Color coloreBordoLuce = new Color(0.55f, 0.4f, 0.24f);
    public Color coloreBordoOmbra = new Color(0.28f, 0.18f, 0.1f);

    [Header("Terra (superficie interna)")]
    public Color coloreLuce = new Color(0.62f, 0.45f, 0.28f);
    public Color coloreOmbra = new Color(0.45f, 0.31f, 0.18f);

    [Header("Spessore 3D")]
    [Tooltip("Altezza in pixel della parete laterale visibile sotto la casella. Impostata automaticamente da HexGridGenerator quando presente.")]
    public float profondita = 60f;
    public Color coloreParete = new Color(0.16f, 0.1f, 0.05f);

    // Direzione della luce simulata (in alto a sinistra), usata per dare rilievo al bordo.
    private const float AngoloLuceGradi = 135f;

    // indici dei vertici da 180 a 360 gradi: i tre lati "vicini" all'osservatore, quelli che nel
    // mattoncino esagonale mostrano la faccia laterale (sinistra, frontale, destra).
    private static readonly int[] BordoVicino = { 3, 4, 5, 0 };

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = GetPixelAdjustedRect();
        Vector2 centro = new Vector2(r.x + r.width / 2f, r.y + r.height / 2f);
        float rx = r.width / 2f;
        float ry = r.height / 2f;

        Vector2[] verticiBordo = CalcolaVertici(rx, ry);
        Color[] coloriBordo = new Color[6];
        for (int i = 0; i < 6; i++)
        {
            float angolo = 60 * i;
            float luce = (Mathf.Cos(Mathf.Deg2Rad * (angolo - AngoloLuceGradi)) + 1f) / 2f;
            coloriBordo[i] = Color.Lerp(coloreBordoOmbra, coloreBordoLuce, luce);
        }

        if (profondita > 0f)
        {
            for (int i = 0; i < BordoVicino.Length - 1; i++)
            {
                int a = BordoVicino[i];
                int b = BordoVicino[i + 1];
                Vector2 alto1 = centro + verticiBordo[a];
                Vector2 alto2 = centro + verticiBordo[b];
                AggiungiParete(vh, alto1, alto2, coloriBordo[a], coloriBordo[b], coloreParete, profondita);
            }
        }

        AggiungiEsagono(vh, centro, verticiBordo, coloriBordo);

        float scala = 1f - Mathf.Clamp01(spessoreBordo);
        if (scala > 0f)
        {
            Vector2[] verticiInterni = CalcolaVertici(rx * scala, ry * scala);
            float ryInterno = ry * scala;
            Color[] coloriInterni = new Color[6];
            for (int i = 0; i < 6; i++)
            {
                float t = ryInterno > 0f ? (verticiInterni[i].y / ryInterno + 1f) / 2f : 0.5f;
                coloriInterni[i] = Color.Lerp(coloreOmbra, coloreLuce, t);
            }
            AggiungiEsagono(vh, centro, verticiInterni, coloriInterni);
        }
    }

    private static Vector2[] CalcolaVertici(float rx, float ry)
    {
        const float invSin60 = 1.1547005f; // 1 / sin(60 gradi)
        Vector2[] vertici = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float angolo = Mathf.Deg2Rad * (60 * i);
            vertici[i] = new Vector2(rx * Mathf.Cos(angolo), ry * invSin60 * Mathf.Sin(angolo));
        }
        return vertici;
    }

    private static void AggiungiEsagono(VertexHelper vh, Vector2 centro, Vector2[] vertici, Color[] colori)
    {
        Color colorCentro = Color.black;
        for (int i = 0; i < 6; i++)
        {
            colorCentro += colori[i];
        }
        colorCentro /= 6f;

        vh.AddVert(CreaVertice(centro, colorCentro));
        int indiceCentro = vh.currentVertCount - 1;

        int primo = vh.currentVertCount;
        for (int i = 0; i < 6; i++)
        {
            vh.AddVert(CreaVertice(centro + vertici[i], colori[i]));
        }

        for (int i = 0; i < 6; i++)
        {
            int a = primo + i;
            int b = primo + (i + 1) % 6;
            vh.AddTriangle(indiceCentro, a, b);
        }
    }

    private static void AggiungiParete(VertexHelper vh, Vector2 alto1, Vector2 alto2, Color colAlto1, Color colAlto2, Color colBasso, float profondita)
    {
        Vector2 basso1 = alto1 + Vector2.down * profondita;
        Vector2 basso2 = alto2 + Vector2.down * profondita;

        int i0 = vh.currentVertCount;
        vh.AddVert(CreaVertice(alto1, colAlto1));
        vh.AddVert(CreaVertice(alto2, colAlto2));
        vh.AddVert(CreaVertice(basso2, colBasso));
        vh.AddVert(CreaVertice(basso1, colBasso));

        vh.AddTriangle(i0, i0 + 1, i0 + 2);
        vh.AddTriangle(i0, i0 + 2, i0 + 3);
    }

    private static UIVertex CreaVertice(Vector2 posizione, Color colore)
    {
        UIVertex v = UIVertex.simpleVert;
        v.position = posizione;
        v.color = colore;
        return v;
    }
}
