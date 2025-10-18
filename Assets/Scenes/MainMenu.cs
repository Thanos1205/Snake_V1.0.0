using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject buttonContainer;

    void Awake()
    {
        // Gespeicherte Einstellungen anwenden
        OptionsMenu.ApplySavedFramerate();
        OptionsMenu.ApplySavedResolution();

        // Wenn wir aus dem Spiel kommen und Options geöffnet werden soll
        if (PlayerPrefs.GetInt("OpenOptionsOnStart", 0) == 1)
        {
            PlayerPrefs.SetInt("OpenOptionsOnStart", 0);
            PlayerPrefs.Save();

            OpenOptions();
        }
    }

    public void PlayGame()
    {
        // Aktuelle Szene laden, dann die nächste Szene im Build
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // Prüfen, ob die nächste Szene existiert
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Keine nächste Szene im Build vorhanden!");
        }
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        buttonContainer.SetActive(false);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        buttonContainer.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
