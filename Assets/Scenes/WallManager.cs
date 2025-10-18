using UnityEngine;

public class WallSetup : MonoBehaviour
{
    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallTop;
    public Transform wallBottom;

    [Tooltip("Dicke der Wände in Welt-Einheiten")]
    public float wallThickness = 1f;

    [Header("Abstände vom Rand (prozentual zur Bildschirmgröße)")]
    [Range(0f, 0.3f)] public float horizontalInsetRatio = 0.05f; // 5% Abstand links/rechts
    [Range(0f, 0.3f)] public float verticalInsetRatio = 0.10f;   // 10% Abstand oben/unten

    void Start()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // Berechne Abstände
        float horizontalInset = camWidth * horizontalInsetRatio;
        float verticalInset = camHeight * verticalInsetRatio;

        // Innenfläche
        float innerWidth = camWidth - (horizontalInset * 2f);
        float innerHeight = camHeight - (verticalInset * 2f);

        // Positionen der Wände
        float leftX = cam.transform.position.x - innerWidth / 2f - wallThickness / 2f;
        float rightX = cam.transform.position.x + innerWidth / 2f + wallThickness / 2f;
        float topY = cam.transform.position.y + innerHeight / 2f + wallThickness / 2f;
        float bottomY = cam.transform.position.y - innerHeight / 2f - wallThickness / 2f;

        // Positionieren
        wallLeft.position = new Vector3(leftX, cam.transform.position.y, 0);
        wallRight.position = new Vector3(rightX, cam.transform.position.y, 0);
        wallTop.position = new Vector3(cam.transform.position.x, topY, 0);
        wallBottom.position = new Vector3(cam.transform.position.x, bottomY, 0);

        // Skalieren (leicht überlappen lassen)
        float overlap = wallThickness * 2f; // 50% Überlappung für saubere Ecken

        wallLeft.localScale = new Vector3(wallThickness, innerHeight + overlap, 1);
        wallRight.localScale = new Vector3(wallThickness, innerHeight + overlap, 1);
        wallTop.localScale = new Vector3(innerWidth + overlap, wallThickness, 1);
        wallBottom.localScale = new Vector3(innerWidth + overlap, wallThickness, 1);
    }
}
