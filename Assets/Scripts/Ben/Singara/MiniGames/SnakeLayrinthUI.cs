using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using static PauseMenuController;
public class SnakeLabyrinthUI : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 10;            // Size of the square grid (gridSize x gridSize)
    public GameObject cellPrefab;        // Prefab for each grid cell (must have Image component)
    public Transform gridParent;         // Parent transform for the generated grid cells

    [Header("Colors")]
    public Color emptyColor = Color.white;
    public Color startColor = Color.green;
    public Color targetColor = Color.red;
    public Color cableColor = Color.blue;
    public Color wallColor = Color.gray;

    [Header("Gameplay")]
    public Vector2Int startPos = new Vector2Int(0, 0);
    public Vector2Int targetPos = new Vector2Int(9, 9);
    public float moveDelay = 0.25f;
    public int obstacleCount = 15;

    [Header("UI")]
    public TextMeshProUGUI countdownText;

    private Image[,] grid;  // 2D array of grid cell images
    private HashSet<Vector2Int> obstacles = new HashSet<Vector2Int>();
    private Vector2Int headPos; // Current head position
    private Vector2Int direction;   // Current move direction
    private List<Vector2Int> cable = new List<Vector2Int>();
    private float timer;    
    private bool isPlaying = false;
    private bool isPreview = false;

    public static SnakeLabyrinthUI Instance { get; private set; }

    private SnakeMiniGameClass snakeMinigameClass;
    //public SnakeMiniGameClass _snakeManager;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        // Switch controls to Snake minigame input map
        playerInput.SwitchCurrentActionMap("SnakeMinigame");
    }

    void OnDisable()
    {
        // Switch back to Player input map when disabled
        playerInput.SwitchCurrentActionMap("Player");
    }

    void Update()
    {
        if (!isPlaying || isPreview) return;

        HandleInput();// Update direction based on player input

        timer += Time.unscaledDeltaTime;
        if (timer >= moveDelay)
        {
            timer = 0f;
            Move();
        }
    }
    /// <summary>
    ///     Initialize and start the game
    /// </summary>
    /// <param name="_snakeMiniGameClass"></param>
    public void InitGame(SnakeMiniGameClass _snakeMiniGameClass)
    {
        snakeMinigameClass = _snakeMiniGameClass;
        GenerateGrid();
        StartCoroutine(PreviewAndStart());
    }

    // Countdown preview before starting the game
    IEnumerator PreviewAndStart()
    {
        isPreview = true;

        int countdown = 3;
        while (countdown > 0)
        {
            if (countdownText != null)
                countdownText.text = countdown.ToString();

            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }

        if (countdownText != null)
            countdownText.text = "Start!";

        yield return new WaitForSecondsRealtime(0.5f);

        if (countdownText != null)
            countdownText.text = "";

        StartGame();
        isPreview = false;
    }

    // Generate the grid and randomly place obstacles
    void GenerateGrid()
    {
        grid = new Image[gridSize, gridSize];
        obstacles.Clear();

        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                grid[x, y] = cell.GetComponent<Image>();
                grid[x, y].color = emptyColor;
            }
        }

        // Forbidden positions (start, target, and their neighbors)
        HashSet<Vector2Int> forbidden = new HashSet<Vector2Int>();
        forbidden.Add(startPos);
        forbidden.Add(targetPos);

        foreach (var n in GetNeighbors(startPos))
            forbidden.Add(n);

        foreach (var n in GetNeighbors(targetPos))
            forbidden.Add(n);

        // Place obstacles randomly
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector2Int pos;
            int tries = 0;
            do
            {
                pos = new Vector2Int(Random.Range(0, gridSize), Random.Range(0, gridSize));
                tries++;
                if (tries > 100) break; // Safety
            } while (forbidden.Contains(pos) || obstacles.Contains(pos));

            if (!forbidden.Contains(pos))
            {
                obstacles.Add(pos);
                grid[pos.x, pos.y].color = wallColor;
            }
        }

        // Place start and target cells
        grid[startPos.x, startPos.y].color = startColor;
        grid[targetPos.x, targetPos.y].color = targetColor;
    }
    /// <summary>
    ///  Start the actual gameplay loop
    /// </summary>
    void StartGame()
    {
        headPos = startPos;
        direction = Vector2Int.right;
        cable.Clear();
        cable.Add(headPos);

        isPlaying = true;
        timer = 0f;
    }

    // Reads input and updates the direction
    void HandleInput()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

        // Map input axes to grid directions
        if (input.y == -1) direction = Vector2Int.up;
        else if (input.y == 1) direction = Vector2Int.down;
        else if (input.x == 1) direction = Vector2Int.right;
        else if (input.x == -1) direction = Vector2Int.left;
    }
    /// <summary>
    ///   Move the head forward and check collisions
    /// </summary>
    void Move()
    {
        headPos += direction;

        // Out of bounds check
        if (headPos.x < 0 || headPos.y < 0 || headPos.x >= gridSize || headPos.y >= gridSize)
        {
            Debug.Log("Game Over: Wand!");
            isPlaying = false;
            return;
        }

        // Obstacle collision
        if (obstacles.Contains(headPos))
        {
            Debug.Log("Game Over: Hindernis getroffen!");
            isPlaying = false;
            return;
        }

        // Self-collision
        if (cable.Contains(headPos))
        {
            Debug.Log("Game Over: Kabel überkreuzt!");
            isPlaying = false;
            return;
        }

        // Add new cable segment
        cable.Add(headPos);
        grid[headPos.x, headPos.y].color = cableColor;

        // Check win condition
        if (headPos == targetPos)
        {
            Debug.Log("Level geschafft!");
            isPlaying = false;
            StartCoroutine(CloseWindow());
            if (countdownText != null)
                countdownText.text = "Modul Active!";
           
        }
    }
    // Delay before closing UI after winning
    IEnumerator CloseWindow()
    {
        yield return new WaitForSecondsRealtime(1f);
        PauseMenuController.Instance.CloseWindow();
        if (snakeMinigameClass != null)
            snakeMinigameClass.EndGame();
    }
    // Helper method: get orthogonal neighbors of a cell
    IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        yield return pos + Vector2Int.up;
        yield return pos + Vector2Int.down;
        yield return pos + Vector2Int.left;
        yield return pos + Vector2Int.right;
    }

}
