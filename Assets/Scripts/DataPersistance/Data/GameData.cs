using System;
using System.Collections.Generic;


// Kompletter Spielstand als GameData
[Serializable]
public class GameData
{
    // Beispiel: Szene/Level → Items (mappe das später Umbra/Cryovista/Singara)
    public List<LevelSaveData> levels = new List<LevelSaveData>();

    public string lastSceneName = "Inventory";
    public DateTime lastSavedUtc;

    public GameData()
    {
        // Default-Werte (falls noch kein Save existiert)
        lastSceneName = "Inventory";
        lastSavedUtc = DateTime.UtcNow;

        // → Hier unsere drei Planeten als Startzustand anlegen
        //   Oder du machst das woanders (z. B. in einem "New Game" Flow).
    }
}
