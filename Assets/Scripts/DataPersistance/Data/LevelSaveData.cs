using System;
using System.Collections.Generic;

/// <summary>
/// Datenpaket für EINE Szene (bei dir = ein Planet/Level).
/// </summary>
[Serializable]
public class LevelSaveData
{
    public int levelIndex;           // z. B. 1,2,3
    public string sceneName;         // "Umbra", "Cryovista", "Singara"
    public bool isCompleted;    // abfrage, ob level geschafft oder items eingesammelt sind
    public List<ItemData> items = new List<ItemData>(); // Liste für items innerhalb des level oder szene

    public LevelSaveData(int index, string scene)
    {
        levelIndex = index; 
        sceneName = scene;
        isCompleted = false;
    }
}
