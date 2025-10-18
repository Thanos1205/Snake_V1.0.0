using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;

    void Awake()
    {
        // Wenn es schon eine Instanz gibt, lösche diese neue
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Musik über Szenen hinweg behalten
        DontDestroyOnLoad(gameObject);
    }
}
