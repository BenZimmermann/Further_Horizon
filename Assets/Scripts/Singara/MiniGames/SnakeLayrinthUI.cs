using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class SnakeLabyrinthUI : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 10;
    public GameObject cellPrefab;
    public Transform gridParent;

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
    public TextMeshProUGUI countdownText;   // TMP statt UI.Text


    private Image[,] grid;
    private HashSet<Vector2Int> obstacles = new HashSet<Vector2Int>();
    private Vector2Int headPos;
    private Vector2Int direction;
    private List<Vector2Int> cable = new List<Vector2Int>();
    private float timer;
    private bool isPlaying = false;
    private bool isPreview = false;

    private PlayerInput playerInput;
    private Vector2 moveInput;

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("SnakeMinigame");
    }

    void OnDisable()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    void Update()
    {
        if (!isPlaying || isPreview) return;

        HandleInput();

        timer += Time.unscaledDeltaTime;
        if (timer >= moveDelay)
        {
            timer = 0f;
            Move();
        }
    }

    public void InitGame()
    {
        GenerateGrid();
        StartCoroutine(PreviewAndStart());
    }

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

        // Ungültige Positionen (Start, Ziel und direkte Nachbarn)
        HashSet<Vector2Int> forbidden = new HashSet<Vector2Int>();
        forbidden.Add(startPos);
        forbidden.Add(targetPos);

        // Nachbarn von Start
        foreach (var n in GetNeighbors(startPos))
            forbidden.Add(n);

        // Nachbarn von Ziel
        foreach (var n in GetNeighbors(targetPos))
            forbidden.Add(n);

        // Hindernisse zufällig platzieren
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

        // Start und Ziel setzen
        grid[startPos.x, startPos.y].color = startColor;
        grid[targetPos.x, targetPos.y].color = targetColor;
    }

    void StartGame()
    {
        headPos = startPos;
        direction = Vector2Int.right;
        cable.Clear();
        cable.Add(headPos);

        isPlaying = true;
        timer = 0f;
    }

    void HandleInput()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

        if (input.y == -1) direction = Vector2Int.up;
        else if (input.y == 1) direction = Vector2Int.down;
        else if (input.x == 1) direction = Vector2Int.right;
        else if (input.x == -1) direction = Vector2Int.left;
    }

    void Move()
    {
        headPos += direction;

        // Spielfeldgrenzen prüfen
        if (headPos.x < 0 || headPos.y < 0 || headPos.x >= gridSize || headPos.y >= gridSize)
        {
            Debug.Log("Game Over: Wand!");
            isPlaying = false;
            return;
        }

        // Hindernisse prüfen
        if (obstacles.Contains(headPos))
        {
            Debug.Log("Game Over: Hindernis getroffen!");
            isPlaying = false;
            return;
        }

        // Selbstkollision prüfen
        if (cable.Contains(headPos))
        {
            Debug.Log("Game Over: Kabel überkreuzt!");
            isPlaying = false;
            return;
        }

        // Kabel hinzufügen
        cable.Add(headPos);
        grid[headPos.x, headPos.y].color = cableColor;

        // Siegbedingung
        if (headPos == targetPos)
        {
            Debug.Log("Level geschafft!");
            isPlaying = false;
            if (countdownText != null)
                countdownText.text = "Modul Active!";
        }
    }

    // Hilfsmethode: Nachbarfelder
    IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        yield return pos + Vector2Int.up;
        yield return pos + Vector2Int.down;
        yield return pos + Vector2Int.left;
        yield return pos + Vector2Int.right;
    }
}
