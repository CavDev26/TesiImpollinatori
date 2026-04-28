using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CambioIndice : MonoBehaviour
{
    public int adder;
    public SpriteRenderer[] spriteRenderersImp;
    public SpriteRenderer[] spriteRenderersFiori;
    public TextMeshProUGUI[] NomeImp;
    public TextMeshProUGUI[] NomeFiori;
    public Sprite None;

    public TextMeshProUGUI[] Info2;
    public void OnCambioIndice()
    {
        GlossarioController.indice += adder;
        if(GlossarioController.indice < 0)
        {
            GlossarioController.indice = 6;
        }
        if (GlossarioController.indice > 6)
        {
            GlossarioController.indice = 0;
        }
        foreach(var i in NomeImp)
        {
            i.text = "";
        }
        foreach( var i in NomeFiori)
        {
            i.text = "";
        }
        foreach( var i in spriteRenderersImp)
        {
            i.sprite = None;
        }
        foreach (var i in spriteRenderersFiori)
        {
            i.sprite = None;
        }
    }
    public void OnCambioIndiceInfo()
    {
        GlossarioController.indiceInfo += adder;
        if (GlossarioController.indiceInfo < 0)
        {
            GlossarioController.indiceInfo = 5;
        }
        if (GlossarioController.indiceInfo > 5)
        {
            GlossarioController.indiceInfo = 0;
        }
        foreach (var i in Info2)
        {
            i.text = "";
        }
    }
}
