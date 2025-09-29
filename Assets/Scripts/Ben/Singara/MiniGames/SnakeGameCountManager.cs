using UnityEngine;

public class SnakeGameCountManager : MonoBehaviour
{
    public static SnakeGameCountManager Instance { get; private set; }

    // Counter for how many Snake games have been completed
    private int completedGames = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCompletedGame()
    {
        completedGames++;
        Debug.Log("Snake Games abgeschlossen: " + completedGames);

        if (completedGames >= 2)
        {
            Debug.Log("Alle Snake-Minigames abgeschlossen!");
        }
    }
}

