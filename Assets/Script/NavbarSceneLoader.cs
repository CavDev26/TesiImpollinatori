using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NavbarSceneLoader : MonoBehaviour
{
    [Header("Bottoni Navbar")]
    public Button btnHome;
    public Button btnGarden;
    public Button btnMinigames;

    [Header("Nomi Esatti delle Scene nei Build Settings")]
    public string homeSceneName = "Home";
    public string gardenSceneName = "NewGlossario";
    public string minigamesSceneName = "MinigamesMenu";

    [Header("Indicatori pagina attiva")]
    [Tooltip("Un indicatore per bottone (figlio del bottone stesso, cosi' segue la sua posizione nel layout). Si mostra/nasconde in base alla pagina corrente, nessuna animazione.")]
    public GameObject indicatoreHome;
    public GameObject indicatoreGarden;
    public GameObject indicatoreMinigames;

    private void Start()
    {
        if (btnHome != null)
            btnHome.onClick.AddListener(() => LoadScene(homeSceneName));

        if (btnGarden != null)
            btnGarden.onClick.AddListener(() => LoadScene(gardenSceneName));

        if (btnMinigames != null)
            btnMinigames.onClick.AddListener(() => LoadScene(minigamesSceneName));

        AggiornaIndicatori();
    }

    private void LoadScene(string sceneName)
    {
        // Carica la scena solo se non ci troviamo gi� l�
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void AggiornaIndicatori()
    {
        string sceneAttiva = SceneManager.GetActiveScene().name;

        MostraIndicatore(indicatoreHome, sceneAttiva == homeSceneName);
        MostraIndicatore(indicatoreGarden, sceneAttiva == gardenSceneName);
        MostraIndicatore(indicatoreMinigames, sceneAttiva == minigamesSceneName);
    }

    private void MostraIndicatore(GameObject indicatore, bool mostra)
    {
        if (indicatore == null)
        {
            return;
        }

        if (mostra)
        {
            indicatore.SetActive(true);
            StartCoroutine(AnimaComparsa(indicatore.GetComponent<RectTransform>()));
        }
        else
        {
            indicatore.SetActive(false);
        }
    }

    // Parte come un piccolo cerchio (scala orizzontale a zero, i due bordi arrotondati della
    // pillola si uniscono) e si allarga fino alla larghezza piena.
    private IEnumerator AnimaComparsa(RectTransform rt)
    {
        const float durata = 0.18f;
        rt.localScale = new Vector3(0f, 1f, 1f);

        float tempo = 0f;
        while (tempo < durata)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tempo / durata));
            rt.localScale = new Vector3(t, 1f, 1f);
            yield return null;
        }

        rt.localScale = Vector3.one;
    }
}