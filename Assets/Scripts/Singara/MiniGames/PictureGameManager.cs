using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PictureGameManager : MonoBehaviour
{
    public static PictureGameManager Instance { get; private set; }

    public PuzzleRow[] rows;   // Im Inspector mit den 5 UI-Reihen verknüpfen
    public Sprite[] allParts;  // Die 5 Bild-Segmente in richtiger Reihenfolge

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void InitGame()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].Init(allParts, i);
            // Jede Reihe kennt ihr "richtiges Teil", 
            // startet aber zufällig in Init()
        }
    }


    public void CheckWin()
    {
        foreach (var row in rows)
        {
            if (!row.IsCorrect()) return;
        }

        Debug.Log("Puzzle fertiggestellt!");
        StartCoroutine(WaitSec());
        
        // -> hier dein EndGame(), Quest-Fortschritt, UI schließen etc.
    }
    IEnumerator WaitSec()
    {
        yield return new WaitForSecondsRealtime(2f);
        PictureGameClass.ActiveInstance.EndGame();
    }
}
