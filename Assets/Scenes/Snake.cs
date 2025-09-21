// Tobi: set highscore in relation to scale of monitor (UI scaling)
// Tobi: improve highscore system (save multiple highscores, names, dates, etc.)


// Features / Power-Ups / Debuffs:
//      Snake does not increase in size
//      Snake does not speed up when eating fruit
//      Snake can reverse direction instantly
//      Snake becomes slower for 10 seconds

//      Snake can move through itself for 10 seconds
//      Snake can move through (inner) walls for 10 seconds (needs to make outer walls first)
//      Snake enters from other side when hitting wall

//      inceases score by 2 for a short time when eating fruit
//      increase score by 3

//      add second snake
//      position from fruit becomes blocked (see blue prefab) after eating it

// add sounds
// add music
// add particle effects
// add textures
// add menu
// add pause functionality


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Snake : MonoBehaviour
{
    public Transform segmentPrefab; // assign the SnakeSegment prefab in Inspector
    public TMP_Text scoreText; // assign UI Text for score display

    private Vector2 direction = Vector2.right; // initial direction
    private Vector2 nextDirection = Vector2.right; // next direction to apply
    private Queue<Vector2> inputQueue = new Queue<Vector2>(); // queue to store input directions
    private float moveTimer = 0f; // timer to track movement intervals

    private const float initialMoveSpeed = 15f; // store initial speed in a variable
    private float moveSpeed; // current speed, can increase when eating fruit

    private List<Transform> segments = new List<Transform>(); // list of all segments including head

    private int startSize = 5; // initial size of the snake
    private bool isResetting = false; // flag to prevent multiple resets

    [SerializeField] private int score = 0;       // aktueller Score
    [SerializeField] private int highScore = 0;   // Highscore

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        ResetGame();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}\nHighscore: {highScore}\nSpeed: {moveSpeed}";
    }

    private void ResetGame()
    {
        StartCoroutine(ResetAfterFade());
    }

    private IEnumerator ResetAfterFade()
    {
        isResetting = true; // stop movement and input

        // Fade out and destroy all segments except head
        for (int i = 1; i < segments.Count; i++)
            StartCoroutine(FadeAndDestroy(segments[i].gameObject));

        segments.Clear();
        segments.Add(transform); // Add head back to the list

        // wait for fade-out to complete
        yield return new WaitForSeconds(0.5f);

        // Update high score if needed
        if (score > highScore)
            highScore = score;
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();

        score = 0;

        // Move head back to center
        transform.position = Vector3.zero;
        direction = Vector2.right;
        nextDirection = Vector2.right;
        moveTimer = 0f;
        moveSpeed = initialMoveSpeed;

        // Add initial segments
        for (int i = 1; i < startSize; i++)
            Grow(false); // false = do not increase speed on initial growth

        isResetting = false; // allow movement and input again

        UpdateScoreUI();

    }

    private IEnumerator FadeAndDestroy(GameObject segment)
    {
        // unset tag to avoid further collisions during fade-out
        segment.tag = "Untagged";

        // Simple fade-out effect by scaling down
        Vector3 startScale = segment.transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            segment.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(segment);
    }

    private void Update()
    {
        // Input handling: change direction based on key presses
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && direction != Vector2.down)
            inputQueue.Enqueue(Vector2.up);
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && direction != Vector2.up)
            inputQueue.Enqueue(Vector2.down);
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && direction != Vector2.right)
            inputQueue.Enqueue(Vector2.left);
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && direction != Vector2.left)
            inputQueue.Enqueue(Vector2.right);
    }

    private void FixedUpdate()
    {
        if (isResetting) return; // solange Reset läuft, keine Bewegung

        if (inputQueue.Count > 0)
        {
            nextDirection = inputQueue.Dequeue();
        }

        // Timer controls how often the Snake moves one step
        moveTimer += Time.fixedDeltaTime;
        float moveInterval = 1f / moveSpeed; // higher speed = smaller interval

        if (moveTimer >= moveInterval)
        {
            // Move body segments from tail to head
            for (int i = segments.Count - 1; i > 0; i--)
                segments[i].position = segments[i - 1].position;

            // Update head direction for this move
            direction = nextDirection;

            // Snap head position to grid (in case of float inaccuracies)
            transform.position = new Vector3(
                Mathf.RoundToInt(transform.position.x) + direction.x,
                Mathf.RoundToInt(transform.position.y) + direction.y,
                0f
            );

            moveTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Collided with wall
            ResetGame();
        }
        else if (collision.gameObject.CompareTag("SnakeBody"))
        {
            // Collided with own body
            ResetGame();
        }
        else if (collision.gameObject.CompareTag("Fruit"))
        {
            // Call Snake’s EatFruit logic
            Grow();

            // Increase score
            score += 1;
            UpdateScoreUI();

            // Call the fruit's respawn function
            collision.GetComponent<Fruit>().RandomizePosition();
        }
    }

    private void Grow(bool increaseSpeed = true)
    {
        Vector3 newSegmentPos;

        if (segments.Count == 1)
        {
            // Only head exists: place new segment behind head
            newSegmentPos = transform.position - new Vector3(direction.x, direction.y, 0f);
        }
        else
        {
            // More than one segment: place new segment at last segment's position
            newSegmentPos = segments[segments.Count - 1].position;
        }
        // Spawn new segment at the **last segment’s position**
        Transform segment = Instantiate(segmentPrefab);
        segment.position = newSegmentPos;
        segment.tag = "SnakeBody";
        segments.Add(segment);

        // Increase speed
        if (increaseSpeed)
            moveSpeed += 1f;
    }
public List<Transform> Segments => segments;
}
