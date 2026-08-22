using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HexGridGenerator : MonoBehaviour
{
    [Header("Riferimenti")]
    public RectTransform zolleContainer;
    public GameObject esagonoPrefab;

    [Header("Forma della griglia (a margherita)")]
    [Tooltip("0 = solo il centro, 1 = centro + 6 attorno (margherita da 7), 2 = un altro anello di 12 (19 totali), ...")]
    [Min(0)] public int raggio = 1;

    [Header("Dimensioni Esagoni (flat-top)")]
    [Range(60f, 400f)] public float hexSize = 190f;
    [Tooltip("Distanza tra le celle. 1 = adiacenti. Valori piu' bassi o piu' alti possono sovrapporle o allontanarle: regola a occhio.")]
    [Range(0.5f, 2f)] public float spaziatura = 1.3f;

    [Header("Prospettiva")]
    [Tooltip("Schiaccia verticalmente l'intera griglia per simulare una vista leggermente inclinata invece che dall'alto.")]
    [Range(0.6f, 1f)] public float scalaVerticale = 0.88f;
    [Tooltip("Spessore della parete 3D del mattoncino esagonale, come frazione dell'altezza dell'esagono.")]
    [Range(0f, 0.6f)] public float profondita3D = 0.4f;

    void Start()
    {
        GeneraGriglia();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null)
            {
                return;
            }
            GeneraGriglia();
        };
    }
#endif

    [ContextMenu("Rigenera Griglia")]
    public void GeneraGriglia()
    {
        if (zolleContainer == null || esagonoPrefab == null)
        {
            return;
        }

        for (int i = zolleContainer.childCount - 1; i >= 0; i--)
        {
            GameObject figlio = zolleContainer.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                Destroy(figlio);
            }
            else
            {
                DestroyImmediate(figlio);
            }
        }

        zolleContainer.localScale = new Vector3(1f, scalaVerticale, 1f);

        float width = hexSize * 2f;
        float height = Mathf.Sqrt(3f) * hexSize;
        float profonditaPixel = height * profondita3D;

        float passoVerticale = height * spaziatura;
        float passoOrizzontale = hexSize * 1.5f * spaziatura;

        int indice = 0;
        for (int q = -raggio; q <= raggio; q++)
        {
            int r1 = Mathf.Max(-raggio, -q - raggio);
            int r2 = Mathf.Min(raggio, -q + raggio);

            for (int r = r1; r <= r2; r++)
            {
                float x = passoOrizzontale * q;
                float y = -passoVerticale * (r + q * 0.5f);

                GameObject istanza = Instantiate(esagonoPrefab, zolleContainer);
                istanza.name = $"Esagono_{q}_{r}";
                istanza.hideFlags = HideFlags.DontSaveInEditor;

                RectTransform rt = istanza.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x, y);
                rt.sizeDelta = new Vector2(width, height);

                if (istanza.TryGetComponent(out HexagonGraphic grafica))
                {
                    grafica.profondita = profonditaPixel;
                    grafica.SetVerticesDirty();
                }

                int indiceCatturato = indice;
                if (istanza.TryGetComponent(out Button bottone))
                {
                    bottone.onClick.AddListener(() => OnEsagonoCliccato(indiceCatturato));
                }

                indice++;
            }
        }
    }

    private void OnEsagonoCliccato(int indice)
    {
        // Punto di estensione: in futuro apre il menu di selezione zollette per riempire questa casella
        Debug.Log($"Casella {indice} selezionata");
    }
}
