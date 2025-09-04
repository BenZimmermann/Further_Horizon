using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public List<LevelSaveData> levels = new List<LevelSaveData>();
}
