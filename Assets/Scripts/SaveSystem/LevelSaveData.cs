using System;
using System.Collections.Generic;

[Serializable]
public class LevelSaveData
{
    public int levelIndex;          // Die Nummer, z. B. 1, 2, 3
    public string sceneName;        // Name der Unity-Szene 
    public bool isCompleted;        // Wurde dieses Level/Szene schon geschafft?
    public List<ItemData> items;    // Items, die in dieser Szene liegen

    public LevelSaveData(int index, string scene)
    {
        levelIndex = index;
        sceneName = scene;
        isCompleted = false;
        items = new List<ItemData>();
    }
}

