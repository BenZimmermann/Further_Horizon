using UnityEngine;

public class SnakeGameCountManager : MonoBehaviour
{
    public static SnakeGameCountManager Instance { get; private set; }

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

        // Beispiel: Wenn beide erledigt sind
        if (completedGames >= 2)
        {
            Debug.Log("Alle Snake-Minigames abgeschlossen!");
        }
    }
}

