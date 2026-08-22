using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Sfondo Prato Graphic")]
public class SfondoPratoGraphic : MaskableGraphic
{
    [Header("Gradiente verticale")]
    public Color coloreAlto = new Color(0.87f, 0.93f, 0.82f);
    public Color coloreBasso = new Color(0.56f, 0.74f, 0.42f);

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = GetPixelAdjustedRect();

        UIVertex bassoSx = UIVertex.simpleVert;
        bassoSx.position = new Vector2(r.x, r.y);
        bassoSx.color = coloreBasso;

        UIVertex bassoDx = UIVertex.simpleVert;
        bassoDx.position = new Vector2(r.x + r.width, r.y);
        bassoDx.color = coloreBasso;

        UIVertex altoDx = UIVertex.simpleVert;
        altoDx.position = new Vector2(r.x + r.width, r.y + r.height);
        altoDx.color = coloreAlto;

        UIVertex altoSx = UIVertex.simpleVert;
        altoSx.position = new Vector2(r.x, r.y + r.height);
        altoSx.color = coloreAlto;

        vh.AddVert(bassoSx);
        vh.AddVert(bassoDx);
        vh.AddVert(altoDx);
        vh.AddVert(altoSx);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
    }
}
