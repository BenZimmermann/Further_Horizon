using System;
using System.Collections.Generic;


// Kompletter Spielstand als GameData
[Serializable]
public class GameData
{
    public string selectedPlanetScene; // aktuell ausgewählter Planet
    public List<string> unlockedPlanets = new List<string>();

    public GameData()
    {
        // Standard: Umbra + Cryovista freigeschaltet
        unlockedPlanets.Add("Umbra");
        unlockedPlanets.Add("Cryovista");

        // Singara bleibt gesperrt
        selectedPlanetScene = "Umbra"; // Startauswahl
    }
}
