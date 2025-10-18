using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public TMP_Dropdown framerateDropdown;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // ---------- Volume ----------
        // Lautstärke initialisieren, Slider auf aktuelle Lautstärke setzen
        if (volumeSlider != null)
            volumeSlider.value = AudioListener.volume;

        // OnValueChanged verbinden
        volumeSlider.onValueChanged.AddListener(SetVolume);


        // ---------- FPS ----------
        // FPS-Dropdown vorbereiten
        framerateDropdown.ClearOptions();
        List<string> fpsOptions = new List<string> { "30", "60", "75", "120", "144", "240", "VSync" };
        framerateDropdown.AddOptions(fpsOptions);

        string savedFPS = PlayerPrefs.GetString("FPS", "VSync");
        int fpsIndex = fpsOptions.FindIndex(o => o == savedFPS);
        if (fpsIndex < 0) fpsIndex = fpsOptions.Count - 1;

        framerateDropdown.value = fpsIndex;
        framerateDropdown.RefreshShownValue();

        framerateDropdown.onValueChanged.AddListener(SetFramerate);
        ApplyFramerate(fpsIndex);


        // ---------- Resolution ----------
        // Alle verfügbaren Monitorauflösungen abfragen
        resolutions = Screen.resolutions;

        // Default nur beim allerersten Start setzen
        if (!PlayerPrefs.HasKey("ResolutionWidth") || !PlayerPrefs.HasKey("ResolutionHeight"))
        {
            PlayerPrefs.SetInt("ResolutionWidth", 1920);
            PlayerPrefs.SetInt("ResolutionHeight", 1080);
            PlayerPrefs.Save();
        }

        int savedWidth = PlayerPrefs.GetInt("ResolutionWidth");
        int savedHeight = PlayerPrefs.GetInt("ResolutionHeight");

        List<string> resOptions = new List<string>();
        HashSet<string> seen = new HashSet<string>();
        int currentResolutionIndex = 0;

        // Fülle Dropdown und finde den Index der gespeicherten Auflösung
        for (int i = 0; i < resolutions.Length; i++)
        {
            int w = resolutions[i].width;
            int h = resolutions[i].height;

            // Seitenverhältnis prüfen
            float ratio = (float)w / h;
            bool is16by9 = Mathf.Abs(ratio - (16f / 9f)) < 0.02f; // kleine Toleranz

            string option = w + " x " + h;
            if (!seen.Contains(option))
            {
                // 16:9 nur für Vollbild ohne Balken
                resOptions.Add(option);
                seen.Add(option);
            }

            // Gespeicherte Auflösung auswählen
            if (w == savedWidth && h == savedHeight)
            {
                currentResolutionIndex = resOptions.IndexOf(option);
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // Disable the resolution dropdown in-game
            resolutionDropdown.interactable = false;
        }
        else
        {
            // Im MainMenu darf man die Auflösung ändern
            resolutionDropdown.interactable = true;
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        // Gespeicherte Auflösung anwenden
        ApplyResolution(currentResolutionIndex);

    }


    // ---------- Volume ----------
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }


    // ---------- FPS ----------
    public void SetFramerate(int index)
    {
        ApplyFramerate(index);

        // Save selected option
        PlayerPrefs.SetString("FPS", framerateDropdown.options[index].text);
        PlayerPrefs.Save();

        // Force UI to refresh the visible text
        framerateDropdown.RefreshShownValue();
    }

    private void ApplyFramerate(int index)
    {
        string selected = framerateDropdown.options[index].text;

        if (selected == "VSync")
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
        }
        else
        {
            int fps = int.Parse(selected);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = fps;
        }

        Debug.Log($"Framerate applied: {Application.targetFrameRate}");
    }

    public static void ApplySavedFramerate()
    {
        string saved = PlayerPrefs.GetString("FPS", "VSync");

        if (saved == "VSync")
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
        }
        else
        {
            if (int.TryParse(saved, out int fps))
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = fps;
            }
            else
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
            }
        }

        Debug.Log($"Applied saved framerate at startup: {Application.targetFrameRate}");
    }


    // ---------- Auflösung ----------
    public void SetResolution(int index)
    {
        ApplyResolution(index);
    }

    private void ApplyResolution(int index)
    {
        // Breite/Höhe aus Dropdown-Option nehmen
        string[] wh = resolutionDropdown.options[index].text.Split('x');
        int w = int.Parse(wh[0].Trim());
        int h = int.Parse(wh[1].Trim());

        float ratio = (float)w / h;
        if (Mathf.Abs(ratio - (16f / 9f)) < 0.02f)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;

        Screen.SetResolution(w, h, true);

        PlayerPrefs.SetInt("ResolutionWidth", w);
        PlayerPrefs.SetInt("ResolutionHeight", h);
        PlayerPrefs.Save();

        Debug.Log($"Resolution applied: {w} x {h}");
    }

    public static void ApplySavedResolution()
    {
        int savedWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int savedHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);

        Resolution[] resolutions = Screen.resolutions;

        // Find closest available resolution
        Resolution chosen = resolutions[0];
        int minDiff = int.MaxValue;

        for (int i = 0; i < resolutions.Length; i++)
        {
            int diff = Mathf.Abs(resolutions[i].width - savedWidth) + Mathf.Abs(resolutions[i].height - savedHeight);
            if (diff < minDiff)
            {
                minDiff = diff;
                chosen = resolutions[i];
            }
        }

        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(chosen.width, chosen.height, true);

        Debug.Log($"Applied saved resolution at startup: {chosen.width} x {chosen.height}");
    }
}
