using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Collega i due bottoni dei cartelli della Home (vai ai minigiochi / vai al giardino) e mostra
// quante zollette sono sbloccate, stesso genere di collegamento gia' fatto in NavbarSceneLoader.
public class CartelliHomeController : MonoBehaviour
{
    [Header("Bottoni")]
    public Button bottoneMinigiochi;
    public Button bottoneGiardino;

    [Header("Nomi Esatti delle Scene nei Build Settings")]
    public string gardenSceneName = "NewGlossario";
    public string minigamesSceneName = "MinigamesMenu";

    [Header("Contatore zollette sbloccate")]
    [Tooltip("Le stesse 7 schede ZollettaData assegnate nel carosello del giardino.")]
    public ZollettaData[] zollette;
    public TMP_Text testoContatore;

    void Start()
    {
        if (bottoneMinigiochi != null)
        {
            bottoneMinigiochi.onClick.AddListener(() => SceneManager.LoadScene(minigamesSceneName));
        }
        if (bottoneGiardino != null)
        {
            bottoneGiardino.onClick.AddListener(() => SceneManager.LoadScene(gardenSceneName));
        }

        AggiornaContatore();
    }

    private void AggiornaContatore()
    {
        if (testoContatore == null)
        {
            return;
        }
        if (zollette == null)
        {
            testoContatore.text = string.Empty;
            return;
        }

        int sbloccate = 0;
        foreach (ZollettaData zolletta in zollette)
        {
            if (GiardinoSalvataggio.EZollettaSbloccata(zolletta, zollette))
            {
                sbloccate++;
            }
        }

        int dasbloccare = zollette.Length - sbloccate;
        testoContatore.text = $"Ti restano ancora\n{dasbloccare}\nzollette da sbloccare!";
    }
}
