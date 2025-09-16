using System;
using System.Collections.Generic;


[Serializable]
public class GameData
{
    public string selectedPlanetScene;       // aktuell ausgewählter Planet
    public List<string> unlockedPlanets = new List<string>();

    // Inventar
    public List<ItemSaveData> items = new List<ItemSaveData>();

    public GameData()
    {
        // Standard: Umbra + Cryovista freigeschaltet
        unlockedPlanets.Add("Umbra");
        unlockedPlanets.Add("Cryovista");

        // Singara bleibt gesperrt
        selectedPlanetScene = "Umbra";

        // Beispiel/Testeinträge
        items.Add(new ItemSaveData { itemId = "potion_01", collected = 5 });
        items.Add(new ItemSaveData { itemId = "wood_01", collected = 20 });
    }
}
