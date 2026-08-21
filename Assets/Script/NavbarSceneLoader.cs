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

    private void Start()
    {
        if (btnHome != null)
            btnHome.onClick.AddListener(() => LoadScene(homeSceneName));

        if (btnGarden != null)
            btnGarden.onClick.AddListener(() => LoadScene(gardenSceneName));

        if (btnMinigames != null)
            btnMinigames.onClick.AddListener(() => LoadScene(minigamesSceneName));
    }

    private void LoadScene(string sceneName)
    {
        // Carica la scena solo se non ci troviamo già lì
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}