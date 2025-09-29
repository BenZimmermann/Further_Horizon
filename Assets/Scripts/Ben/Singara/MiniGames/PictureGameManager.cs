using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PictureGameManager : MonoBehaviour
{
    public static PictureGameManager Instance { get; private set; }

    private PictureGameClass pictureGameClass; // Reference to the current game logic instance
    public PuzzleRow[] rows;   // The 5 puzzle rows (assign via Inspector)
    public Sprite[] allParts;  // The 5 puzzle pieces in the correct order

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    ///  Initializes the puzzle game with the given PictureGameClass reference
    /// </summary>
    /// <param name="_pictureGameClass"></param>
    public void InitGame(PictureGameClass _pictureGameClass)
    {
        pictureGameClass = _pictureGameClass;
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].Init(allParts, i);
            // Each row knows which piece is correct,
            // but starts with a randomized setup in Init()
        }
    }

    /// <summary>
    /// Checks whether the player has completed the puzzle correctly
    /// </summary>
    public void CheckWin()
    {
        foreach (var row in rows)
        {
            if (!row.IsCorrect()) return;
        }
        Debug.Log("Puzzle fertiggestellt!");
        StartCoroutine(WaitSec());
    }
    // Small delay before finishing the puzzle (e.g. show success effect first)
    IEnumerator WaitSec()
    {
        yield return new WaitForSecondsRealtime(2f);
        if(pictureGameClass != null)
            pictureGameClass.EndGame();
        //PictureGameClass.ActiveInstance.EndGame();

    }
}
