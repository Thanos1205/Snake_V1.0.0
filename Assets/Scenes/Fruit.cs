using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject spawnMarkerPrefab; // assign the BlueMarker prefab here
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject wallUp;
    public GameObject wallDown;

    [SerializeField] private Snake snake; // reference to the Snake script

    private void Start()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        // Get wall sizes
        float leftBound = wallLeft.GetComponent<BoxCollider2D>().bounds.max.x;
        float rightBound = wallRight.GetComponent<BoxCollider2D>().bounds.min.x;
        float bottomBound = wallDown.GetComponent<BoxCollider2D>().bounds.max.y;
        float topBound = wallUp.GetComponent<BoxCollider2D>().bounds.min.y;

        // Random position within bounds and not on the snake
        Vector3 newPos;
        do
        {
            newPos = new Vector3(
                Mathf.Round(Random.Range(leftBound, rightBound)),
                Mathf.Round(Random.Range(bottomBound, topBound)),
                0f
            );
        } while (IsOnSnake(newPos));

        transform.position = newPos;

        // Spawn blue marker at this position (created for testing only)
        if (spawnMarkerPrefab != null)
            Instantiate(spawnMarkerPrefab, newPos, Quaternion.identity);
    }
    private bool IsOnSnake(Vector3 position)
    {
        // Check if the position overlaps with any segment of the snake
        foreach (Transform segment in snake.Segments)
            if (Vector3Int.RoundToInt(segment.position) == Vector3Int.RoundToInt(position))
                return true;
        return false;
    }
}
