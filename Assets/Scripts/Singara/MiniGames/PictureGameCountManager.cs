using UnityEngine;

public class PictureGameCountManager : MonoBehaviour
{
    public static PictureGameCountManager Instance { get; private set; }

    private int completedPuzzles = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCompletedPuzzle()
    {
        completedPuzzles++;

        // Beispiel: Wenn beide erledigt sind
        if (completedPuzzles >= 3)
        {
            Debug.Log("Alle Puzzle-Minigames abgeschlossen!");
        }
    }
}