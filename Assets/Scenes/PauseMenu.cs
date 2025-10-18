using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public GameObject optionsMenuPanel; // Das Options-Menü innerhalb des Spiels
    public GameObject pauseButton;

    private bool isPaused = false;

    void Start()
    {
        // Panels initial ausblenden
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    void Update()
    {
        // ESC-Taste toggelt Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Wenn gerade das Optionsmenü aktiv ist → zurück ins Pausemenü
            if (optionsMenuPanel != null && optionsMenuPanel.activeSelf)
            {
                CloseOptions();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (pauseButton != null)
            pauseButton.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Öffnet das Optionsmenü innerhalb des Spiels
    public void OpenOptions()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(true);
    }

    // Wird von „Back“ im Optionsmenü aufgerufen
    public void CloseOptions()
    {
        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    // Gehe zurück ins Hauptmenü
    public void LeaveToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
